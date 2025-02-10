﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Text.Json;

namespace Microsoft.Agents.BotBuilder.App.State
{
    /// <summary>
    /// Accessor class for managing an individual state scope.
    /// </summary>
    public class TurnStateEntry
    {
        private Record _value;
        private string _hash;
        private static readonly JsonSerializerOptions _serializerOptions = new() { MaxDepth = 64 };

        /// <summary>
        /// Constructs the turn state entry.
        /// </summary>
        /// <param name="value">Value to initialize the state scope with. The default is an {} object.</param>
        /// <param name="storageKey">Storage key to use when persisting the state scope.</param>
        public TurnStateEntry(Record value, string? storageKey = null)
        {
            ArgumentNullException.ThrowIfNull(value);
            _value = value;
            StorageKey = storageKey;
            _hash = ComputeHash(value);
        }

        /// <inheritdoc />
        public bool HasChanged
        {
            get { return ComputeHash(_value!) != _hash; }
        }

        /// <inheritdoc />
        public bool IsDeleted { get; private set; } = false;

        /// <inheritdoc />
        public Record? Value
        {
            get
            {
                if (IsDeleted)
                {
                    _value = new();
                    IsDeleted = false;
                }

                return _value;
            }
        }

        /// <inheritdoc />
        public string? StorageKey { get; }

        /// <summary>
        /// Clears the state scope.
        /// </summary>
        public void Delete()
        {
            IsDeleted = true;
        }

        /// <summary>
        /// Replaces the state scope with a new value.
        /// </summary>
        /// <param name="value">New value to replace the state scope with.</param>
        public void Replace(Record value)
        {
            ArgumentNullException.ThrowIfNull(value);
            _value = value;
        }

        // TODO: Optimize if possible
        /// <summary>
        /// Computes the hash from the object
        /// </summary>
        /// <param name="obj">The object to compute has from</param>
        /// <returns>Returns a Json object representation </returns>
        internal static string ComputeHash(object obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return JsonSerializer.Serialize(obj, _serializerOptions);
        }
    }
}
