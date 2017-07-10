using System;
using System.Configuration;
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
		private readonly MercadolibreService mercadolibre;
		static readonly IAppCache cache = new CachingService();

		public NotificationsController(QueueClient queueClient, MercadolibreService mercadolibre)
		{
			this.queueClient = queueClient;
			this.mercadolibre = mercadolibre;
		}

		public IHttpActionResult Post([FromBody]dynamic value)
		{
			this.Log(value);
			SendIfEnabledUser(value);
			return Ok();
		}

		private void SendIfEnabledUser(dynamic value)
		{
			var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(value);
			var shouldSend = MessageIsFromEnabledUser(value.user_id);
			if (shouldSend)
			{
				var message = new BrokeredMessage(serialized);
				this.queueClient.Send(message);
			}
		}

		private bool MessageIsFromEnabledUser(string userId)
		{
			try
			{
				var userInformation = GetUserInformation(userId);
				return userInformation.Enabled;
			}
			catch (Exception) //If there is any error
			{
				return true; //return true to send message anyway
			}
		}

		private UserInformation GetUserInformation(string userId)
		{
			var expirationTime = ConfigurationManager.AppSettings["UsersCacheExpiration"] ?? "15";
			var userInformation = cache.GetOrAdd(
				userId,
				() => mercadolibre.GetUserInformation(int.Parse(userId)),
				DateTimeOffset.Now.AddMinutes(double.Parse(expirationTime))
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
