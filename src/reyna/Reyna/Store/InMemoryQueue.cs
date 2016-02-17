namespace Reyna
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Interfaces;

    internal sealed class InMemoryQueue : IRepository
    {
        public byte[] Password{ get; set; }

        private Queue<IMessage> queue;

        public InMemoryQueue()
        {
            queue = new Queue<IMessage>();
        }

        public event EventHandler<EventArgs> MessageAdded;

        public long AvailableMessagesCount {
            get
            {
                return queue.Count;
            }
        }

        public IMessage GetNextMessageAfter(long messageId)
        {
            throw new NotImplementedException();
        }

        public void DeleteMessagesFrom(IMessage message)
        {
            throw new NotImplementedException();
        }

        private object SyncRoot
        {
            get
            {
                return ((ICollection)queue).SyncRoot;
            }
        }

        public void Initialise()
        {
        }

        public void Add(IMessage message)
        {
            lock (SyncRoot)
            {
                queue.Enqueue(message);
                FireMessageAdded();
            }
        }

        public void Add(IMessage message, long storageSizeLimit)
        {
            Add(message);
        }

        public IMessage Get()
        {
            lock (SyncRoot)
            {
                if (queue.Count == 0)
                {
                    return null;
                }

                return queue.Peek();
            }
        }

        public IMessage Remove()
        {
            lock (SyncRoot)
            {
                if (queue.Count == 0)
                {
                    return null;
                }

                return queue.Dequeue();
            }
        }

        private void FireMessageAdded()
        {
            if (MessageAdded == null)
            {
                return;
            }

            MessageAdded.Invoke(this, new EventArgs());
        }

        public void ShrinkDb(long limit) { }
    }
}
