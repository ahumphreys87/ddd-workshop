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
    public class EventJournalTest
    {
        [Fact]
        public void TestEventJournalOpenClose()
        {
            EventJournal journal = EventJournal.Open("test");

            Assert.NotNull(journal);
            Assert.Equal("test", journal.Name);

            EventJournal journalPreOpened = EventJournal.Open("test");
            Assert.Equal(journal, journalPreOpened);

            journal.Close();
        }

        [Fact]
        public void TestOpenEventJournalReader()
        {
            EventJournal journal = EventJournal.Open("test");
            EventJournalReader reader = journal.Reader("test_reader");
            Assert.NotNull(reader);
            Assert.Equal("test_reader", reader.Name);
            EventJournalReader readerPreOpened = journal.Reader("test_reader");
            Assert.Equal(reader, readerPreOpened);

            journal.Close();
        }

        [Fact]
        public void TestWriteRead()
        {
            EventJournal journal = EventJournal.Open("test");
			journal.Write("name123", 1, EventBatch.Of("type1", "type1_instance1"));
			journal.Write("name456", 1, EventBatch.Of("type2", "type2_instance1"));
			EventJournalReader reader = journal.Reader("test_reader");
            Assert.Equal(new StoredEvent(0, new EventValue("name123", 1, "type1", "type1_instance1", "")), reader.ReadNext());
            reader.Acknowledge(0);
            Assert.Equal(new StoredEvent(1, new EventValue("name456", 1, "type2", "type2_instance1", "")), reader.ReadNext());
            reader.Acknowledge(1);
            Assert.Equal(new StoredEvent(-1, new EventValue("", -1, "", "", "")), reader.ReadNext());

            journal.Close();
        }

        [Fact]
        public void TestWriteReadStream()
        {
			EventJournal journal = EventJournal.Open("test");
			journal.Write("name123", 1, EventBatch.Of("type1", "type1_instance1"));
			journal.Write("name456", 1, EventBatch.Of("type2", "type2_instance1"));
			journal.Write("name123", 2, EventBatch.Of("type1-1", "type1-1_instance1"));
			journal.Write("name123", 3, EventBatch.Of("type1-2", "type1-2_instance1"));
			journal.Write("name456", 2, EventBatch.Of("type2-1", "type2-1_instance1"));

			EventStreamReader streamReader = journal.StreamReader();

            EventStream eventStream123 = streamReader.StreamFor("name123");
            Assert.Equal(3, eventStream123.StreamVersion);
            Assert.Equal(3, eventStream123.Stream.Count);
            Assert.Equal(new EventValue("name123", 1, "type1", "type1_instance1", ""), eventStream123.Stream[0]);
            Assert.Equal(new EventValue("name123", 2, "type1-1", "type1-1_instance1", ""), eventStream123.Stream[1]);
            Assert.Equal(new EventValue("name123", 3, "type1-2", "type1-2_instance1", ""), eventStream123.Stream[2]);

            EventStream eventStream456 = streamReader.StreamFor("name456");
            Assert.Equal(2, eventStream456.StreamVersion);
            Assert.Equal(2, eventStream456.Stream.Count);
            Assert.Equal(new EventValue("name456", 1, "type2", "type2_instance1", ""), eventStream456.Stream[0]);
            Assert.Equal(new EventValue("name456", 2, "type2-1", "type2-1_instance1", ""), eventStream456.Stream[1]);

            journal.Close();
        }

        [Fact]
        public void TestWriteReadStreamSnapshot()
        {
            EventJournal journal = EventJournal.Open("test");
			journal.Write("name123", 1, EventBatch.Of("type1", "type1_instance1", "SNAPSHOT123-1"));
			journal.Write("name456", 1, EventBatch.Of("type2", "type2_instance1", "SNAPSHOT456-1"));
			journal.Write("name123", 2, EventBatch.Of("type1-1", "type1-1_instance1", "SNAPSHOT123-2"));
			journal.Write("name123", 3, EventBatch.Of("type1-2", "type1-2_instance1"));
			journal.Write("name456", 2, EventBatch.Of("type2-1", "type2-1_instance1", "SNAPSHOT456-2"));

			EventStreamReader streamReader = journal.StreamReader();

            EventStream eventStream123 = streamReader.StreamFor("name123");
            Assert.Equal("name123", eventStream123.StreamName);
            Assert.Equal(3, eventStream123.StreamVersion);
            Assert.Equal(2, eventStream123.Stream.Count);
            Assert.Equal("SNAPSHOT123-2", eventStream123.Snapshot);
            Assert.Equal(new EventValue("name123", 2, "type1-1", "type1-1_instance1", "SNAPSHOT123-2"), eventStream123.Stream[0]);
            Assert.Equal(new EventValue("name123", 3, "type1-2", "type1-2_instance1", ""), eventStream123.Stream[1]);

            EventStream eventStream456 = streamReader.StreamFor("name456");
            Assert.Equal("name456", eventStream456.StreamName);
            Assert.Equal(2, eventStream456.StreamVersion);
            Assert.Equal(1, eventStream456.Stream.Count);
            Assert.Equal("SNAPSHOT456-2", eventStream456.Snapshot);
            Assert.Equal(new EventValue("name456", 2, "type2-1", "type2-1_instance1", "SNAPSHOT456-2"), eventStream456.Stream[0]);

            journal.Close();
        }
    }
}
