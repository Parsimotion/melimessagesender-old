using System;
using System.Web.Http;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;

namespace MeliMessageSender.Controllers
{
    public class NotificationsController : ApiController
    {
	    private readonly QueueClient queueClient;

	    public NotificationsController(QueueClient queueClient)
		{
			this.queueClient = queueClient;
		}

	    public IHttpActionResult Get()
        {
			var message = this.queueClient.Receive(TimeSpan.FromSeconds(10));

			if (message != null)
			{
				try
				{
					var notification = Newtonsoft.Json.JsonConvert.DeserializeObject(message.GetBody<string>());
					message.Complete();
					return Ok(notification);
				}
				catch (Exception)
				{
					message.Abandon();
					throw new ApplicationException();
				}
			}
	        return Ok();
        }

        public void Post([FromBody]dynamic value)
        {
	        var recordsMessage = Newtonsoft.Json.JsonConvert.SerializeObject(value);
			var message = new BrokeredMessage(recordsMessage);
			this.queueClient.Send(message);
        }
    }
}
