﻿/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace doLittle.Runtime.Events.Storage.Development
{
    /// <summary>
    /// Delegate providing path to where to store <see cref="EventSourceVersion"/> for each <see cref="EventSource"/>
    /// </summary>
    public delegate string ICanProvideEventSourceVersionsPath();
}