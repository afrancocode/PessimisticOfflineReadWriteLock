using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.PessimisticOfflineReadWriteLock.Repository.Sql
{
	public sealed class Connections
	{
		private static readonly Connections instance = new Connections();

		public static Connections INSTANCE
		{
			get { return instance; }
		}

		private Dictionary<Guid, ConnectionData> connections;

		private Connections()
		{
			this.connections = new Dictionary<Guid, ConnectionData>();
		}

		public void Register(IDbConnection connection, Guid id)
		{
			ConnectionData data = null;
			if (this.connections.TryGetValue(id, out data))
			{
				Debug.Assert(false, "Duplicated Id");
				throw new InvalidOperationException();
			}
			connections.Add(id, new ConnectionData { Connection = connection });
		}

		public void Register(IDbTransaction transaction, Guid id)
		{
			ConnectionData data = null;
			if(!this.connections.TryGetValue(id, out data))
			{
				Debug.Assert(false, "Invalid Id");
				throw new InvalidOperationException();
			}

			if(data.Transaction != null)
			{
				Debug.Assert(false, "Has current Transaction");
				throw new InvalidOperationException();
			}

			data.Transaction = transaction;
		}

		public IDbConnection GetConnection(Guid id)
		{
			ConnectionData data = null;
			if (!this.connections.TryGetValue(id, out data) || data.Connection == null)
			{
				Debug.Assert(false, "Unexisting connection");
				throw new InvalidOperationException();
			}
			return data.Connection;
		}

		public IDbTransaction GetTransaction(Guid id)
		{
			ConnectionData data = null;
			if(!this.connections.TryGetValue(id, out data) || data.Transaction == null)
			{
				Debug.Assert(false, "Unexisting Transaction");
				throw new InvalidOperationException();
			}
			return data.Transaction;
		}

		public void Remove(Guid id)
		{
			this.connections.Remove(id);
		}

		private sealed class ConnectionData
		{
			public IDbConnection Connection;
			public IDbTransaction Transaction;
		}
	}
}
