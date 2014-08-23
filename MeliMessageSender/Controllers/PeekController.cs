using System;
using System.Web.Http;
using Microsoft.ServiceBus.Messaging;

namespace MeliMessageSender.Controllers
{
	public class PeekController : ApiController
	{
		private readonly QueueClient queueClient;

		public PeekController(QueueClient queueClient)
		{
			this.queueClient = queueClient;
		}

		public IHttpActionResult Get()
		{
			var message = this.queueClient.Receive(TimeSpan.FromSeconds(1));

			if (message != null)
			{
				var notification = Newtonsoft.Json.JsonConvert.DeserializeObject(message.GetBody<string>());
				message.Abandon();
				return Ok(notification);
			}
			return Ok();
		}
	}
}
