using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Domain
{
	public interface IRepository<T> where T : IAggregateRoot
	{
		T FindBy(Guid id);
		void Save(T entity);
	}
}
