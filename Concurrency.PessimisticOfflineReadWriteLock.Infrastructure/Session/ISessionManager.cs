using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Data;
using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Map;

namespace Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Session
{
	public interface ISessionManager
	{
		ISession Create();
		ISession GetCurrent();
		void SetCurrent(ISession session);
		void Close(ISession session);
	}
}
