using System;
using System.Web.Http;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;

namespace MeliMessageSender.Controllers
{
    public class NotificationController : ApiController
    {

        public IHttpActionResult Get()
        {
			var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");

			var Client =
				QueueClient.CreateFromConnectionString(connectionString, "MeliNotifications");

			var message = Client.Receive(TimeSpan.FromSeconds(10));

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
			var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");

			var Client = QueueClient.CreateFromConnectionString(connectionString, "MeliNotifications");

	        var recordsMessage = Newtonsoft.Json.JsonConvert.SerializeObject(value);
			var message = new BrokeredMessage(recordsMessage);

			message.Properties["TestProperty"] = "TestValue";

			Client.Send(message);

        }
    }
}
