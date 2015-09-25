namespace Reyna.Interfaces
{
    using System;

    public interface IRepository
    {
        event EventHandler<EventArgs> MessageAdded;

        void Initialise();

        void Add(IMessage message);

        void Add(IMessage message, long storageSizeLimit);

        IMessage Get();

        IMessage Remove();

        void ShrinkDb(long limit);
    }
}
