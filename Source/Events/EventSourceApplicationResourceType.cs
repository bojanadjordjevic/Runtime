﻿/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using doLittle.Applications;

namespace doLittle.Events
{
    /// <summary>
    /// Represents a <see cref="IApplicationResourceType">application resource type</see> for 
    /// <see cref="IEventSource">events</see>
    /// </summary>
    public class EventSourceApplicationResourceType : IApplicationResourceType
    {
        /// <inheritdoc/>
        public string Identifier => "EventSource";

        /// <inheritdoc/>
        public Type Type => typeof(IEventSource);

        /// <inheritdoc/>
        public ApplicationArea Area => ApplicationAreas.Domain;
    }
}
