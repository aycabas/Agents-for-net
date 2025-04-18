﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Agents.Core.Models;
using Microsoft.Agents.Core.Serialization;
using Xunit;

namespace Microsoft.Agents.Model.Tests
{
    public class PlaceTests
    {
        [Fact]
        public void PlaceInits()
        {
            var address = "123 Drury Lane, Muffin Man Village, Enchanted Forest 45678";
            var geo = new GeoCoordinates();
            var hasMap = "http://muffin-man-who-lives-on-drury-lane.com";
            var type = "Place";
            var name = "muffinMan";

            var place = new Place(address, geo, hasMap, type, name);

            Assert.NotNull(place);
            Assert.IsType<Place>(place);
            Assert.Equal(address, place.Address);
            Assert.Equal(geo, place.Geo);
            Assert.Equal(hasMap, place.HasMap);
            Assert.Equal(type, place.Type);
            Assert.Equal(name, place.Name);
        }

        [Fact]
        public void PlaceInitsWithNoArgs()
        {
            var place = new Place();

            Assert.NotNull(place);
            Assert.IsType<Place>(place);
        }

        [Fact]
        public void PlaceTypedDeserialize()
        {
            var json = "{\"entities\": [{\"type\": \"place\", \"name\": \"placename\"}]}";
            var activity = ProtocolJsonSerializer.ToObject<IActivity>(json);

            Assert.NotNull(activity.Entities);
            Assert.NotEmpty(activity.Entities);
            Assert.IsType<Place>(activity.Entities[0]);

            var place = activity.Entities[0] as Place;
            Assert.Equal("placename", place.Name);
        }
    }
}
