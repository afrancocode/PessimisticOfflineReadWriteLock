using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.UnitOfWork;
using Concurrency.PessimisticOfflineReadWriteLock.Model;
using Concurrency.PessimisticOfflineReadWriteLock.Repository.Sql.Mapper;

namespace Concurrency.PessimisticOfflineReadWriteLock.Repository.Sql.Repositories
{
	public sealed class CustomerRepository : Repository<Customer>, ICustomerRepository
	{
		public CustomerRepository(IUnitOfWork uow)
			: base(uow)
		{ }

		protected override BaseMapper CreateMapper()
		{
			return new CustomerMapper();
		}
	}
}
