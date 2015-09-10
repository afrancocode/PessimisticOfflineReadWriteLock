using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.PessimisticOfflineReadWriteLock.DbLockManager;
using Concurrency.PessimisticOfflineReadWriteLock.Manager;

namespace Concurrency.PessimisticOfflineReadWriteLock.AppConsole
{
	public sealed class TestMultipleReads : IConcurrencyTest
	{
		public void Execute()
		{
			var manager = ApplicationManager.INSTANCE;

			var id = new Guid("a004c037-294d-4796-b51b-070a4e832241");
			var user1 = new TestInfo(id);

			user1.LoadEntity();

			var user2 = new TestInfo(id);
			user2.LoadEntity();

			user1.CloseEntity();
			user2.CloseEntity();

			user1.Close();
			user2.Close();
		}
	}
}
