/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Collections;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Relativity.Protobuf;
using Dolittle.Runtime.Events.Store;
using Dolittle.Serialization.Protobuf;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Events.Relativity.Grpc
{
    /// <summary>
    /// Represents a concrete connection through a <see cref="IBarrier"/>
    /// </summary>
    public class QuantumTunnelConnection : IDisposable
    {
        readonly string _url;
        readonly IEnumerable<Dolittle.Artifacts.Artifact> _events;
        readonly ILogger _logger;
        readonly Application _application;
        readonly BoundedContext _boundedContext;
        readonly Channel _channel;
        readonly QuantumTunnelService.QuantumTunnelServiceClient _client;
        readonly Application _destinationApplication;
        readonly BoundedContext _destinationBoundedContext;
        readonly IGeodesics _geodesics;
        readonly ISerializer _serializer;
        readonly IEventStore _eventStore;
        readonly IEventProcessors _eventProcessors;
        
        readonly CancellationTokenSource _runCancellationTokenSource;
        readonly CancellationToken _runCancellationToken;
        Thread _runThread = null;

        /// <summary>
        /// Initializes a new instance of <see cref="QuantumTunnelConnection"/>
        /// </summary>
        /// <param name="application">The current <see cref="Application"/></param>
        /// <param name="boundedContext">The current <see cref="BoundedContext"/></param>
        /// <param name="destinationApplication">The destination <see cref="Application"/></param>
        /// <param name="destinationBoundedContext">The destination <see cref="BoundedContext"/></param>
        /// <param name="url">Url for the <see cref="IEventHorizon"/> we're connecting to</param>
        /// <param name="events"><see cref="IEnumerable{Artifact}">Events</see> to connect for</param>
        /// <param name="geodesics"><see cref="IGeodesics"/> for path offsetting</param>
        /// <param name="eventStore"><see cref="IEventStore"/> to persist incoming events to</param>
        /// <param name="eventProcessors"><see cref="IEventProcessors"/> for processing incoming events</param>
        /// <param name="serializer"><see cref="ISerializer"/> to use for deserializing content of commits</param>
        /// <param name="logger"><see cref="ILogger"/> for logging purposes</param>
        public QuantumTunnelConnection(
                Application application,
                BoundedContext boundedContext,
                Application destinationApplication,
                BoundedContext destinationBoundedContext,
                string url,
                IEnumerable<Dolittle.Artifacts.Artifact> events,
                IGeodesics geodesics,
                IEventStore eventStore,
                IEventProcessors eventProcessors,
                ISerializer serializer,
                ILogger logger)
        {
            _url = url;
            _events = events;
            _logger = logger;
            _application = application;
            _boundedContext = boundedContext;
            _geodesics = geodesics;
            _serializer = serializer;
            _eventStore = eventStore;
            _eventProcessors = eventProcessors;
            _destinationApplication = destinationApplication;
            _destinationBoundedContext = destinationBoundedContext;
            _channel = new Channel(_url, ChannelCredentials.Insecure);
            _client = new QuantumTunnelService.QuantumTunnelServiceClient(_channel);

            _runCancellationTokenSource = new CancellationTokenSource();
            _runCancellationToken = _runCancellationTokenSource.Token;

            Task.Run(() => Run(), _runCancellationToken);

            AppDomain.CurrentDomain.ProcessExit += ProcessExit;
            AssemblyLoadContext.Default.Unloading += AssemblyLoadContextUnloading;
            Console.CancelKeyPress += (s, e) => Close();
            
        }

        /// <summary>
        /// Destructs the <see cref="QuantumTunnelConnection"/>
        /// </summary>
        ~QuantumTunnelConnection()
        {
            Dispose();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            AppDomain.CurrentDomain.ProcessExit -= ProcessExit;
            AssemblyLoadContext.Default.Unloading -= AssemblyLoadContextUnloading;

            Close();
        }

        void ProcessExit(object sender, EventArgs e)
        {
            Close();
        }

        void AssemblyLoadContextUnloading(AssemblyLoadContext context)
        {
            Close();
        }

        void Close()
        {
            _runCancellationTokenSource.Cancel();

            //if( _runThread != null ) _runThread.Abort();

            _logger.Information("Collapsing quantum tunnel");
            _channel.ShutdownAsync();
        }

        void Run()
        {
            _logger.Information($"Establishing connection towards event horizon for application ('{_destinationApplication}') and bounded context ('{_destinationBoundedContext}') at '{_url}'");

            Task.Run(async() =>
            {
                _runThread = Thread.CurrentThread;
                for(;;)
                {
                    
                    _runCancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        await OpenAndHandleStream();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Error occurred during establishing quantum tunnel");
                    }
                    _logger.Warning("Connection broken - backing off for a second");
                    Thread.Sleep(1000);
                    _logger.Warning("Trying to reconnect");
                }
            }).Wait();

            Close();
        }

        async Task OpenAndHandleStream()
        {
            _logger.Information($"Opening tunnel towards application '{_application}' and bounded context '{_boundedContext}'");

            var openTunnelMessage = new OpenTunnel
            {
                Application = _application.ToProtobuf(),
                BoundedContext = _boundedContext.ToProtobuf(),
                ClientId = Guid.NewGuid().ToProtobuf()
            };

            _events.Select(_ => _.ToMessage()).ForEach(openTunnelMessage.Events.Add);

            var stream = _client.Open(openTunnelMessage);
            try
            {
                while (await stream.ResponseStream.MoveNext(_runCancellationToken))
                {
                    _logger.Information("Commit received");

                    try
                    {
                        var current = stream.ResponseStream.Current.ToCommittedEventStream(_serializer);
                        var version = _eventStore.GetVersionFor(current.Source.EventSource);
                        version = new Store.EventSourceVersion(version.Commit+1,0);

                        var versionedEventSource = new Store.VersionedEventSource(version, current.Source.EventSource, current.Source.Artifact);

                        var eventEnvelopes = new List<Store.EventEnvelope>();

                        current.Events.ForEach(_ => 
                        {
                            var envelope = new Store.EventEnvelope(
                                _.Id, 
                                new Store.EventMetadata(
                                    new Store.VersionedEventSource(version, current.Source.EventSource, current.Source.Artifact),
                                    _.Metadata.CorrelationId,
                                    _.Metadata.Artifact,
                                    _.Metadata.CausedBy,
                                    _.Metadata.Occurred
                                ), 
                                _.Event
                            );
                            eventEnvelopes.Add(envelope);

                            version = version.IncrementSequence();
                        });

                        var uncommittedEventStream = new Store.UncommittedEventStream(
                            current.Id,
                            current.CorrelationId,
                            versionedEventSource,
                            current.Timestamp,
                            new Store.EventStream(eventEnvelopes)
                        );

                        _logger.Information("Commit events to store");
                        var committedEventStream = _eventStore.Commit(uncommittedEventStream);

                        _logger.Information("Process committed events");
                        _eventProcessors.Process(committedEventStream);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Couldn't handle incoming commit");
                    }
                }
            }
            catch (Exception moveException)
            {
                _logger.Error(moveException, "There was a problem moving to the next item in the stream");

            }

            _logger.Information("Done opening and handling the stream");
        }

    }
}