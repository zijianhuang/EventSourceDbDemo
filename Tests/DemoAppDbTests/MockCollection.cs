using Fonlow.EntityFrameworkCore;
using Fonlow.DemoApp;
using Fonlow.DemoApp.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PoemsDbTests
{
	public static class TestConstants
	{
		/// <summary>
		/// For ConnectionDefinition that will drop and create DB schema.
		/// </summary>
		public const string MockInit = "MockInit";
		//public const string MockMore = "MockMore";
	}

	/// <summary>
	/// Collection fixture to use DbInitFixture.
	/// http://xunit.github.io/docs/shared-context.html
	/// </summary>
	[CollectionDefinition(TestConstants.MockInit)]
	public class DbSetupCollection : ICollectionFixture<DbInitFixture>
	{
		// This class has no code, and is never created. Its purpose is simply
		// to be the place to apply [CollectionDefinition] and all the
		// ICollectionFixture<> interfaces.
	}

	public class DbFixtureBase {
		readonly protected DemoAppDb demoAppDb;

		public DbFixtureBase()
		{
			IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
			var appSettings = config.GetSection("appSettings");
			var plugins = appSettings.GetSection("dbEngineDbContextPlugins").Get<string[]>();
			if (plugins == null || plugins.Length == 0)
			{
				Console.Error.WriteLine("No plugin of dbEngineDbContext found in appSettings");
				throw new ArgumentException("Need plugin");
			}

			var dbEngineDbContext = DbEngineDbContextLoader.CreateDbEngineDbContextFromAssemblyFile(plugins[0] + ".dll");

			demoAppDb = new DemoAppDb(config, dbEngineDbContext);
		}
	}

	/// <summary>
	/// For creating DB. Intended to be used as collection fixture shared by all test classes which use the same DB.
	/// </summary>
	public class DbInitFixture : DbFixtureBase, IAsyncLifetime
	{
		public DbInitFixture() { }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
		public async Task DisposeAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
		{
			//do nothing
		}

		public async Task InitializeAsync()
		{
			await demoAppDb.DropAndCreate();
		}
	}

	/// <summary>
	/// Fixture used by each test class, using the same DB context options without recreating DB
	/// </summary>
	public class DbContextFixture :DbFixtureBase
	{
		public DbContextFixture(){}

		public DbContextOptions<DemoAppContext> Options => demoAppDb.GetOptions();
	}
}
