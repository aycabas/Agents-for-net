﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Agents.Extensions.Teams.Models;
using Xunit;

namespace Microsoft.Agents.Extensions.Teams.Tests.Model
{
    public class TabEntityContextTests
    {
        [Theory]
        [InlineData("files")]
        [InlineData(null)]
        public void TabEntityContextInits(string tabEntityId)
        {
            var tabEntityContext = new TabEntityContext()
            {
                TabEntityId = tabEntityId
            };

            Assert.NotNull(tabEntityContext);
            Assert.IsType<TabEntityContext>(tabEntityContext);
            Assert.Equal(tabEntityId, tabEntityContext.TabEntityId);
        }
    }
}
