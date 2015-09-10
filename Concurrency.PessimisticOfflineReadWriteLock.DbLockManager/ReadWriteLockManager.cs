using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Data;
using Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Locking;
using Concurrency.PessimisticOfflineReadWriteLock.Manager;

namespace Concurrency.PessimisticOfflineReadWriteLock.DbLockManager
{
	public sealed class ReadWriteLockManager : IReadWriteLockManager
	{
		private static readonly string INSERT_SQL = "INSERT INTO Lock VALUES('{0}', '{1}', '{2}')";
		private static readonly string UPDATE_SQL = "UPDATE Lock SET LockType='{0}' WHERE LockableId='{1}' AND OwnerId='{2}'";
		private static readonly string DELETE_SINGLE_SQL = "DELETE FROM Lock WHERE LockableId='{0}' and OwnerId='{1}'";
		private static readonly string DELETE_ALL_SQL = "DELETE FROM Lock WHERE OwnerId='{0}'";
		private static readonly string CHECK_SQL = "SELECT LockableId, LockType FROM Lock WHERE LockableId='{0}' AND OwnerId='{1}'";
		private static readonly string CHECK_UPGRADE_SQL = "SELECT COUNT(*) AS Conflicts FROM Lock WHERE LockableId='{0}' AND OwnerId!='{1}' AND ( LockType < {2} OR LockType > {2})";

		public ReadWriteLockManager()
		{
		}

		public void AdcquireLock(Guid lockable, Guid owner, LockType type)
		{
			var lockInfo = GetLockInfo(lockable, owner);
			if(!lockInfo.HasLock(type))
			{
				if(CanClaimLock(lockable, owner, type))
				{
					var db = ApplicationManager.INSTANCE.GetDbProvider();
					var connection = db.CreateConnection();
					connection.Open();
					try
					{
						var transaction = connection.BeginTransaction(IsolationLevel.Serializable);
						try
						{
							using (var command = connection.CreateCommand())
							{
								command.CommandType = CommandType.Text;
								command.Transaction = transaction;
								if (lockInfo.LockType == LockType.None)
									command.CommandText = string.Format(INSERT_SQL, lockable, owner, (byte)type);
								else
									command.CommandText = string.Format(UPDATE_SQL, (byte)type, lockable, owner);
								command.ExecuteNonQuery();
								transaction.Commit();
							}
						}
						catch(DbException e)
						{
							transaction.Rollback();
							throw new ConcurrencyException("Unable to lock: " + lockable);
						}
					}
					finally
					{
						connection.Close();
					}
				}
				else
				{
					throw new ConcurrencyException("Unable to lock: " + lockable);
				}
			}
		}

		private LockInfo GetLockInfo(Guid lockable, Guid owner)
		{
			var info = new LockInfo(lockable, owner);
			var db = ApplicationManager.INSTANCE.GetDbProvider();
			var connection = db.CreateConnection();
			connection.Open();
			try
			{
				using(var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = string.Format(CHECK_SQL, lockable.ToString(), owner.ToString());
					var reader = command.ExecuteReader();
					if(reader.Read())
					{
						LockType result = LockType.None;
						if (Enum.TryParse(((byte)reader["LockType"]).ToString(), out result))
							info.LockType = result;
					}
				}
			}
			finally 
			{
				connection.Close();
			}
			return info;
		}

		private bool CanClaimLock(Guid lockable, Guid owner, LockType type)
		{
			var db = ApplicationManager.INSTANCE.GetDbProvider();
			var connection = db.CreateConnection();
			connection.Open();
			try
			{
				using (var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = string.Format(CHECK_UPGRADE_SQL, lockable, owner, (byte)type);
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						int conflicts = 0;
						int.TryParse(reader["Conflicts"].ToString(), out conflicts);
						return (conflicts == 0);
					}
				}
			}
			finally
			{
				connection.Close();
			}
			return false;
		}

		public void ReleaseLock(Guid lockable, Guid owner, LockType type)
		{
			var lockInfo = GetLockInfo(lockable, owner);
			if (lockInfo.HasLock(type))
			{
				var newLock = lockInfo.Downgrade(type);
				var db = ApplicationManager.INSTANCE.GetDbProvider();
				var connection = db.CreateConnection();
				connection.Open();
				try
				{
					var transaction = connection.BeginTransaction(IsolationLevel.Serializable);
					try
					{
						using (var command = connection.CreateCommand())
						{
							command.CommandType = CommandType.Text;
							command.Transaction = transaction;
							if (newLock == LockType.None)
								command.CommandText = string.Format(DELETE_SINGLE_SQL, lockable, owner);
							else
								command.CommandText = string.Format(UPDATE_SQL, (byte)newLock, lockable, owner);
							command.ExecuteNonQuery();
							transaction.Commit();
						}
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
				}
			}
		}

		public void ReleaseAllLocks(Guid owner)
		{
			var db = ApplicationManager.INSTANCE.GetDbProvider();
			var connection = db.CreateConnection();
			connection.Open();
			try
			{
				var transaction = connection.BeginTransaction(IsolationLevel.Serializable);
				try
				{
					using (var command = connection.CreateCommand())
					{
						command.CommandType = CommandType.Text;
						command.Transaction = transaction;
						command.CommandText = string.Format(DELETE_ALL_SQL, owner);
						command.ExecuteNonQuery();
						transaction.Commit();
					}
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
			}
		}

		private sealed class LockInfo
		{
			public LockInfo(Guid lockable, Guid owner)
			{
				this.Lockable = lockable;
				this.Owner = owner;
				this.LockType = LockType.None;
			}

			public Guid Lockable { get; private set; }
			public Guid Owner { get; private set; }
			public LockType LockType { get; set; }

			public bool HasLock(LockType value)
			{
				return (LockType >= value);
			}

			public bool CanClaim(LockType value)
			{
				return (value > LockType);
			}

			public LockType Downgrade(LockType type)
			{
				if ((type & LockType.Read) == 0)
					return LockType.Read;
				return LockType.None;
			}
		}
	}
}
