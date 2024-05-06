using Fonlow.EntityFrameworkCore.Abstract;
using Fonlow.DemoApp.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Fonlow.DemoApp
{
	public class DemoAppDb
	{
		readonly DbContextOptions<DemoAppContext> options;

		readonly IConfiguration appConfig;

		string basicConnectionString;
		readonly IDbEngineDbContext dbEngineDbContext;

		/// <summary>
		/// Connect to database and get DB options
		/// </summary>
		/// <param name="config">It should contain a connection string named after "DbConnection"</param>
		/// <param name="dbEngineDbContext">For specific DB engine</param>
		public DemoAppDb(IConfiguration config, IDbEngineDbContext dbEngineDbContext)
		{
			appConfig = config;
			basicConnectionString = appConfig.GetConnectionString("DemoAppDbConnection");
			this.dbEngineDbContext = dbEngineDbContext;
			this.options = GetOptions();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		/// <param name="connectionString"></param>
		/// <param name="dbEngineDbContext"></param>
		public DemoAppDb(IConfiguration config, string connectionString, IDbEngineDbContext dbEngineDbContext)
		{
			appConfig = config;
			this.basicConnectionString = connectionString;
			this.dbEngineDbContext = dbEngineDbContext;
			this.options = GetOptions();
		}
		/// <summary>
		/// create DB according to connection string
		/// </summary>
		public async Task DropAndCreate()
		{
			using DemoAppContext context = NewDemoAppContext();
			if (context.Database.EnsureDeleted())
			{
				Console.WriteLine("Old db is deleted.");
			}

			await context.Database.EnsureCreatedAsync();

			Console.WriteLine(String.Format("Database is initialized, created: {0}", context.Database.GetDbConnection().ConnectionString));
		}

		public DemoAppContext NewDemoAppContext(){
			DemoAppContext context = new(options);
			return context;
		}

		/// <summary>
		/// Make initial connection and get DbContextOptions.
		/// </summary>
		/// <returns></returns>
		public DbContextOptions<DemoAppContext> GetOptions()
		{
			var optionsBuilder = new DbContextOptionsBuilder<DemoAppContext>();
			Console.WriteLine($"Ready to connect {dbEngineDbContext.DbEngineName} db with {basicConnectionString} ...");
			dbEngineDbContext.ConnectDatabase(optionsBuilder, basicConnectionString);
			return optionsBuilder.Options;
		}
	}
}
