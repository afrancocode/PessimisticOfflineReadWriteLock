using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Data;

namespace Concurrency.PessimisticOfflineReadWriteLock.AppConsole
{
	public sealed class SqlProvider : IDbProvider
	{
		private static readonly string CONNECTION_STRING = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=D:\GitHub\PessimisticOfflineReadWriteLock\Concurrency.PessimisticOfflineReadWriteLock.AppConsole\Db\RWPessimistic.mdf;Integrated Security=True";

		public IDbConnection CreateConnection()
		{
			return new SqlConnection(CONNECTION_STRING);
		}

		public IDbTransaction BeginSystemTransaction(IDbConnection connection, IsolationLevel isolation = IsolationLevel.ReadUncommitted)
		{
			return connection.BeginTransaction(isolation);
		}
	}
}
