using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Domain;
using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Locking;
using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Session;
using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.UnitOfWork;
using Concurrency.PessimisticOfflineReadWriteLock.Manager;
using Concurrency.PessimisticOfflineReadWriteLock.Model;
using Concurrency.PessimisticOfflineReadWriteLock.Repository.Sql.Repositories;
using Concurrency.PessimisticOfflineReadWriteLock.Repository.Sql.UnitOfWork;

namespace Concurrency.PessimisticOfflineReadWriteLock.AppConsole
{
	public sealed class TestInfo
	{
		public TestInfo(Guid entityId)
		{
			this.EntityId = entityId;
			this.Session = ApplicationManager.INSTANCE.GetSessionManager().Create();
			this.UnitOfWork = new UnitOfWork();
			this.Repository = new CustomerRepository(this.UnitOfWork);
		}

		public Guid EntityId { get; private set; }
		public ISession Session { get; private set; }
		public Customer Entity { get; private set; }

		private ICustomerRepository repository;
		public ICustomerRepository Repository 
		{
			get
			{
				if (this.repository == null)
					this.repository = new CustomerRepository(this.UnitOfWork);
				return this.repository;
			}
			private set
			{
				this.repository = value;
			}
		}

		public IUnitOfWork UnitOfWork { get; private set; }

		public void LoadEntity()
		{
			SetCurrentSession();
			try
			{
				AdcquireLock(LockType.Read);
				this.Entity = this.Repository.FindBy(this.EntityId);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		public void EditEntity(string name)
		{
			SetCurrentSession();
			try
			{
				AdcquireLock(LockType.Write); // Try to get write lock
				this.Entity.Name = name;
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		public void SaveEntity()
		{
			SetCurrentSession();
			try
			{
				AdcquireLock(LockType.Write); // Try to get a write lock that maybe already has
				this.Repository.Save(this.Entity);
				this.UnitOfWork.Commit();
				ReleaseLock(LockType.Write); // Release write after all the commit was completed
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		public void CloseEntity()
		{
			SetCurrentSession();
			try
			{
				ReleaseLock(LockType.Read);
				this.Session.GetMap().Remove(this.EntityId);
				this.Entity = null;
				this.repository = null;
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		public void Close()
		{
			var session = this.Session;
			this.Session = null;
			session.Close();
		}

		private void SetCurrentSession()
		{
			ApplicationManager.INSTANCE.GetSessionManager().SetCurrent(this.Session);
		}

		private void AdcquireLock(LockType type)
		{
			ApplicationManager.INSTANCE.GetLockManager().AdcquireLock(this.EntityId, this.Session.Id, type);
		}

		private void ReleaseLock(LockType type)
		{
			ApplicationManager.INSTANCE.GetLockManager().ReleaseLock(this.EntityId, this.Session.Id, type);
		}
	}
}
