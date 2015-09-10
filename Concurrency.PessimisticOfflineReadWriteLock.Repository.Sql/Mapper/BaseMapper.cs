using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Domain;
using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Locking;
using Concurrency.PessimisticOfflineReadWriteLock.Manager;

namespace Concurrency.PessimisticOfflineReadWriteLock.Repository.Sql.Mapper
{
	public abstract class BaseMapper
	{
		protected abstract string Table { get; }

		public EntityBase Find(Guid id)
		{
			var session = ApplicationManager.INSTANCE.GetSessionManager().GetCurrent();
			Debug.Assert(session != null);
			var entity = session.GetMap().Get(id);
			if (entity == null)
			{
				try
				{
					var db = ApplicationManager.INSTANCE.GetDbProvider();
					var connection = db.CreateConnection();
					connection.Open();
					try
					{
						using (var command = connection.CreateCommand())
						{
							command.CommandType = System.Data.CommandType.Text;
							command.CommandText = GetLoadSQL(id);
							var reader = command.ExecuteReader();
							if (reader.Read())
							{
								entity = Load(id, reader);
								session.GetMap().Add(id, entity);
							}
						}
					}
					finally
					{
						connection.Close();
					}
				}
				catch (DbException dbe)
				{
					throw new Exception("unexpected error finding " + Table + ": " + id + "\n" + dbe.Message);
				}
			}
			return entity;
		}

		public void Update(EntityBase entity)
		{
			var session = ApplicationManager.INSTANCE.GetSessionManager().GetCurrent();
			Debug.Assert(session != null);
			try
			{
				var connection = Connections.INSTANCE.GetConnection(session.Id);
				using (var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = GetUpdateSQL(entity);
					command.Transaction = Connections.INSTANCE.GetTransaction(session.Id);
					command.ExecuteNonQuery();
				}
			}
			catch (DbException dbe)
			{
				throw new Exception("Unexpected error updating: " + dbe.Message);
			}
		}

		protected abstract EntityBase Load(Guid id, IDataReader reader);

		protected abstract string GetLoadSQL(Guid id);
		protected abstract string GetUpdateSQL(EntityBase entity);
	}
}
