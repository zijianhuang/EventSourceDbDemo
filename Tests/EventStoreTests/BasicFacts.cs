using EventStore.Client;
using System.Text.Json;

namespace EventStoreTests
{
	public class BasicFacts : IClassFixture<EventStoreClientFixture>
	{
		public BasicFacts(EventStoreClientFixture fixture)
		{
			eventStoreClient = fixture.Client; // all tests here shared the same client connection
		}

		readonly EventStoreClient eventStoreClient;

		/// <summary>
		/// Basic example from EventSourceDb tutorial on https://developers.eventstore.com/clients/grpc/#creating-an-event
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task TestBasic()
		{
			var evt = new TestEvent
			{
				EntityId = Guid.NewGuid(),
				ImportantData = "I wrote my first event!"
			};

			var eventData = new EventData(
				Uuid.NewUuid(),
				"TestEvent",
				JsonSerializer.SerializeToUtf8Bytes(evt)
			);

			const string connectionString = "esdb://admin:changeit@localhost:2113?tls=true&tlsVerifyCert=false";
			/// tls should be set to true. Different from the official tutorial as of 2024-05-05 on https://developers.eventstore.com/clients/grpc/#creating-an-event. 
			/// I used the zipped EventStoreDb installed in Windows machine, launched with `EventStore.ClusterNode.exe --dev`

			var settings = EventStoreClientSettings.Create(connectionString);
			using EventStoreClient client = new(settings);
			string streamName = "some-stream";
			IWriteResult writeResult = await client.AppendToStreamAsync(
				streamName,
				StreamState.Any,
				new[] { eventData }
				); // todo: DEBUG step over will cause DeadlineExceeded. Look like a defect of EventStore.Client.Grpc. Report to the dev team later.

			EventStoreClient.ReadStreamResult readStreamResult = client.ReadStreamAsync(
				Direction.Forwards,
				streamName,
				StreamPosition.Start,
				10);

			ResolvedEvent[] events = await readStreamResult.ToArrayAsync();
			string eventText = System.Text.Encoding.Default.GetString(events[0].Event.Data.ToArray());
			TestEvent eventObj = JsonSerializer.Deserialize<TestEvent>(eventText);
			Assert.Equal("I wrote my first event!", eventObj.ImportantData);
		}

		[Fact]
		public async Task TestBackwardFromEnd()
		{
			string importantData = "I wrote my test with fixture " + DateTime.Now.ToString("yyMMddHHmmssfff");
			var evt = new TestEvent
			{
				EntityId = Guid.NewGuid(),
				ImportantData = importantData,
			};

			var eventData = new EventData(
				Uuid.NewUuid(),
				"testEvent", //The name of the event type. It is strongly recommended that these use lowerCamelCase, if projections are to be used.
				JsonSerializer.SerializeToUtf8Bytes(evt), // The raw bytes of the event data.
				null, // The raw bytes of the event metadata.
				"application/json" // The Content-Type of the EventStore.Client.EventData.Data. Valid values are 'application/json' and 'application/octet-stream'.
			);

			string streamName = "some-stream2";
			IWriteResult writeResult = await eventStoreClient.AppendToStreamAsync(
				streamName,
				StreamState.Any,
				new[] { eventData }
				);

			EventStoreClient.ReadStreamResult readStreamResult = eventStoreClient.ReadStreamAsync(
				Direction.Backwards,
				streamName,
				StreamPosition.End,
				10);
			Assert.Equal(TaskStatus.WaitingForActivation, readStreamResult.ReadState.Status);

			ResolvedEvent[] events = await readStreamResult.ToArrayAsync();
			string eventText = System.Text.Encoding.Default.GetString(events[0].Event.Data.ToArray());
			TestEvent eventObj = JsonSerializer.Deserialize<TestEvent>(eventText);
			Assert.Equal(importantData, eventObj.ImportantData); // so the first in events returned is the latest in the DB side.
		}

		[Fact]
		public async Task TestBackwardFromEndWriteOnly()
		{
			string importantData = "I wrote my test with fixture " + DateTime.Now.ToString("yyMMddHHmmssfff");
			var evt = new TestEvent
			{
				EntityId = Guid.NewGuid(),
				ImportantData = importantData,
			};

			var eventData = new EventData(
				Uuid.NewUuid(),
				"testEvent", //The name of the event type. It is strongly recommended that these use lowerCamelCase, if projections are to be used.
				JsonSerializer.SerializeToUtf8Bytes(evt), // The raw bytes of the event data.
				null, // The raw bytes of the event metadata.
				"application/json" // The Content-Type of the EventStore.Client.EventData.Data. Valid values are 'application/json' and 'application/octet-stream'.
			);

			string streamName = "some-stream2";
			IWriteResult writeResult = await eventStoreClient.AppendToStreamAsync(
				streamName,
				StreamState.Any,
				new[] { eventData }
				);

			Assert.True(writeResult.LogPosition.CommitPosition >0);
		}

		[Fact]
		public async Task TestDelete()
		{
			await TestBackwardFromEndWriteOnly();
			string streamName = "some-stream2";
			EventStoreClient.ReadStreamResult readStreamResult = eventStoreClient.ReadStreamAsync(
				Direction.Backwards,
				streamName,
				StreamPosition.End,
				10);
			Assert.Equal(TaskStatus.WaitingForActivation, readStreamResult.ReadState.Status);

			ResolvedEvent[] events = await readStreamResult.ToArrayAsync();
			string eventText = System.Text.Encoding.Default.GetString(events[0].Event.Data.ToArray());
			TestEvent eventObj = JsonSerializer.Deserialize<TestEvent>(eventText);

			var deleteResult = await eventStoreClient.DeleteAsync(streamName, StreamState.Any);
			Assert.True(deleteResult.LogPosition.CommitPosition > 0);
		}

	}
}