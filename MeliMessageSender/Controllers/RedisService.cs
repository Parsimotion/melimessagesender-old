using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace MeliMessageSender.Controllers
{
	public class RedisService
	{
		public static bool RedisStatus { get; set; }
		private const string NOTIFICATIONS_SET_KEY_NAME = "unique-notifications";
		private readonly IDatabase Database;

		public RedisService(IDatabase database)
		{
			RedisStatus = true;
			this.Database = database;
		}

		public virtual async Task<bool> IsUniqueNotification(string resource)
		{
			try
			{
				//Here the sequence of calls matter
				var isUnique = await Database.SetAddAsync(NOTIFICATIONS_SET_KEY_NAME, resource);
				RedisStatus = true;
				return isUnique;
			}
			catch (Exception e)
			{
				RedisStatus = false;
				return true;
			}
		}
	}
}