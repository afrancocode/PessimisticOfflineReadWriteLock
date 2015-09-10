using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOfflineReadWriteLock.Manager;

namespace Concurrency.PessimisticOfflineReadWriteLock.AppConsole
{
	public sealed class TestSingleWrite : IConcurrencyTest
	{
		public void Execute()
		{
			var manager = ApplicationManager.INSTANCE;

			var id = new Guid("a004c037-294d-4796-b51b-070a4e832241");
			var user1 = new TestInfo(id);

			user1.LoadEntity();
			user1.EditEntity("New Name");
			user1.SaveEntity();
			user1.CloseEntity();
			user1.Close();

			var user2 = new TestInfo(id);
			user2.LoadEntity();
			user2.EditEntity("Alfonso");
			user2.SaveEntity();
			user2.CloseEntity();
			user2.Close();
		}
	}
}
