using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using LazyCache;
using MeliMessageSender.Models;
using MeliMessageSender.Services;
using Microsoft.ServiceBus.Messaging;

namespace MeliMessageSender.Controllers
{
	public class NotificationsController : ApiController
	{
		private readonly QueueClient queueClient;
		private readonly MercadolibreApi mercadolibre;
		static readonly IAppCache cache = new CachingService();
		static readonly double expirationTime = double.Parse(ConfigurationManager.AppSettings["UsersCacheExpiration"] ?? "15");

		public NotificationsController(QueueClient queueClient, MercadolibreApi mercadolibre)
		{
			this.queueClient = queueClient;
			this.mercadolibre = mercadolibre;
		}

		public async Task<IHttpActionResult> Post([FromBody]dynamic value)
		{
			this.Log(value);
			await SendIfEnabledUser(value);
			return Ok();
		}

		private async Task SendIfEnabledUser(dynamic value)
		{
			string userId = value.user_id;
			var shouldSend = await MessageIsFromEnabledUser(userId);
			if (shouldSend)
			{
				var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(value);
				var message = new BrokeredMessage(serialized);
				this.queueClient.Send(message);
			}
		}

		private async Task<bool> MessageIsFromEnabledUser(string userId)
		{
			try
			{
				var userInformation = await GetUserInformation(userId);
				return userInformation.enabled;
			}
			catch (Exception) //If there is any error
			{
				return true; //return true to send message anyway
			}
		}

		private async Task<UserInformation> GetUserInformation(string userId)
		{
			var userInformation = await cache.GetOrAddAsync(
				userId, 
				() => mercadolibre.GetUserInformationAsync(int.Parse(userId)),
				DateTimeOffset.Now.AddMinutes(expirationTime)
			);
			return userInformation;
		}

		private void Log(dynamic value)
		{
			try
			{
				string userId = value.user_id;
				string resource = value.resource;
				System.Diagnostics.Trace.TraceInformation("{0} {1}", userId, resource);
			}
			catch { }
		}
	}

}
