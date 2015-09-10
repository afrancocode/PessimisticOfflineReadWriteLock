using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Map;

namespace Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Session
{
	public interface ISession
	{
		Guid Id { get; }
		IdentityMap GetMap();
		void Close();
	}
}
