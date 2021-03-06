﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.PessimisticOfflineReadWriteLock.Infrastructure.Domain
{
	public abstract class EntityBase : IAggregateRoot
	{
		protected EntityBase() { }

		public Guid Id { get; set; }

		public override bool Equals(object entity)
		{
			return entity != null
				&& entity is EntityBase
				&& this == (EntityBase)entity;
		}

		public override int GetHashCode()
		{
			return this.Id.GetHashCode();
		}

		public static bool operator ==(EntityBase entity1, EntityBase entity2)
		{
			if ((object)entity1 == null && (object)entity2 == null)
				return true;

			if ((object)entity1 == null || (object)entity2 == null)
				return false;

			if (entity1.Id.ToString() == entity2.Id.ToString())
				return true;

			return false;
		}

		public static bool operator !=(EntityBase entity1, EntityBase entity2)
		{
			return (!(entity1 == entity2));
		}
	}
}
