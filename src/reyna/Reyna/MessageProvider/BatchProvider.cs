namespace Reyna
{
    using System;
    using System.Net;
    using Interfaces;

    internal class BatchProvider : IMessageProvider
    {
        private const string PeriodicBackoutCheckTAG = "BatchProvider";

        public BatchProvider(IRepository repository, IPeriodicBackoutCheck periodicBackoutCheck, IBatchConfiguration batchConfiguration)
        {
            Repository = repository;
            BatchConfiguration = batchConfiguration;
            PeriodicBackoutCheck = periodicBackoutCheck;
        }

        public bool CanSend
        {
            get
            {
                long interval = (long)(BatchConfiguration.SubmitInterval * 0.9);
                if (PeriodicBackoutCheck.IsTimeElapsed(PeriodicBackoutCheckTAG, interval))
                {
                    return true;
                }

                return Repository.AvailableMessagesCount >= BatchConfiguration.BatchMessageCount;
            }
        }

        internal IBatchConfiguration BatchConfiguration { get; set; }

        internal IPeriodicBackoutCheck PeriodicBackoutCheck { get; set; }

        private IRepository Repository { get; set; }

        private bool BatchDeleted { get; set; }

        public IMessage GetNext()
        {
            var message = Repository.Get();
            var batch = new Batch();

            WebHeaderCollection headers = null;
            var count = 0;
            long size = 0;
            long maxMessagesCount = BatchConfiguration.BatchMessageCount;
            long maxBatchSize = BatchConfiguration.BatchMessagesSize;
            while (message != null && count < maxMessagesCount && size < maxBatchSize)
            {
                headers = message.Headers;
                batch.Add(message);
                
                size = GetSize(batch);
                count++;
                message = Repository.GetNextMessageAfter(message.Id);
            }

            if (size > maxBatchSize)
            {
                batch.RemoveLastMessage();
            }

            if (batch.Events.Count > 0)
            {
                var lastMessage = batch.Events[batch.Events.Count - 1];
                var batchMessage = new Message(GetBatchUploadUrl(lastMessage.Url), batch.ToJson());
                batchMessage.Id = lastMessage.ReynaId;
                for (int index = 0; index < headers.Count; index++)
                {
                    batchMessage.Headers.Add(headers.Keys[index], headers[index]);
                }

                return batchMessage;
            }

            return null;
        }

        public void Delete(IMessage message)
        {
            Repository.DeleteMessagesFrom(message);
            BatchDeleted = true;
        }

        public void Close()
        {
            if (BatchDeleted)
            {
                PeriodicBackoutCheck.Record(PeriodicBackoutCheckTAG);
            }

            BatchDeleted = false;
        }

        private Uri GetBatchUploadUrl(Uri uri)
        {
            if (BatchConfiguration.BatchUrl != null)
            {
                return BatchConfiguration.BatchUrl;
            }

            return GetUploadUrlFromMessageUrl(uri);
        }

        private Uri GetUploadUrlFromMessageUrl(Uri uri)
        {
            var path = uri.AbsoluteUri;
            int index = path.LastIndexOf("/");
            var batchPath = path.Substring(0, index) + "/batch";
            return new Uri(batchPath);
        }

        private long GetSize(Batch batch)
        {
            return System.Text.Encoding.UTF8.GetByteCount(batch.ToJson());
        }
    }
}
