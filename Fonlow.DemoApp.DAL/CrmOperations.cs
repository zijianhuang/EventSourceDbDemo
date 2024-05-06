using Fonlow.DemoApp.Models;
using Fonlow.DemoApp.EF;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Fonlow.DemoApp.DAL
{
	public class CrmOperations
	{
		public CrmOperations(DbContextOptions<DemoAppContext> options)
		{
			this.options = options;
		}

		readonly DbContextOptions<DemoAppContext> options;

		DemoAppContext NewContext()
		{
			var c = new DemoAppContext(options);
			c.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
			return c;
		}

		public Person GetPerson(Guid id)
		{
			using var context = NewContext();
			return context.People.SingleOrDefault(d => d.Id == id);
		}

		public Person AddPerson(Person person)
		{
			if (person == null)
				throw new DemoAppArgumentNullException(nameof(person));

			if (person.Id != Guid.Empty)
				throw new DemoAppArgumentNullException("This entity already has Id. Not good", nameof(person));

			using var context = NewContext();
			using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction();
			try
			{
				context.People.Add(person);
				context.SaveChanges();
				transaction.Commit();
				return person;
			}
			catch (Exception)
			{
				transaction.Rollback();
				throw;
			}
		}

		public void UpdatePerson(Person person)
		{
			if (person.Id.Equals(Guid.Empty))
			{
				throw new ArgumentException("Not an existing poem"); //otherwise DbUpdateException is thrown
			}

			try
			{
				using var context = NewContext();
				context.People.Attach(person);
				context.Entry(person).State = EntityState.Modified;
				context.SaveChanges();

			}
			catch (DbUpdateException ex)
			{
				string msg = ex.ToErrorsText();
				Trace.TraceError(msg);
				throw new DemoAppArgumentException("Not associated with an existing entity. -- " + msg);
			}
		}

		public int DeletePerson(Guid id)
		{
			try
			{
				using var context = NewContext();
				Person p = new();
				p.Id = id;
				context.People.Attach(p);
				context.People.Remove(p);
				int c = context.SaveChanges();
				return c;
			}
			catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
			{
				return -1;
			}
		}

	}
}
