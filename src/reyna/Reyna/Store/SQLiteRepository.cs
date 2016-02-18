namespace Reyna
{
    using System;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.IO;
    using System.Reflection;
    using Interfaces;

    internal sealed class SQLiteRepository : IRepository
    {
        private const string CreateTableSql = "CREATE TABLE Message (id INTEGER PRIMARY KEY AUTOINCREMENT, url TEXT, body TEXT);CREATE TABLE Header (id INTEGER PRIMARY KEY AUTOINCREMENT, messageid INTEGER, key TEXT, value TEXT, FOREIGN KEY(messageid) REFERENCES message(id));";
        private const string InsertMessageSql = "INSERT INTO Message(url, body) VALUES(@url, @body); SELECT last_insert_rowid();";
        private const string InsertHeaderSql = "INSERT INTO Header(messageid, key, value) VALUES(@messageId, @key, @value);";
        private const string DeleteMessageSql = "DELETE FROM Header WHERE messageid = @messageId;DELETE FROM Message WHERE id = @messageId";
        private const string DeleteMessagesFromSql = "DELETE FROM Header WHERE messageid <= @messageId;DELETE FROM Message WHERE id <= @messageId";
        private const string SelectTop1MessageSql = "SELECT id, url, body FROM Message ORDER BY id ASC LIMIT 1";
        private const string SelectTop1MessageSqlFrom = "SELECT id, url, body FROM Message WHERE id > @messageId ORDER BY id ASC LIMIT 1";
        private const string SelectHeaderSql = "SELECT key, value FROM Header WHERE messageid = @messageId";
        private const string SelectMinIdWithTypeSql = "SELECT min(id) FROM Message WHERE url = @type";
        private const string SelectNumberOfMessagesSql = "SELECT count(*) from Message";
        private const string SelectMessageIdWithOffsetSql = "SELECT id FROM Message LIMIT 1 OFFSET @offset";
        private const string DeleteMessagesToIdSql = "DELETE FROM Message WHERE id < @id";
        private const string DeleteHeadersToMessageIdSql = "DELETE FROM Header WHERE messageid < @id";

        private static readonly object Locker = new object();

        public SQLiteRepository()
        {
            SizeDifferenceToStartCleaning = 307200; ////300Kb in bytes
        }

        public SQLiteRepository(byte[] password)
        {
            Password = password;
            SizeDifferenceToStartCleaning = 307200; ////300Kb in bytes
        }

        private delegate IMessage ExecuteFunctionInTransaction(DbTransaction transaction);

        private delegate void ExecuteActionInTransaction(IMessage message, DbTransaction transaction);

        public event EventHandler<EventArgs> MessageAdded;

        public long AvailableMessagesCount {
            get
            {
                long count = 0;
                Execute(connection =>
                {
                    count = GetNumberOfMessages(connection);
                });

                return count;
            }
        }

        public IMessage GetNextMessageAfter(long messageId)
        {
            return ExecuteInTransaction(t =>
            {
                IMessage message;
                var messageIdParameter = CreateParameter("@messageId", messageId);
                using (var reader = ExecuteReader(SelectTop1MessageSqlFrom, t, messageIdParameter))
                {
                    reader.Read();
                    message = CreateFromDataReader(reader);
                }

                if (message == null)
                {
                    return null;
                }

                FillHeaders(message, t);

                return message;
            });
        }

        public void DeleteMessagesFrom(IMessage message)
        {
            if (message == null)
            {
                return;
            }

            var messageId = this.CreateParameter("@messageId", message.Id);
            this.ExecuteInTransaction((t) => this.ExecuteNonQuery(SQLiteRepository.DeleteMessagesFromSql, t, messageId));
        }

        internal long SizeDifferenceToStartCleaning { get; set; }

        internal bool Exists
        {
            get
            {
                FileInfo fileInfo = new FileInfo(DatabasePath);
                return File.Exists(DatabasePath) && fileInfo.Length >= 4096;
            }
        }

        public byte[] Password { get; set; }

        public string DatabasePath
        {
            get
            {
                var assemblyFile = new FileInfo(Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName);
                return Path.Combine(assemblyFile.DirectoryName, "reyna.db");
            }
        }

        public void Initialise()
        {
            lock (Locker)
            {
                if (Exists)
                {
                    return;
                }

                Create();
            }
        }

        public void Add(IMessage message)
        {
            lock (Locker)
            {
                ExecuteInTransaction(t => InsertMessage(t, message));
            }
        }

        public void Add(IMessage message, long storageSizeLimit)
        {
            lock (Locker)
            {
                ExecuteInTransaction(t =>
                {
                    long dbSize = GetDbSize(t);

                    if (DbSizeApproachesLimit(dbSize, storageSizeLimit))
                    {
                        ClearOldRecords(t, message);
                    }

                    InsertMessage(t, message);
                });
            }
        }

        public IMessage Get()
        {
            return GetFirstInQueue();
        }

        public IMessage Remove()
        {
            return GetFirstInQueue((message, t) =>
            {
                var sql = DeleteMessageSql;
                var messageId = CreateParameter("@messageId", message.Id);
                ExecuteNonQuery(sql, t, messageId);
            });
        }

        public void ShrinkDb(long limit)
        {
            lock (Locker)
            {
                Execute(connection =>
                {
                    limit -= SizeDifferenceToStartCleaning;
                    long size = GetDbSize(connection);

                    if (size <= limit)
                    {
                        return;
                    }

                    do
                    {
                        Shrink(connection, limit, size);
                        Vacuum(connection);
                        size = GetDbSize(connection);
                    }
                    while (size > limit);
                });
            }
        }

        internal void Create()
        {
            SQLiteConnection.CreateFile(DatabasePath);

            ExecuteInTransaction(t =>
            {
                var sql = CreateTableSql;
                ExecuteNonQuery(sql, t);
            });
        }

        private void InsertMessage(DbTransaction transaction, IMessage message)
        {
            var sql = InsertMessageSql;
            var url = CreateParameter("@url", message.Url);
            var body = CreateParameter("@body", message.Body);
            var id = Convert.ToInt32(ExecuteScalar(sql, transaction, url, body));

            sql = InsertHeaderSql;
            var messageId = CreateParameter("@messageId", id);

            foreach (string headerKey in message.Headers.Keys)
            {
                var key = CreateParameter("@key", headerKey);
                var value = CreateParameter("@value", message.Headers[headerKey]);
                ExecuteScalar(sql, transaction, messageId, key, value);
            }

            if (MessageAdded != null)
            {
                MessageAdded.Invoke(this, EventArgs.Empty);
            }
        }

        private long GetDbSize(DbTransaction transaction)
        {
            return (long)ExecuteScalar("pragma page_size", transaction) * (long)ExecuteScalar("pragma page_count", transaction);
        }

        private long GetDbSize(DbConnection connection)
        {
            return (long)ExecuteScalar("pragma page_size", connection) * (long)ExecuteScalar("pragma page_count", connection);
        }

        private bool DbSizeApproachesLimit(long size, long limit)
        {
            return (limit > size) && (limit - size) < SizeDifferenceToStartCleaning;
        }

        private void ClearOldRecords(DbTransaction transaction, IMessage message)
        {
            long? oldestMessageId = FindOldestMessageIdWithType(transaction, message.Url);

            if (oldestMessageId.HasValue)
            {
                RemoveExistingMessage(transaction, oldestMessageId.Value);
            }
        }

        private void RemoveExistingMessage(DbTransaction transaction, long messageId)
        {
            var messageParameter = CreateParameter("@messageid", messageId);
            ExecuteNonQuery(DeleteMessageSql, transaction, messageParameter);
        }

        private long? FindOldestMessageIdWithType(DbTransaction transaction, Uri type)
        {
            var typeParameter = CreateParameter("@type", type);
            object result = ExecuteScalar(SelectMinIdWithTypeSql, transaction, typeParameter);

            if (result is DBNull)
            {
                return null;
            }

            return (long)result;
        }

        private void Shrink(DbConnection connection, long sizeLimit, long size)
        {
            double limitPercentage = 1 - ((double)sizeLimit / size);
            long numberOfMessages = GetNumberOfMessages(connection);
            long numberOfMessagesToRemove = (long)Math.Round(numberOfMessages * limitPercentage);
            numberOfMessagesToRemove = numberOfMessagesToRemove == 0 ? 1 : numberOfMessagesToRemove;

            long thresholdId = GetMessageIdToWhichShrink(connection, numberOfMessagesToRemove);

            ExecuteInTransaction(t =>
            {
                RemoveFromHeadersToMessageId(t, thresholdId);
                RemoveFromMessagesToId(t, thresholdId);
            });
        }

        private long GetNumberOfMessages(DbConnection connection)
        {
            return (long)ExecuteScalar(SelectNumberOfMessagesSql, connection);
        }

        private long GetMessageIdToWhichShrink(DbConnection connection, long numberOfMessagesToRemove)
        {
            var offsetParameter = CreateParameter("@offset", numberOfMessagesToRemove);
            return (long)ExecuteScalar(SelectMessageIdWithOffsetSql, connection, offsetParameter);
        }

        private void RemoveFromMessagesToId(DbTransaction transaction, long thresholdId)
        {
            var id = CreateParameter("@id", thresholdId);
            ExecuteNonQuery(DeleteMessagesToIdSql, transaction, id);
        }

        private void RemoveFromHeadersToMessageId(DbTransaction transaction, long thresholdId)
        {
            var id = CreateParameter("@id", thresholdId);
            ExecuteNonQuery(DeleteHeadersToMessageIdSql, transaction, id);
        }

        private void Vacuum(DbConnection connection)
        {
            ExecuteNonQuery("vacuum", connection);
        }

        private DbConnection CreateConnection()
        {
            var connectionString = string.Format("Data Source={0}", DatabasePath);
            var connection = new SQLiteConnection(connectionString);

            if (Password != null && Password.Length > 0)
            {
                connection.SetPassword(Password);
            }

            connection.Open();
            return connection;
        }

        private DbParameter CreateParameter(string parameterName, object value)
        {
            return new SQLiteParameter(parameterName, value);
        }

        private DbCommand CreateCommand(string sql, DbTransaction transaction, params DbParameter[] parameters)
        {
            var command = transaction.Connection.CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction;
            command.Connection = transaction.Connection;

            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }

        private DbCommand CreateCommand(string sql, DbConnection connection, params DbParameter[] parameters)
        {
            var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Connection = connection;

            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }

        private int ExecuteNonQuery(string sql, DbTransaction transaction, params DbParameter[] parameters)
        {
            using (var command = CreateCommand(sql, transaction, parameters))
            {
                return command.ExecuteNonQuery();
            }
        }

        private int ExecuteNonQuery(string sql, DbConnection connection, params DbParameter[] parameters)
        {
            using (var command = CreateCommand(sql, connection, parameters))
            {
                return command.ExecuteNonQuery();
            }
        }

        private object ExecuteScalar(string sql, DbTransaction transaction, params DbParameter[] parameters)
        {
            using (var command = CreateCommand(sql, transaction, parameters))
            {
                return command.ExecuteScalar();
            }
        }

        private object ExecuteScalar(string sql, DbConnection connection, params DbParameter[] parameters)
        {
            using (var command = CreateCommand(sql, connection, parameters))
            {
                return command.ExecuteScalar();
            }
        }

        private DbDataReader ExecuteReader(string sql, DbTransaction transaction, params DbParameter[] parameters)
        {
            using (var command = CreateCommand(sql, transaction, parameters))
            {
                return command.ExecuteReader();
            }
        }

        private IMessage ExecuteInTransaction(ExecuteFunctionInTransaction func)
        {
            using (var connection = CreateConnection())
            using (var transaction = connection.BeginTransaction())
            {
                var message = func(transaction);
                transaction.Commit();
                return message;
            }
        }

        private void ExecuteInTransaction(Action<DbTransaction> action)
        {
            using (var connection = CreateConnection())
            using (var transaction = connection.BeginTransaction())
            {
                action(transaction);
                transaction.Commit();
            }
        }

        private void Execute(Action<DbConnection> action)
        {
            using (var connection = CreateConnection())
            {
                action(connection);
            }
        }

        private IMessage GetFirstInQueue(params ExecuteActionInTransaction[] postActions)
        {
            return ExecuteInTransaction(t =>
            {
                IMessage message = null;

                var sql = SelectTop1MessageSql;
                using (var reader = ExecuteReader(sql, t))
                {
                    reader.Read();
                    message = CreateFromDataReader(reader);
                }

                if (message == null)
                {
                    return null;
                }

                sql = SelectHeaderSql;
                var messageId = CreateParameter("@messageId", message.Id);
                using (var reader = ExecuteReader(sql, t, messageId))
                {
                    while (reader.Read())
                    {
                        AddHeader(message, reader);
                    }

                    AddReynaHeader(message);
                }

                foreach (var postAction in postActions)
                {
                    postAction(message, t);
                }

                return message;
            });
        }

        private IMessage CreateFromDataReader(DbDataReader reader)
        {
            if (!reader.HasRows)
            {
                return null;
            }

            var id = Convert.ToInt32(reader["id"]);
            var url = new Uri(reader["url"] as string);
            var body = reader["body"] as string;

            var message = new Message(url, body)
            {
                Id = id
            };

            return message;
        }

        private void FillHeaders(IMessage message, DbTransaction t)
        {
            var messageId = CreateParameter("@messageId", message.Id);
            using (var reader = ExecuteReader(SelectHeaderSql, t, messageId))
            {
                while (reader.Read())
                {
                    AddHeader(message, reader);
                }

                AddReynaHeader(message);
            }
        }

        private void AddHeader(IMessage message, DbDataReader reader)
        {
            var key = reader["key"] as string;
            var value = reader["value"] as string;
            message.Headers.Add(key, value);
        }

        private void AddReynaHeader(IMessage message)
        {
            message.Headers.Add("reyna-id", message.Id.ToString());
        }
    }
}
