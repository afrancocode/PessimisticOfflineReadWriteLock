using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Session;

namespace Concurrency.PessimisticOfflineReadWriteLock.Manager.Sessions
{
	public sealed class SessionManager : ISessionManager
	{
		private static readonly object _lock = new object();
		private ISession current;
		private List<ISession> sessions;

		public SessionManager()
		{
			this.sessions = new List<ISession>();
		}

		public ISession Create()
		{
			var session = new Session();
			this.sessions.Add(session);
			return session;
		}

		public ISession GetCurrent()
		{
			lock(_lock)
				return this.current;
		}

		public void SetCurrent(ISession session)
		{
			lock (_lock)
				this.current = session;
		}

		public void Close(ISession session)
		{
			lock(_lock)
			{
				ApplicationManager.INSTANCE.GetLockManager().ReleaseAllLocks(session.Id);
				this.sessions.Remove(session);
				if (this.current == session)
					this.current = null;
			}
		}
	}
}
