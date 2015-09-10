using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Data;
using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Locking;
using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Session;

namespace Concurrency.PessimisticOfflineReadWriteLock.Infrastructure
{
	public interface IApplicationManager
	{
		ISessionManager GetSessionManager();
		IReadWriteLockManager GetLockManager();
		IDbProvider GetDbProvider();
		void Close();
	}
}
