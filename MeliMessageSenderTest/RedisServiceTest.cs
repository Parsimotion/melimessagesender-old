using System;
using System.Threading.Tasks;
using MeliMessageSender.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StackExchange.Redis;

namespace MeliMessageSenderTest
{
	[TestClass]
	public class RedisServiceTest
	{
		private RedisService Service { get; set; }
		private Mock<IDatabase> DatabaseMock;

		[TestInitialize]
		public void SetUp()
		{
			this.DatabaseMock = new Mock<IDatabase>();
		}

		[TestMethod]
		public async Task When_redis_fails_a_resouse_is_unique_regardless_else()
		{
			this.DatabaseMock.Setup(d => d.SetAddAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>())).ThrowsAsync(new Exception("La pucha! Pasó algo en Redis"));
			this.Service = new RedisService(DatabaseMock.Object);
			Assert.IsTrue(await this.Service.IsUniqueNotification("aResource"));
		}

		[TestMethod]
		public async Task A_resource_is_unique_when_resource_is_adding_to_set()
		{
			this.DatabaseMock.Setup(d => d.SetAddAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>())).ReturnsAsync(true);
			this.Service = new RedisService(DatabaseMock.Object);
			Assert.IsTrue(await this.Service.IsUniqueNotification("aResource"));
		}

		[TestMethod]
		public async Task A_resource_exists_when_already_exists_in_set()
		{
			this.DatabaseMock.Setup(d => d.SetAddAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>())).ReturnsAsync(false);
			this.Service = new RedisService(DatabaseMock.Object);
			Assert.IsFalse(await this.Service.IsUniqueNotification("aResource"));
		}

		[TestMethod]
		public async Task When_redis_responds_status_is_true()
		{
			this.DatabaseMock.Setup(d => d.SetAddAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>())).ReturnsAsync(false);
			await new RedisService(DatabaseMock.Object).IsUniqueNotification("aResource");
			Assert.IsTrue(RedisService.RedisStatus);
		}

		[TestMethod]
		public async Task When_redis_fails_status_is_false()
		{
			this.DatabaseMock.Setup(d => d.SetAddAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>())).ThrowsAsync(new Exception("La pucha! Pasó algo en Redis"));
			await new RedisService(DatabaseMock.Object).IsUniqueNotification("aResource");
			Assert.IsFalse(RedisService.RedisStatus);
		}
	}
}