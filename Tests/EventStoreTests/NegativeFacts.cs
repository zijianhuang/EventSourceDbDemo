using EventStore.Client;
using Grpc.Core;
using System.Text.Json;

namespace EventStoreTests
{
	public class NegativeFacts : IClassFixture<EventStoreClientFixture>
	{
		public NegativeFacts(EventStoreClientFixture fixture)
		{
			eventStoreClient = fixture.Client; // all tests here shared the same client connection
		}

		readonly EventStoreClient eventStoreClient;

		/// <summary>
		/// Test with a host not existing
		/// https://learn.microsoft.com/en-us/aspnet/core/grpc/deadlines-cancellation
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task TestUnavailableThrows()
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

			const string connectionString = "esdb://admin:changeit@localhost:2000?tls=true&tlsVerifyCert=false"; // this connection is not there on port 2000
			var settings = EventStoreClientSettings.Create(connectionString);
			using EventStoreClient client = new(settings);
			string streamName = "some-stream";
			var ex = await Assert.ThrowsAsync<Grpc.Core.RpcException>(() => client.AppendToStreamAsync(
				streamName,
				StreamState.Any,
				new[] { eventData }
				));
			Assert.Equal(Grpc.Core.StatusCode.Unavailable, ex.StatusCode);
		}

		/// <summary>
		/// Simulate a slow or disrupted connection to trigger error.
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task TestDeadlineExceededThrows()
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

			string streamName = "some-stream";
			var ex = await Assert.ThrowsAsync<Grpc.Core.RpcException>(() => eventStoreClient.AppendToStreamAsync(
				streamName,
				StreamState.Any,
				new[] { eventData },
				null,
				TimeSpan.FromMicroseconds(2) // set deadline very short to trigger DeadlineExceeded. This could happen due to network latency or TCP/IP's nasty nature.
				));
			Assert.Equal(Grpc.Core.StatusCode.DeadlineExceeded, ex.StatusCode);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task TestReadNotExistingThrows()
		{
			EventStoreClient.ReadStreamResult readStreamResult = eventStoreClient.ReadStreamAsync(
				Direction.Backwards,
				"NotExistingStream",
				StreamPosition.End,
				10);
			Assert.Equal(TaskStatus.WaitingForActivation, readStreamResult.ReadState.Status);

			var ex = await Assert.ThrowsAsync<EventStore.Client.StreamNotFoundException>(async () => { var rs = await readStreamResult.ToArrayAsync(); });
			Assert.Contains("not found", ex.Message);
		}

		[Fact]
		public async Task TestReadNotExistingWhtNewStreamNameThrows_10()
		{
			var baseKey = DateTime.Now.ToString("yyMMddHHmmssfff");
			for (int i = 0; i < 10; i++)
			{
				EventStoreClient.ReadStreamResult readStreamResult = eventStoreClient.ReadStreamAsync(
					Direction.Backwards,
					"NotExistingStream" + baseKey + i.ToString(),
					StreamPosition.End,
					10);
				Assert.Equal(TaskStatus.WaitingForActivation, readStreamResult.ReadState.Status);

				var ex = await Assert.ThrowsAsync<EventStore.Client.StreamNotFoundException>(async () => { var rs = await readStreamResult.ToArrayAsync(); });
				Assert.Contains("not found", ex.Message);
			}
		}

		[Fact]
		public async Task TestReadNotExistingWhtSameStreamNameThrows_10()
		{
			var baseKey = DateTime.Now.ToString("yyMMddHHmmssfff");
			for (int i = 0; i < 10; i++)
			{
				EventStoreClient.ReadStreamResult readStreamResult = eventStoreClient.ReadStreamAsync(
					Direction.Backwards,
					"NotExistingStream" + baseKey + i.ToString(),
					StreamPosition.End,
					10);
				Assert.Equal(TaskStatus.WaitingForActivation, readStreamResult.ReadState.Status);

				var ex = await Assert.ThrowsAsync<EventStore.Client.StreamNotFoundException>(async () => { var rs = await readStreamResult.ToArrayAsync(); });
				Assert.Contains("not found", ex.Message);
			}
		}

		[Fact]
		public async Task TestDeleteNotExistingThrows()
		{
			var ex = await Assert.ThrowsAsync<EventStore.Client.WrongExpectedVersionException>(() => eventStoreClient.DeleteAsync("SomethingNotExisting", StreamState.Any));
			Assert.Equal(StatusCode.FailedPrecondition, (ex.InnerException as Grpc.Core.RpcException).StatusCode);
		}

	}
}