using Fonlow.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Fonlow.DemoApp
{
	/// <summary>
	/// Create database AppAuth for development.
	/// Without arguments, this will create a database according to connection string in appsettings.json.
	/// </summary>
	class Program
	{
		static async Task<int> Main(string[] args)
		{
			DemoAppDb demoAppDb;
			IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
			var appSettings = config.GetSection("appSettings");

			if (args.Length == 0)//for internal development
			{
				Console.WriteLine("Create database with connection string in appsetings.json ...");
				var plugins = appSettings.GetSection("dbEngineDbContextPlugins").Get<string[]>();
				if (plugins == null || plugins.Length == 0)
				{
					Console.Error.WriteLine("No plugin of dbEngineDbContext found in appSettings");
					return 10;
				}

				var dbEngineDbContext = DbEngineDbContextLoader.CreateDbEngineDbContextFromAssemblyFile(plugins[0] + ".dll");
				if (dbEngineDbContext == null)
				{
					Console.Error.WriteLine("No dbEngineDbContext");
					return 11;
				}

				demoAppDb = new DemoAppDb(config, dbEngineDbContext);
				await demoAppDb.DropAndCreate();
			}
			else
			{
				var pluginAssemblyName = args[0];
				var dbEngineDbContext = DbEngineDbContextLoader.CreateDbEngineDbContextFromAssemblyFile(pluginAssemblyName + ".dll");
				if (dbEngineDbContext == null)
				{
					Console.Error.WriteLine("No dbEngineDbContext");
					return 11;
				}

				var connectionString = args[1];
				Console.WriteLine("Create database with arguments ...");
				demoAppDb = new DemoAppDb(config, connectionString, dbEngineDbContext);
				await demoAppDb.DropAndCreate();
			}

			Console.WriteLine("Done.");
			return 0;
		}
	}

}
