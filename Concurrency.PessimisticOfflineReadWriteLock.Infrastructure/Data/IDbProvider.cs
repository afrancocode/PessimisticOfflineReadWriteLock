using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Data
{
	public interface IDbProvider
	{
		IDbConnection CreateConnection();
		IDbTransaction BeginSystemTransaction(IDbConnection connection, IsolationLevel isolation = IsolationLevel.ReadUncommitted);
	}
}
