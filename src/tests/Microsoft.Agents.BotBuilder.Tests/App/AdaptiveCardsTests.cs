﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Agents.BotBuilder.App;
using Microsoft.Agents.BotBuilder.App.AdaptiveCards;
using Microsoft.Agents.BotBuilder.Testing;
using Microsoft.Agents.BotBuilder.Tests.App.TestUtils;
using Microsoft.Agents.Core.Models;
using Microsoft.Agents.Core.Serialization;
using Moq;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.Agents.BotBuilder.Tests.App
{
    public class AdaptiveCardsTests
    {
        [Fact]
        public async Task Test_OnActionExecute_Verb()
        {
            // Arrange
            IActivity[] activitiesToSend = null;
            void CaptureSend(IActivity[] arg)
            {
                activitiesToSend = arg;
            }
            var adapter = new SimpleAdapter(CaptureSend);
            var turnContext = new TurnContext(adapter, new Activity()
            {
                Type = ActivityTypes.Invoke,
                Name = "adaptiveCard/action",
                Value = ProtocolJsonSerializer.ToObject<JsonElement>(new
                {
                    action = new
                    {
                        type = "Action.Execute",
                        verb = "test-verb",
                        data = new { testKey = "test-value" }
                    }
                }),
                Recipient = new() { Id = "recipientId" },
                Conversation = new() { Id = "conversationId" },
                From = new() { Id = "fromId" },
                ChannelId = "channelId",
            });
            var adaptiveCardInvokeResponseMock = new Mock<AdaptiveCardInvokeResponse>();
            var expectedInvokeResponse = new InvokeResponse()
            {
                Status = 200,
                Body = adaptiveCardInvokeResponseMock.Object,
            };
            var turnState = TurnStateConfig.GetTurnStateWithConversationStateAsync(turnContext);

            var app = new Application(new()
            {
                StartTypingTimer = false,
                TurnStateFactory = () => turnState.Result,
            });
            ActionExecuteHandlerAsync handler = (turnContext, turnState, data, cancellationToken) =>
            {
                TestAdaptiveCardActionData actionData = Cast<TestAdaptiveCardActionData>(data);
                Assert.Equal("test-value", actionData.TestKey);
                return Task.FromResult(adaptiveCardInvokeResponseMock.Object);
            };

            // Act
            app.AdaptiveCards.OnActionExecute("test-verb", handler);
            await app.OnTurnAsync(turnContext);

            // Assert
            Assert.NotNull(activitiesToSend);
            Assert.Single(activitiesToSend);
            Assert.Equal("invokeResponse", activitiesToSend[0].Type);
            Assert.Equivalent(expectedInvokeResponse, activitiesToSend[0].Value);
        }

        [Fact]
        public async Task Test_OnActionExecute_Verb_NotHit()
        {
            // Arrange
            IActivity[] activitiesToSend = null;
            void CaptureSend(IActivity[] arg)
            {
                activitiesToSend = arg;
            }
            var adapter = new SimpleAdapter(CaptureSend);
            var turnContext1 = new TurnContext(adapter, new Activity()
            {
                Type = ActivityTypes.Invoke,
                Name = "adaptiveCard/action",
                Value = ProtocolJsonSerializer.ToObject<JsonElement>(new
                {
                    action = new
                    {
                        type = "Action.Execute",
                        verb = "not-test-verb"
                    }
                }),
                Recipient = new() { Id = "recipientId" },
                Conversation = new() { Id = "conversationId" },
                From = new() { Id = "fromId" },
                ChannelId = "channelId",
            });
            var turnContext2 = new TurnContext(adapter, new Activity()
            {
                Type = ActivityTypes.Invoke,
                Name = "application/search",
                Conversation = new() { Id = "conversationId" },
                From = new() { Id = "fromId" },
                ChannelId = "channelId"
            });
            var adaptiveCardInvokeResponseMock = new Mock<AdaptiveCardInvokeResponse>();
            var turnState = TurnStateConfig.GetTurnStateWithConversationStateAsync(turnContext1);

            var app = new Application(new()
            {
                StartTypingTimer = false,
                TurnStateFactory = () => turnState.Result,
            });
            ActionExecuteHandlerAsync handler = (turnContext, turnState, data, cancellationToken) =>
            {
                return Task.FromResult(adaptiveCardInvokeResponseMock.Object);
            };

            // Act
            app.AdaptiveCards.OnActionExecute("test-verb", handler);
            await app.OnTurnAsync(turnContext1);
            await app.OnTurnAsync(turnContext2);

            // Assert
            Assert.Null(activitiesToSend);
        }

        [Fact]
        public async Task Test_OnActionExecute_RouteSelector_ActivityNotMatched()
        {
            var adapter = new SimpleAdapter();
            var turnContext = new TurnContext(adapter, new Activity()
            {
                Type = ActivityTypes.Invoke,
                Name = "application/search",
                Recipient = new() { Id = "recipientId" },
                Conversation = new() { Id = "conversationId" },
                From = new() { Id = "fromId" },
                ChannelId = "channelId",
            });
            var adaptiveCardInvokeResponseMock = new Mock<AdaptiveCardInvokeResponse>();
            var turnState = TurnStateConfig.GetTurnStateWithConversationStateAsync(turnContext);

            var app = new Application(new()
            {
                StartTypingTimer = false,
                TurnStateFactory = () => turnState.Result,
            });
            RouteSelectorAsync routeSelector = (turnContext, cancellationToken) =>
            {
                return Task.FromResult(true);
            };
            ActionExecuteHandlerAsync handler = (turnContext, turnState, data, cancellationToken) =>
            {
                return Task.FromResult(adaptiveCardInvokeResponseMock.Object);
            };

            // Act
            app.AdaptiveCards.OnActionExecute(routeSelector, handler);
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await app.OnTurnAsync(turnContext));

            // Assert
            Assert.Equal("Unexpected AdaptiveCards.OnActionExecute() triggered for activity type: invoke", exception.Message);
        }

        [Fact]
        public async Task Test_OnActionSubmit_Verb()
        {
            // Arrange
            var adapter = new SimpleAdapter();
            var turnContext = new TurnContext(adapter, new Activity()
            {
                Type = ActivityTypes.Message,
                Value = ProtocolJsonSerializer.ToObject<JsonElement>(new
                {
                    verb = "test-verb",
                    testKey = "test-value"
                }),
                Recipient = new("test-id"),
                Conversation = new() { Id = "conversationId" },
                From = new() { Id = "fromId" },
                ChannelId = "channelId",
            });
            var turnState = TurnStateConfig.GetTurnStateWithConversationStateAsync(turnContext);

            var app = new Application(new()
            {
                StartTypingTimer = false,
                TurnStateFactory = () => turnState.Result,
            });
            var called = false;
            ActionSubmitHandler handler = (turnContext, turnState, data, cancellationToken) =>
            {
                called = true;
                TestAdaptiveCardSubmitData submitData = Cast<TestAdaptiveCardSubmitData>(data);
                Assert.Equal("test-verb", submitData.Verb);
                Assert.Equal("test-value", submitData.TestKey);
                return Task.CompletedTask;
            };

            // Act
            app.AdaptiveCards.OnActionSubmit("test-verb", handler);
            await app.OnTurnAsync(turnContext);

            // Assert
            Assert.True(called);
        }

        [Fact]
        public async Task Test_OnActionSubmit_Verb_NotHit()
        {
            // Arrange
            var adapter = new SimpleAdapter();
            var turnContext = new TurnContext(adapter, new Activity()
            {
                Type = ActivityTypes.Message,
                Value = ProtocolJsonSerializer.ToObject<JsonElement>(new
                {
                    verb = "test-verb",
                    testKey = "test-value"
                }),
                Recipient = new("test-id"),
                Conversation = new() { Id = "conversationId" },
                From = new() { Id = "fromId" },
                ChannelId = "channelId",
            });
            var turnState = TurnStateConfig.GetTurnStateWithConversationStateAsync(turnContext);
            var app = new Application(new()
            {
                StartTypingTimer = false,
                TurnStateFactory = () => turnState.Result,
            });
            var called = false;
            ActionSubmitHandler handler = (turnContext, turnState, data, cancellationToken) =>
            {
                called = true;
                TestAdaptiveCardSubmitData submitData = Cast<TestAdaptiveCardSubmitData>(data);
                Assert.Equal("test-verb", submitData.Verb);
                Assert.Equal("test-value", submitData.TestKey);
                return Task.CompletedTask;
            };

            // Act
            app.AdaptiveCards.OnActionSubmit("not-test-verb", handler);
            await app.OnTurnAsync(turnContext);

            // Assert
            Assert.False(called);
        }

        [Fact]
        public async Task Test_OnActionSubmit_RouteSelector_ActivityNotMatched()
        {
            // Arrange
            var adapter = new SimpleAdapter();
            var turnContext = new TurnContext(adapter, new Activity()
            {
                Type = ActivityTypes.Message,
                Text = "test-text",
                Recipient = new("test-id"),
                Conversation = new() { Id = "conversationId" },
                From = new() { Id = "fromId" },
                ChannelId = "channelId",
            });
            var turnState = TurnStateConfig.GetTurnStateWithConversationStateAsync(turnContext);

            var app = new Application(new()
            {
                StartTypingTimer = false,
                TurnStateFactory = () => turnState.Result,
            });
            RouteSelectorAsync routeSelector = (turnContext, cancellationToken) =>
            {
                return Task.FromResult(true);
            };
            ActionSubmitHandler handler = (turnContext, turnState, data, cancellationToken) =>
            {
                return Task.CompletedTask;
            };

            // Act
            app.AdaptiveCards.OnActionSubmit(routeSelector, handler);
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await app.OnTurnAsync(turnContext));

            // Assert
            Assert.Equal("Unexpected AdaptiveCards.OnActionSubmit() triggered for activity type: message", exception.Message);
        }

        [Fact]
        public async Task Test_OnSearch_Dataset()
        {
            // Arrange
            IActivity[] activitiesToSend = null;
            void CaptureSend(IActivity[] arg)
            {
                activitiesToSend = arg;
            }
            var adapter = new SimpleAdapter(CaptureSend);
            var turnContext = new TurnContext(adapter, new Activity()
            {
                Type = ActivityTypes.Invoke,
                Name = "application/search",
                Value = ProtocolJsonSerializer.ToObject<JsonElement>(new
                {
                    kind = "search",
                    queryText = "test-query",
                    queryOptions = new
                    {
                        skip = 0,
                        top = 15
                    },
                    dataset = "test-dataset"
                }),
                Recipient = new("test-id"),
                Conversation = new() { Id = "conversationId" },
                From = new() { Id = "fromId" },
                ChannelId = "channelId",
            });
            IList<AdaptiveCardsSearchResult> searchResults = new List<AdaptiveCardsSearchResult>
            {
                new("test-title", "test-value")
            };
            var expectedInvokeResponse = new InvokeResponse()
            {
                Status = 200,
                Body = new SearchInvokeResponse()
                {
                    StatusCode = 200,
                    Type = "application/vnd.microsoft.search.searchResponse",
                    Value = new
                    {
                        Results = searchResults
                    }
                }
            };
            var turnState = TurnStateConfig.GetTurnStateWithConversationStateAsync(turnContext);

            var app = new Application(new()
            {
                StartTypingTimer = false,
                TurnStateFactory = () => turnState.Result,
            });
            SearchHandlerAsync handler = (turnContext, turnState, query, cancellationToken) =>
            {
                Assert.Equal("test-query", query.Parameters.QueryText);
                Assert.Equal("test-dataset", query.Parameters.Dataset);
                return Task.FromResult(searchResults);
            };

            // Act
            app.AdaptiveCards.OnSearch("test-dataset", handler);
            await app.OnTurnAsync(turnContext);

            // Assert
            Assert.NotNull(activitiesToSend);
            Assert.Single(activitiesToSend);
            Assert.Equal("invokeResponse", activitiesToSend[0].Type);
            Assert.Equivalent(expectedInvokeResponse, activitiesToSend[0].Value);
        }

        [Fact]
        public async Task Test_OnSearch_Dataset_NotHit()
        {
            // Arrange
            IActivity[] activitiesToSend = null;
            void CaptureSend(IActivity[] arg)
            {
                activitiesToSend = arg;
            }
            var adapter = new SimpleAdapter(CaptureSend);
            var turnContext = new TurnContext(adapter, new Activity()
            {
                Type = ActivityTypes.Invoke,
                Name = "application/search",
                Value = ProtocolJsonSerializer.ToObject<JsonElement>(new
                {
                    kind = "search",
                    queryText = "test-query",
                    queryOptions = new
                    {
                        skip = 0,
                        top = 15
                    },
                    dataset = "test-dataset"
                }),
                Recipient = new("test-id"),
                Conversation = new() { Id = "conversationId" },
                From = new() { Id = "fromId" },
                ChannelId = "channelId",
            });
            IList<AdaptiveCardsSearchResult> searchResults = new List<AdaptiveCardsSearchResult>
            {
                new("test-title", "test-value")
            };
            var turnState = TurnStateConfig.GetTurnStateWithConversationStateAsync(turnContext);

            var app = new Application(new()
            {
                StartTypingTimer = false,
                TurnStateFactory = () => turnState.Result,
            });
            SearchHandlerAsync handler = (turnContext, turnState, query, cancellationToken) =>
            {
                Assert.Equal("test-query", query.Parameters.QueryText);
                Assert.Equal("test-dataset", query.Parameters.Dataset);
                return Task.FromResult(searchResults);
            };

            // Act
            app.AdaptiveCards.OnSearch("not-test-dataset", handler);
            await app.OnTurnAsync(turnContext);

            // Assert
            Assert.Null(activitiesToSend);
        }

        [Fact]
        public async Task Test_OnSearch_RouteSelector_ActivityNotMatched()
        {
            // Arrange
            var adapter = new SimpleAdapter();
            var turnContext = new TurnContext(adapter, new Activity()
            {
                Type = ActivityTypes.Invoke,
                Name = "adaptiveCard/action",
                Recipient = new("test-id"),
                Conversation = new() { Id = "conversationId" },
                From = new() { Id = "fromId" },
                ChannelId = "channelId",
            });
            IList<AdaptiveCardsSearchResult> searchResults = new List<AdaptiveCardsSearchResult>
            {
                new("test-title", "test-value")
            };
            var turnState = TurnStateConfig.GetTurnStateWithConversationStateAsync(turnContext);

            var app = new Application(new()
            {
                StartTypingTimer = false,
                TurnStateFactory = () => turnState.Result,
            });
            RouteSelectorAsync routeSelector = (turnContext, cancellationToken) =>
            {
                return Task.FromResult(true);
            };
            SearchHandlerAsync handler = (turnContext, turnState, query, cancellationToken) =>
            {
                Assert.Equal("test-query", query.Parameters.QueryText);
                Assert.Equal("test-dataset", query.Parameters.Dataset);
                return Task.FromResult(searchResults);
            };

            // Act
            app.AdaptiveCards.OnSearch(routeSelector, handler);
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await app.OnTurnAsync(turnContext));

            // Assert
            Assert.Equal("Unexpected AdaptiveCards.OnSearch() triggered for activity type: invoke", exception.Message);
        }

        private static T Cast<T>(object data)
        {
            T result = ProtocolJsonSerializer.ToObject<T>(data);
            Assert.NotNull(result);
            return result;
        }

        private sealed class TestAdaptiveCardActionData
        {
            public string TestKey { get; set; }
        }

        private sealed class TestAdaptiveCardSubmitData
        {
            public string Verb { get; set; }

            public string TestKey { get; set; }
        }
    }
}
