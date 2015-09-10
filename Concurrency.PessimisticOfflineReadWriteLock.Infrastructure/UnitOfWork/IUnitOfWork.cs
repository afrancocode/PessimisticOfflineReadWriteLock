using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Domain;

namespace Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.UnitOfWork
{
	public interface IUnitOfWork
	{
		void RegisterAmended(IAggregateRoot entity, IUnitOfWorkRepository repository);
		void Commit();
	}
}
