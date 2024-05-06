using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fonlow.DemoApp.Models;

namespace Fonlow.DemoApp.EF
{
	internal class AddressETC : IEntityTypeConfiguration<Address>
	{
		public void Configure(EntityTypeBuilder<Address> builder)
		{
			builder.HasKey(d => new { d.Id });
			builder.Property(t => t.Id).ValueGeneratedOnAdd();
			builder.Property(t => t.AddressType);
			builder.Property(t => t.EntityId);
			builder.Property(t => t.Street1).HasMaxLength(128);
			builder.Property(t => t.Street2).HasMaxLength(128);
			builder.Property(t => t.City).HasMaxLength(32);
			builder.Property(t => t.State).HasMaxLength(32);
			builder.Property(t => t.Postcode).HasMaxLength(10);
			builder.Property(t => t.Country).HasMaxLength(32);


			builder.ToTable("addresses");
		}
	}

	internal class PhoneNumberETC : IEntityTypeConfiguration<PhoneNumber>
	{
		public void Configure(EntityTypeBuilder<PhoneNumber> builder)
		{
			builder.HasKey(d => new { d.Id });
			builder.Property(t => t.Id).ValueGeneratedOnAdd();
			builder.Property(t => t.FullNumber).HasMaxLength(20).IsRequired();
			builder.Property(t => t.PhoneType);
			builder.Property(t => t.EntityId);

			builder.ToTable("phonenumbers");
		}
	}

	internal class EntityETC : IEntityTypeConfiguration<Entity>
	{
		public void Configure(EntityTypeBuilder<Entity> builder)
		{
			builder.HasKey(d => d.Id);
			builder.Property(t => t.Id).ValueGeneratedOnAdd();

			builder.Property(t => t.Name).IsRequired().HasMaxLength(64);
			builder.Property(t => t.Alias).HasMaxLength(64);
			builder.HasIndex(t => new { t.Name, t.Alias }).IsUnique();

			builder.HasMany(t => t.Addresses).WithOne()//so to cascade delete
						   .HasForeignKey(d => d.EntityId).OnDelete(DeleteBehavior.Cascade);

			builder.HasMany(t => t.PhoneNumbers).WithOne()
						   .HasForeignKey(d => d.EntityId).OnDelete(DeleteBehavior.Cascade);

			builder.Property(t => t.EmailAddress).HasMaxLength(96);
			builder.Property(t => t.Web).HasMaxLength(96);

			builder.Property(t => t.Notes);

			builder.Property("Discriminator").HasMaxLength(16);

			builder.HasIndex(d => d.ExpiryUtc);
			builder.ToTable("entities");
		}
	}

	internal class BizEntityETC : IEntityTypeConfiguration<BizEntity>
	{
		public void Configure(EntityTypeBuilder<BizEntity> builder)
		{

			builder.Property(t => t.TradeName).HasMaxLength(64);
			builder.Property(t => t.Industries).HasMaxLength(128);
		}
	}

	internal class PersonETC : IEntityTypeConfiguration<Person>
	{
		public void Configure(EntityTypeBuilder<Person> builder)
		{
			builder.Property(t => t.Surname).HasMaxLength(40); //Medicare says 40 characters.
			builder.Property(t => t.GivenName).HasMaxLength(40);
			builder.Property(t => t.MiddleName).HasMaxLength(32);
			builder.Property(t => t.Title).HasMaxLength(32);
			builder.Property(t => t.Sex).HasMaxLength(1);
			builder.Property(t => t.Nation).HasMaxLength(36);
		}
	}

	internal class BizPeopleMapETC : IEntityTypeConfiguration<BizPeopleMap>
	{
		public void Configure(EntityTypeBuilder<BizPeopleMap> builder)
		{
			builder.HasKey(d => new { d.BizEntityId, d.PersonId });
			builder.Property(t => t.Role).HasMaxLength(64);

			builder.ToTable("BizPeopleMap");
		}
	}

	internal class P2PMapETC : IEntityTypeConfiguration<P2PMap>
	{
		public void Configure(EntityTypeBuilder<P2PMap> builder)
		{
			builder.HasKey(d => new { d.MeId, d.TheOtherId });
			builder.Property(t => t.Role).HasMaxLength(64);

			builder.ToTable("P2PMap");
		}
	}

	internal class B2BMapETC : IEntityTypeConfiguration<B2BMap>
	{
		public void Configure(EntityTypeBuilder<B2BMap> builder)
		{
			builder.HasKey(d => new { d.MeId, d.TheOtherId });
			builder.Property(t => t.Role).HasMaxLength(64);

			builder.HasOne(d => d.Me).WithMany(d => d.B2BJunction).IsRequired().HasForeignKey(d => d.MeId);
			builder.HasOne(d => d.TheOther).WithMany().IsRequired().HasForeignKey(d => d.TheOtherId);
			builder.ToTable("B2BMap");
		}
	}


}
