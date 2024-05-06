
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Fonlow.DemoApp.Models
{
	#region Entities
	/// <summary>
	/// Base class of people or organizations. Table entities.
	/// </summary>
	[DataContract(Namespace = Constants.DataNamespace)]
	public class Entity
	{
		/// <summary>
		/// 
		/// </summary>
		public Entity()
		{
			Addresses = new List<Address>();
			PhoneNumbers = new List<PhoneNumber>();
		}

		[DataMember]
		public Guid Id { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Name and Alias (Known As) together should be unique. So people and company with the same name could use different alias.
		/// At the UI level, if an Entity has an alias different from name, both will be displayed.
		/// </summary>
		[DataMember]
		public string Alias { get; set; }

		[DataMember]
		public virtual List<Address> Addresses { get; set; }

		[DataMember]
		public virtual List<PhoneNumber> PhoneNumbers { get; set; }



		[DataMember]
		[Display(Name = "Email")]
		public string EmailAddress
		{
			get;
			set;
		}

		[DataMember]
		[Display(Name = "Web")]
		public string Web
		{
			get;
			set;
		}

		[DataMember]
		public string ABN { get; set; }

		[DataMember]
		public string Notes { get; set; }

		/// <summary>
		/// Optional birthdate. For Biz, it is the registration date.
		/// </summary>
		//[DateTimeKind(DateTimeKind.Utc)]
		[DataMember]
		public DateOnly? DOB { get; set; }

		public override string ToString()
		{
			return Name;
		}

		/// <summary>
		/// Equal Id and Name.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is not Entity entity)
			{
				return false;
			}

			return Id == entity.Id && Name == entity.Name;
		}


		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		//[DataMember] with audit trail or event sourcing, you may not need this.
		//public DateTimeOffset? CreatedUtc { get; set; }

		//[DataMember]
		//public DateTimeOffset? ModifiedUtc { get; set; }

		[DataMember]
		public DateTimeOffset? ExpiryUtc { get; set; }
	}

	/// <summary>
	/// Table people.
	/// At UI level, Name will be composed as "Surname, Givenname". In the event of identical name is identified, an alias could be created base on birthdate or address., 
	/// </summary>
	[DataContract(Namespace = Constants.DataNamespace)]
	public class Person : Entity
	{
		public Person()
		{
			BizPeopleJunction = new List<BizPeopleMap>();
			P2PJunction = new List<P2PMap>();
		}

		[DataMember]
		public string Surname { get; set; }

		[DataMember]
		public string GivenName { get; set; }

		[DataMember]
		public string MiddleName { get; set; }


		[DataMember]
		public string Title { get; set; }


		/// <summary>
		/// M for Male, F or Female, and others for political correctness in the future.
		/// </summary>
		[DataMember]
		public string Sex { get; set; }

		public virtual IList<BizPeopleMap> BizPeopleJunction { get; set; }

		/// <summary>
		/// To be serialized only at the client side, for adding a new patient associated with current login patient, in the booking app.
		/// </summary>
		[DataMember]
		public virtual IList<P2PMap> P2PJunction { get; set; } //To be serialized only at the client side

		//[DataMember]
		//public EmployeeProfile Employee { get; set; }

		[DataMember]
		public string Nation { get; set; }

		public override string ToString()
		{
			return $"{Title} {Surname}, {GivenName}; {MiddleName}";
		}
	}

	/// <summary>
	/// Business organizations. Table bizentities
	/// </summary>
	[DataContract(Namespace = Constants.DataNamespace)]
	public class BizEntity : Entity
	{
		public BizEntity()
		{
			BizPeopleJunction = new List<BizPeopleMap>();
			B2BJunction = new List<B2BMap>();
		}

		[DataMember]
		[Display(Name = "Trading Name")]
		public string TradeName { get; set; }

		[DataMember]
		public string ACN { get; set; }

		/// <summary>
		/// If multiple is needed, make it a CSV.
		/// </summary>
		[DataMember]
		public string Industries { get; set; }

		public virtual IList<BizPeopleMap> BizPeopleJunction { get; set; }

		public virtual IList<B2BMap> B2BJunction { get; set; }

	}


	#endregion

	/// <summary>
	/// bizpeoplemap
	/// </summary>
	[DataContract(Namespace = Constants.DataNamespace)]
	public class BizPeopleMap
	{
		//[DataMember]
		//public Guid Id { get; set; }

		[DataMember]
		public Guid BizEntityId { get; set; }

		//		public BizEntity BizEntity { get; set; }

		[DataMember]
		public Guid PersonId { get; set; }

		//		public Person Person { get; set; }

		/// <summary>
		/// Owner, manager, or staff or customer/patient etc. symbolic
		/// </summary>
		[DataMember]
		public string Role { get; set; }
	}

	[DataContract(Namespace = Constants.DataNamespace)]
	public class P2PMap
	{

		//		public virtual Person Me { get; set; }
		Guid meId;
		[DataMember]
		public Guid MeId
		{
			get
			{
				return meId;
			}

			set
			{
				if (value == TheOtherId)
					throw new DemoAppArgumentException("Must not have relationship to myself.");

				meId = value;
			}
		}

		//		public virtual Person TheOtehr { get; set; }

		Guid theOtherId;
		[DataMember]
		public Guid TheOtherId
		{
			get { return theOtherId; }
			set
			{
				if (MeId == value)
					throw new DemoAppArgumentException("Must not have relationship to myself.");

				theOtherId = value;
			}
		}

		/// <summary>
		/// Next kin, son, father, employee etc.
		/// </summary>
		[DataMember]
		public string Role { get; set; }
	}

	[DataContract(Namespace = Constants.DataNamespace)]
	public class B2BMap
	{

#pragma warning disable CA1716 // Identifiers should not match keywords
		public virtual BizEntity Me { get; set; }
#pragma warning restore CA1716 // Identifiers should not match keywords
		Guid meId;
		[DataMember]
		public Guid MeId
		{
			get
			{
				return meId;
			}

			set
			{
				if (value == TheOtherId)
					throw new DemoAppArgumentException("Must not have relationship to myself.");

				meId = value;
			}
		}

		public virtual BizEntity TheOther { get; set; }

		Guid theOtherId;
		[DataMember]
		public Guid TheOtherId
		{
			get { return theOtherId; }
			set
			{
				if (MeId == value)
					throw new DemoAppArgumentException("Must not have relationship to myself.");

				theOtherId = value;
			}
		}

		/// <summary>
		/// Next kin, son, father, employee etc.
		/// </summary>
		[DataMember]
		public string Role { get; set; }
	}


	[DataContract(Namespace = Constants.DataNamespace)]
	[Flags]
	public enum AddressType
	{
		[EnumMember]
		Undefined = 0,
		[EnumMember]
		Geo = 1,
		[EnumMember]
		Postal = 2,
		[EnumMember]
		Delivery = 4,
		[EnumMember]
		Home = 8
	}

	public static class AddressTypeExtension
	{
		public static string ToShortString(this AddressType t)
		{
			return t == AddressType.Undefined ? String.Empty : t.ToString();
		}
	}

	/// <summary>
	/// Common structure used by tables. Better not to have its own table. Table addresses.
	/// </summary>
	[DataContract(Namespace = Constants.DataNamespace)]
	public class Address
	{
		public Address()
		{
			Country = "AUS";
		}

		[DataMember]
		public long Id { get; set; }


		[DataMember]
		public string Street1 { get; set; }
		[DataMember]
		public string Street2 { get; set; }

		[DataMember]
		public string City { get; set; }

		[DataMember]
		public string State { get; set; }

		[DataMember]
		public string Postcode { get; set; }

		[DataMember]
		public string Country { get; set; }

		[DataMember]
		[Display(Name = "Type")]
		public AddressType AddressType { get; set; }

		/// <summary>
		/// Foreign key to Entity defined in Fluent
		/// </summary>
		[DataMember]
		public Guid EntityId { get; set; }
	}


	[DataContract(Namespace = Constants.DataNamespace)]
	public enum PhoneType
	{
		[EnumMember]
		Tel = 0,
		[EnumMember]
		Mobile = 1,
		[EnumMember]
		Skype = 2,
		[EnumMember]
		Fax = 3,
		[EnumMember]
		WorkTel = 4,
		[EnumMember]
		HomeTel = 5,
	}

	/// <summary>
	/// Table phonenumbers
	/// </summary>
	[DataContract(Namespace = Constants.DataNamespace)]
	public class PhoneNumber
	{
		[DataMember]
		public long Id { get; set; }

		[DataMember]
		[Display(Name = "Number")]
		public string FullNumber { get; set; }

		[DataMember]
		[Display(Name = "Type")]
		public PhoneType PhoneType { get; set; }

		[DataMember]
		public Guid EntityId { get; set; }
	}

}
