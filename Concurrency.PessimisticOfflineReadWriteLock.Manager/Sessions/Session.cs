using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Map;
using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Session;

namespace Concurrency.PessimisticOfflineReadWriteLock.Manager.Sessions
{
	public sealed class Session : ISession
	{
		private IdentityMap map;

		public Session()
		{
			this.Id = Guid.NewGuid();
			this.map = new IdentityMap();
		}

		public Guid Id { get; private set; }

		public IdentityMap GetMap()
		{
			return this.map;
		}

		public void Close()
		{
			ApplicationManager.INSTANCE.GetSessionManager().Close(this);
		}
	}
}
