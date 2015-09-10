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
	public interface IConcurrencyTest
	{
		void Execute();
	}

	class Program
	{
		static void Main(string[] args)
		{
			((ApplicationManager)ApplicationManager.INSTANCE).Inject(new SqlProvider(), new ReadWriteLockManager()); // HACK

			var tests = new IConcurrencyTest[] { new TestMultipleReads(), new TestSingleWrite(), new TestMultipleReadWrite() };

			foreach(var test in tests)
				test.Execute();

			Console.WriteLine("Press enter to close ...");
			Console.ReadLine();
		}
	}
}
