using System.Threading.Tasks;
using MeliMessageSender.Controllers;
using Microsoft.WindowsAzure.Storage.Queue;
using Moq;
using Xunit;

namespace MeliMessageSenderTest
{
	public class NotificationsControllerTest
	{
		private Mock<RedisService> RedisServiceMock;
		private Mock<QueueService> QueueServiceMock;
		private NotificationsController Controller;

		public NotificationsControllerTest()
		{
			this.QueueServiceMock = new Mock<QueueService>(null);
			this.RedisServiceMock = new Mock<RedisService>(null);
			this.Controller = new NotificationsController(QueueServiceMock.Object, RedisServiceMock.Object);
		}

		[Fact]
		public async Task When_resource_exists_then_no_message_is_added_to_queue()
		{
			this.RedisServiceMock.Setup(r => r.IsUniqueNotification(It.IsAny<string>())).Returns(Task.FromResult(false));
			await Controller.Post(new Notification {resource = "aResource"});
			QueueServiceMock.Verify(q => q.AddMessage(It.IsAny<CloudQueueMessage>()), Times.Never);
		}

		[Fact]
		public async Task When_resource_is_unique_then_a_message_is_added_to_queue()
		{
			this.RedisServiceMock.Setup(r => r.IsUniqueNotification(It.IsAny<string>())).Returns(Task.FromResult(true));
			await Controller.Post(new Notification { resource = "aResource" });
			QueueServiceMock.Verify(q => q.AddMessage(It.IsAny<CloudQueueMessage>()), Times.Once);
		}
	}
}
