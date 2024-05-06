using Fonlow.DemoApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Fonlow.DemoApp.EF
{
	/// <summary>
	/// 
	/// </summary>
	public class DemoAppContext : DbContext
	{
		public DemoAppContext(DbContextOptions<DemoAppContext> options)
			: base(options)
		{
		}

		//The DbSet properties are for accessing table in codes.
		public DbSet<Entity> Entities { get; set; }
		public DbSet<Person> People { get; set; }
		public DbSet<BizEntity> BizEntities { get; set; }
		public DbSet<Address> Addresses { get; set; }
		public DbSet<PhoneNumber> PhoneNumbers { get; set; }


		public DbSet<BizPeopleMap> BizPeopleMap { get; set; }
		public DbSet<P2PMap> P2PMap { get; set; }
		public DbSet<B2BMap> B2BMap { get; set; }

		public bool HasChanges
		{
			get
			{
				return ChangeTracker.HasChanges();
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			modelBuilder.UseCollation("utf8_general_ci");
			base.OnModelCreating(modelBuilder);

			//To create tables
			modelBuilder.ApplyConfiguration(new AddressETC());
			modelBuilder.ApplyConfiguration(new PhoneNumberETC());

			modelBuilder.ApplyConfiguration(new EntityETC());
			modelBuilder.ApplyConfiguration(new PersonETC());
			modelBuilder.ApplyConfiguration(new BizEntityETC());

			//Junction tables
			modelBuilder.ApplyConfiguration(new BizPeopleMapETC());
			modelBuilder.ApplyConfiguration(new P2PMapETC());
			modelBuilder.ApplyConfiguration(new B2BMapETC());

			ValueConverter<DateTime, DateTime> dateTimeConverter = new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
			// Thanks to https://stackoverflow.com/questions/50727860/ef-core-2-1-hasconversion-on-all-properties-of-type-datetime
			foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
			{
				foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableProperty property in entityType.GetProperties())
				{
					if ((property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?)) && property.Name.Contains("Utc", StringComparison.OrdinalIgnoreCase))
						property.SetValueConverter(dateTimeConverter);
				}
			}
		}

		public override int SaveChanges()
		{
			System.Collections.Generic.IEnumerable<object> entities = from e in ChangeTracker.Entries()
																	  where e.State == EntityState.Added
																		  || e.State == EntityState.Modified
																	  select e.Entity;
			foreach (object entity in entities)
			{
				ValidationContext validationContext = new ValidationContext(entity);
				Validator.ValidateObject(entity, validationContext);
			}

			return base.SaveChanges();
		}

		/// <summary>
		/// https://stackoverflow.com/questions/46430619/net-core-2-ef-core-error-handling-save-changes
		/// </summary>
		void Validate()
		{
			System.Collections.Generic.IEnumerable<object> entities = from e in ChangeTracker.Entries()
																	  where e.State == EntityState.Added
																		  || e.State == EntityState.Modified
																	  select e.Entity;
			foreach (object entity in entities)
			{
				ValidationContext validationContext = new ValidationContext(entity);
				Validator.ValidateObject(entity, validationContext);
			}
		}
		public override Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken)
		{
			Validate();
			return base.SaveChangesAsync(cancellationToken);
		}

	}

}
