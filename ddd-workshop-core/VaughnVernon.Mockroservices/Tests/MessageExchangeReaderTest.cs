//   Copyright © 2017 Vaughn Vernon. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using Xunit;

namespace VaughnVernon.Mockroservices
{
    public class MessageExchangeReaderTest
    {
        [Fact]
        public void TestExchangeRoundTrip()
        {
            TestableDomainEvent domainEvent = new TestableDomainEvent(1, "something");
            string serializedDomainEvent = Serialization.Serialize(domainEvent);
            Message eventMessage = new Message(Convert.ToString(domainEvent.Id), domainEvent.GetType().Name, serializedDomainEvent);
            MessageExchangeReader reader = MessageExchangeReader.From(eventMessage);
            Assert.Equal(eventMessage.Id, reader.Id);
            Assert.Equal(eventMessage.Type, reader.Type);
            Assert.Equal(domainEvent.Id, reader.IdAsLong);
            Assert.Equal(domainEvent.Name, reader.PayloadStringValue("name"));
            Assert.Equal(domainEvent.EventVersion, reader.PayloadIntegerValue("eventVersion"));
            Assert.Equal(domainEvent.OccurredOn, reader.PayloadLongValue("occurredOn"));
        }
    }
}
