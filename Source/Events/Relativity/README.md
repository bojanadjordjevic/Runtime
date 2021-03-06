# Events Relativity

## Protobuf

We're using [gRPC](https://www.grpc.io) for communication which relies on protobuf.
All available services and messages are defined through `.proto` files. After changing these you need to
generate the source by running the following in your shell:

```shell
$ ./generate_protos.sh
```

## Vocabulary

The vocabulary in this is loosely based on [general relativity](https://en.wikipedia.org/wiki/General_relativity) and a tribute to the late professor [Stephen Hawking](https://en.wikipedia.org/wiki/Stephen_Hawkings) with a semi scientific approach linking the concepts
to what they actually do in the software. You can find a fun video [here](https://www.youtube.com/watch?v=E8hzLM0JpYw) that explains event horizons in 60 seconds.

### [Event Horizon](https://en.wikipedia.org/wiki/Event_horizon)

The event horizon represents the final entry for committed events. At this point they can only be seen by other singularities.
In Dolittle, a singularity would then be a running node that is connected to this event horizon and receives a stream of particles.
The particles, representing committed events. By committed events, we mean events that has been persisted into an event store.

### [Singularity](https://en.wikipedia.org/wiki/Gravitational_singularity)

A singularity represents a single destination point for an event-particle. An event horizon can [spaghettify](https://en.wikipedia.org/wiki/Spaghettification) particles into multiple singularities.

### [Quantum Tunnel](https://en.wikipedia.org/wiki/Quantum_tunnelling)

Each singularity can connect to any event horizon, they establish a quantum tunnel for the purpose of passing particles through.

### [Wave Function](https://en.wikipedia.org/wiki/Wave_function)

Part of the process moving through a quantum tunnel means at times the [quantum state](https://en.wikipedia.org/wiki/Quantum_state) gets collapsed.
The state, being an event particle has the possibility to change between different versions of the the software.
This process is described sa [wave function collapse](https://en.wikipedia.org/wiki/Wave_function_collapse).
A undefined process in Dolittle, but seems interesting is the [wave function renormalization](https://en.wikipedia.org/wiki/Wave_function_renormalization).

### [Particle](https://en.wikipedia.org/wiki/Particle)

Particles are small objects, and in Dolittle there is an event particle. This is the thing that passes through the event
horizon into each singularity.

### [Barrier](https://en.wikipedia.org/wiki/Rectangular_potential_barrier)

For quantum tunnels to be opened from a singularity towards an event horizon, it has to penetrate the barrier.
This is the last line of defense for connecting - the barrier can refuse the opening of the tunnel.

### [Gravitational Lens](https://en.wikipedia.org/wiki/Gravitational_lens)

A gravitational lens is a distribution of matter between a distant light source and an observer, that is capable of bending the light from the source as the light travels towards the observer.
In order to observe black holes and its event horizons, one can do so through observing the gravitational lens.
Translated, this means the actual server that keeps the connection and observes (or in fact waits) for black holes with its quantum tunnels and singularities.

### [Geodesics](https://vrs.amsi.org.au/geodesic-incompleteness-spacetime/)

An observer travelling along a geodesic path may remain in motion forever, or the path may terminate after a finite amount of time. Paths that carry on indefinitely are called complete geodesics, and those that stop abruptly, incomplete geodesics.
This relates to how far in the offset in which a singularity, in our case a bounded context has reached when connected to an event horizon.
