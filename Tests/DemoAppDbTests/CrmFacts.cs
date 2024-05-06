using Fonlow.DemoApp.DAL;
using Fonlow.DemoApp.Models;
using PoemsDbTests;

namespace DemoAppDbTests
{
	[Collection(TestConstants.MockInit)]
	public class CrmFacts : IClassFixture<DbContextFixture>
	{
		public CrmFacts(DbContextFixture fixture)
		{
			crmOperations = new CrmOperations(fixture.Options);
		}

		readonly CrmOperations crmOperations;

		Person AddPerson(){
			var givenName = "John" + DateTime.Now.ToString("yyMMddHHmmssffff");
			Person p = crmOperations.AddPerson( new()
			{
				Surname = "Smith",
				GivenName = givenName,
				Name= $"{givenName} Smith"
			});

			Assert.NotEqual(Guid.Empty, p.Id);
			return p;
		}

		[Fact]
		public void TestAddPerson()
		{
			AddPerson();
		}

		[Fact]
		public void TestUpdatePerson()
		{
			var p = AddPerson();
			p.Notes = "abcde " + DateTime.Now.ToString("yyMMddHHmmssffff");
			crmOperations.UpdatePerson(p);

			var newP = crmOperations.GetPerson(p.Id);
			Assert.Equal(p.Notes, newP.Notes);
		}

		[Fact]
		public void TestDeletePerson()
		{
			var p = AddPerson();
			crmOperations.DeletePerson(p.Id);
			var newP = crmOperations.GetPerson(p.Id);
			Assert.Null(newP);
		}


	}
}