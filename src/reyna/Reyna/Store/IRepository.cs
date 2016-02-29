namespace Reyna.Interfaces
{
    using System;

    public interface IRepository
    {
        event EventHandler<EventArgs> MessageAdded;

        long AvailableMessagesCount { get; }

        IMessage GetNextMessageAfter(long messageId);

        void DeleteMessagesFrom(IMessage message);

        void Initialise();

        void Add(IMessage message);

        void Add(IMessage message, long storageSizeLimit);

        IMessage Get();

        IMessage Remove();

        void ShrinkDb(long limit);

        byte[] Password { get; set;}
    }
}
