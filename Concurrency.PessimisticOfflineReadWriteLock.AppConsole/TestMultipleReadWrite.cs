using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOfflineReadWriteLock.Manager;

namespace Concurrency.PessimisticOfflineReadWriteLock.AppConsole
{
	public sealed class TestMultipleReadWrite : IConcurrencyTest
	{
		public void Execute()
		{
			var manager = ApplicationManager.INSTANCE;

			var id = new Guid("a004c037-294d-4796-b51b-070a4e832241");
			var user1 = new TestInfo(id);

			user1.LoadEntity();

			var oldName = user1.Entity.Name;
			var newName = oldName + oldName.Length;
			
			var user2 = new TestInfo(id);
			user2.LoadEntity();

			user1.EditEntity(newName);

			user2.CloseEntity();

			user1.EditEntity(newName);

			user2.LoadEntity();

			user1.SaveEntity();

			user2.LoadEntity();

			user1.CloseEntity();

			user2.EditEntity(oldName);
			user2.SaveEntity();
			user2.CloseEntity();

			user1.LoadEntity();
			Debug.Assert(user1.Entity.Name == oldName);
			user1.CloseEntity();
			
			user1.Close();
			user2.Close();
		}
	}
}
