using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Locking
{
	[Flags]
	public enum LockType
	{
		None = 0x0,
		Read = 0x1,
		Write = 0x2,
		Full = Read | Write,
	}
}
