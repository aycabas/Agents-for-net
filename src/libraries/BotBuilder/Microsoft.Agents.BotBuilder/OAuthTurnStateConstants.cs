﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.Agents.BotBuilder
{
    /// <summary>
    /// Constants used in TurnState.
    /// </summary>
    public static class OAuthTurnStateConstants
    {
        /// <summary>
        /// Default amount of time an OAuthCard will remain active (clickable and actively waiting for a token).
        /// After this time:
        /// (1) the OAuthCard will not allow the user to click on it.
        /// (2) any polling triggered by the OAuthCard will stop.
        /// </summary>
        public static readonly TimeSpan OAuthLoginTimeoutValue = TimeSpan.FromMinutes(15);
    }
}
