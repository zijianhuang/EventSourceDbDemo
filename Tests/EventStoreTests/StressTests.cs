using EventStore.Client;
using System.Text.Json;

namespace EventStoreTests
{
	public class StressTests : IClassFixture<EventStoreClientFixture>
	{
		public StressTests(EventStoreClientFixture fixture)
		{
			eventStoreClient = fixture.Client; // all tests in the same class shared the same client connection
		}

		readonly EventStoreClient eventStoreClient;


		/// <summary>
		/// For the same stream name, the subsequent calls are super quick.
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task TestBackwardFromEnd100()
		{
			for (int i = 0; i < 100; i++)
			{
				string importantData = "I wrote my test with fixture " + DateTime.Now.ToString("yyMMddHHmmssfff");
				var evt = new TestEvent
				{
					EntityId = Guid.NewGuid(),
					ImportantData = importantData,
				};

				var eventData = new EventData(
					Uuid.NewUuid(),
					"testEventStress", 
					JsonSerializer.SerializeToUtf8Bytes(evt), 
					null, 
					"application/json" 
				);

				string streamName = "some-streamStress";
				IWriteResult writeResult = await eventStoreClient.AppendToStreamAsync(
					streamName,
					StreamState.Any,
					new[] { eventData }
					);

				EventStoreClient.ReadStreamResult readStreamResult = eventStoreClient.ReadStreamAsync(
					Direction.Backwards,
					streamName,
					StreamPosition.End,
					10); //maxCount

				ResolvedEvent[] events = await readStreamResult.ToArrayAsync();
				string eventText = System.Text.Encoding.Default.GetString(events[0].Event.Data.ToArray());
				TestEvent eventObj = JsonSerializer.Deserialize<TestEvent>(eventText);
				Assert.Equal(importantData, eventObj.ImportantData); // so the first in events returned is the latest in the DB side.
			}
		}

		[Fact]
		public async Task TestBackwardFromEndWriteOnly_100()
		{
			for (int i = 0; i < 100; i++)
			{
				string importantData = "I wrote my test with fixture " + DateTime.Now.ToString("yyMMddHHmmssfff");
				var evt = new TestEvent
				{
					EntityId = Guid.NewGuid(),
					ImportantData = importantData,
				};

				var eventData = new EventData(
					Uuid.NewUuid(),
					"testEventStress",
					JsonSerializer.SerializeToUtf8Bytes(evt),
					null,
					"application/json"
				);

				string streamName = "some-streamStress";
				IWriteResult writeResult = await eventStoreClient.AppendToStreamAsync(
					streamName,
					StreamState.Any,
					new[] { eventData }
					);

				Assert.True(writeResult.LogPosition.CommitPosition > 0); 
			}
		}

	}
}