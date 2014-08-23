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

        public void Post([FromBody]dynamic value)
        {
	        var recordsMessage = Newtonsoft.Json.JsonConvert.SerializeObject(value);
			var message = new BrokeredMessage(recordsMessage);
			this.queueClient.Send(message);
        }
    }
}
