using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Domain;
using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.UnitOfWork;
using Concurrency.PessimisticOfflineReadWriteLock.Manager;

namespace Concurrency.PessimisticOfflineReadWriteLock.Repository.Sql.UnitOfWork
{
	public class UnitOfWork : IUnitOfWork
	{
		private Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>> update;

		public UnitOfWork()
		{
			this.update = new Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>>();
		}

		public void RegisterAmended(IAggregateRoot entity, IUnitOfWorkRepository repository)
		{
			List<IAggregateRoot> items = null;
			if (!update.TryGetValue(repository, out items))
			{
				items = new List<IAggregateRoot>();
				update.Add(repository, items);
			}
			items.Add(entity);
		}

		public void Commit()
		{
			var manager = ApplicationManager.INSTANCE.GetSessionManager();
			Debug.Assert(manager != null);

			var session = manager.GetCurrent();
			Debug.Assert(session != null);

			var db = ApplicationManager.INSTANCE.GetDbProvider();

			var connection = db.CreateConnection();
			Connections.INSTANCE.Register(connection, session.Id);
			connection.Open();
			try
			{
				var transaction = db.BeginSystemTransaction(connection);
				Connections.INSTANCE.Register(transaction, session.Id);
				try
				{
					UpdateDirty();
					transaction.Commit();
				}
				catch
				{
					transaction.Rollback();
					throw;
				}
			}
			finally
			{
				connection.Close();
				Connections.INSTANCE.Remove(session.Id);
			}
		}

		private void UpdateDirty()
		{
			foreach (var updateInfo in this.update)
			{
				var persistTo = updateInfo.Key;
				while (updateInfo.Value.Any())
				{
					var entity = updateInfo.Value[0];
					persistTo.PersistUpdateOf(entity);
					updateInfo.Value.RemoveAt(0);
				}
			}
		}

		public static readonly IUnitOfWork Empty = new EmptyUnitOfWork();

		private sealed class EmptyUnitOfWork : IUnitOfWork
		{
			void IUnitOfWork.RegisterAmended(IAggregateRoot entity, IUnitOfWorkRepository repository) { }
			void IUnitOfWork.Commit() { }
		}
	}
}
