using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Domain;

namespace Concurrency.PessimisticOfflineReadWriteLock.Model
{
	public interface ICustomerRepository : IRepository<Customer>
	{
	}
}
