using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Locking
{
	public interface IReadWriteLockManager
	{
		void AdcquireLock(Guid lockable, Guid owner, LockType type);
		void ReleaseLock(Guid lockable, Guid owner, LockType type);
		void ReleaseAllLocks(Guid owner);
	}
}
