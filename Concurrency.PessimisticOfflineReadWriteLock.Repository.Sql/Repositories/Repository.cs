using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Domain;
using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.UnitOfWork;
using Concurrency.PessimisticOfflineReadWriteLock.Repository.Sql.Mapper;

namespace Concurrency.PessimisticOfflineReadWriteLock.Repository.Sql.Repositories
{
	public abstract class Repository<T> : IRepository<T>, IUnitOfWorkRepository where T : EntityBase, IAggregateRoot
	{
		private IUnitOfWork uow;
		private BaseMapper mapper;

		public Repository(IUnitOfWork uow)
		{
			Debug.Assert(uow != null);
			this.uow = uow;
		}

		protected BaseMapper Mapper
		{
			get
			{
				if (this.mapper == null)
					this.mapper = CreateMapper();
				return this.mapper;
			}
		}

		public T FindBy(Guid id)
		{
			return (T)Mapper.Find(id);
		}

		public void Save(T entity)
		{
			this.uow.RegisterAmended(entity, this);
		}

		protected abstract BaseMapper CreateMapper();

		protected virtual void PersistUpdate(T entity)
		{
			this.Mapper.Update(entity);
		}

		#region IUnitOfWorkRepository implementation

		void IUnitOfWorkRepository.PersistUpdateOf(IAggregateRoot entity)
		{
			PersistUpdate((T)entity);
		}

		#endregion
	}
}
