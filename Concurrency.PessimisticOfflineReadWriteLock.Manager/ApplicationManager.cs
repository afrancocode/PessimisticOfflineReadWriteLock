using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure;
using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Data;
using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Locking;
using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Session;
using Concurrency.PessimisticOfflineReadWriteLock.Manager.Sessions;

namespace Concurrency.PessimisticOfflineReadWriteLock.Manager
{
	public sealed class ApplicationManager : IApplicationManager
	{
		private static readonly IApplicationManager instance = new ApplicationManager();

		public static IApplicationManager INSTANCE
		{
			get { return instance; }
		}

		private ISessionManager sessionManager;
		private IReadWriteLockManager lockManager;
		private IDbProvider dbProvider;

		private ApplicationManager()
		{
			this.sessionManager = new SessionManager();
		}

		public void Inject(IDbProvider dbProvider, IReadWriteLockManager lockManager)
		{
			Debug.Assert(dbProvider != null && lockManager != null);
			if (this.dbProvider != null || this.lockManager != null)
				throw new InvalidOperationException();
			this.dbProvider = dbProvider;
			this.lockManager = lockManager;
		}

		public ISessionManager GetSessionManager()
		{
			return this.sessionManager;
		}

		public IReadWriteLockManager GetLockManager()
		{
			Debug.Assert(this.lockManager != null);
			return this.lockManager;
		}

		public IDbProvider GetDbProvider()
		{
			Debug.Assert(this.dbProvider != null);
			return dbProvider;
		}

		public void Close()
		{
			// Clear Locks
			// Clear Sessions
			// Close Connections
			throw new NotImplementedException();
		}
	}
}
