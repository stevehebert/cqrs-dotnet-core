using cqrs.Messaging;
using cqrs.Messaging.Handling;
using System;
using System.Collections.Generic;
using Xunit;

namespace cqrs_test.Messaging.Handling
{
    public class EventDispatcherTests
    {
        #region Simple Event Handlers

        public class TestEvent : IEvent
        {
            public TestEvent(Guid guid)
            {
                SourceId = guid;
            }
            public Guid SourceId
            {
                get;

                private set;
            }
        }

        public class TestEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : IEvent
        {
            public IList<IEvent> Events { get; private set; }

            public TestEventHandler(IList<IEvent> events)
            {
                Events = events;
            }

            public void Handle(TEvent @event)
            {
                Events.Add(@event);
            }
        }

        public class SaveEventHandler : TestEventHandler<SaveEvent>
        {
            public SaveEventHandler(IList<IEvent> events) : base(events) { }
        }

        public class LoadEventHandler : TestEventHandler<LoadEvent>
        {
            public LoadEventHandler(IList<IEvent> events) : base(events) { }
        }


        public class SaveEvent : TestEvent
        {
            public SaveEvent(Guid guid) : base(guid) {}
        }

       

        public class LoadEvent : TestEvent
        {
            public LoadEvent(Guid guid) : base(guid) { }
        }

        #endregion


        [Fact]
        public void send_event_to_a_registered_handler_works()
        {
            var eventDispatcher = new EventDispatcher();
            var list = new List<IEvent>();
            var guid = Guid.NewGuid();

            eventDispatcher.Register(new SaveEventHandler(list));

            eventDispatcher.DispatchMessage(new SaveEvent(guid));

            Assert.Equal(list[0].SourceId, guid);
        }

        [Fact]
        public void send_event_to_a_nonregistered_event_handler_does_not_trigger_the_message()
        {
            var eventDispatcher = new EventDispatcher();
            var list = new List<IEvent>();
            var guid = Guid.NewGuid();

            eventDispatcher.Register(new SaveEventHandler(list));

            eventDispatcher.DispatchMessage(new LoadEvent(guid));

            Assert.Equal(list.Count, 0);
        }

        [Fact]
        public void registering_and_triggering_enumerations_works()
        {
            var list = new List<IEvent>();
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var eventDispatcher = new EventDispatcher(new IEventHandler[] { new SaveEventHandler(list), new LoadEventHandler(list) });

            //eventDispatcher.DispatchMessage(new LoadEvent(guid1));
            eventDispatcher.DispatchMessages(new IEvent[] { new SaveEvent(guid1), new LoadEvent(guid2) });

            Assert.Equal(list.Count, 2);
            Assert.Equal(list[0].SourceId, guid1);
            Assert.Equal(list[1].SourceId, guid2);
        }

    }
}
