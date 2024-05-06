using EventStore;
using EventStore.Client;
using Microsoft.Extensions.Configuration;

namespace EventStoreTests
{
	public class EventStoreClientFixture : IDisposable
	{
		// todo: What is the best way to clear an event store for unit tests? https://github.com/EventStore/EventStore/issues/1328
		public EventStoreClientFixture()
		{
			IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
			string eventStoreConnectionString = config.GetConnectionString("eventStoreConnection");
			var settings = EventStoreClientSettings.Create(eventStoreConnectionString);
			Client = new(settings);
		}

		public EventStoreClient Client { get; private set; }

		#region IDisposable pattern
		bool disposed = false;

		void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					Client.Dispose();
				}

				disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}
		#endregion
	}
}
