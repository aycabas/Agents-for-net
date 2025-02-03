﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Agents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Agents.State
{
    public class TempState : IBotState
    {
        public static readonly string ScopeName = "temp";
        private readonly Dictionary<string, object> _state = [];

        public const string AuthScopeKey = "AuthScope";
        public const string InvokeResponseKey = "InvokeResponse";
        public const string BotIdentityKey = "BotIdentity";

        public string Name => ScopeName;

        public void ClearState()
        {
            _state.Clear();
        }

        public Task DeleteStateAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            _state.Clear();
            return Task.CompletedTask;
        }

        public bool HasValue(string name)
        {
            return ObjectPath.HasValue(_state, name);
        }

        public void DeleteValue(string name)
        {
            ObjectPath.RemovePathValue(_state, name);
        }

        public T GetValue<T>(string name, Func<T> defaultValueFactory = null)
        {
            T result = default;

            try
            {
                result = ObjectPath.GetPathValue<T>(_state, name, true);
                if (result == null)
                {
                    if (defaultValueFactory != null)
                    {
                        result = defaultValueFactory();
                    }
                    SetValue(name, result);
                }
            }
            catch (KeyNotFoundException)
            {
                if (defaultValueFactory != null)
                {
                    // use default Value Factory and save default value for any further calls
                    result = defaultValueFactory();
                    SetValue(name, result);
                }
            }

            return result;
        }

        public bool IsLoaded()
        {
            return true;
        }

        public Task LoadAsync(ITurnContext turnContext, bool force = false, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(ITurnContext turnContext, bool force = false, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void SetValue<T>(string name, T value)
        {
            ObjectPath.SetPathValue(_state, name, value);
        }
    }
}
