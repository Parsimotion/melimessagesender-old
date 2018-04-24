using System;
using System.Linq;
using System.Web.Http;
using Microsoft.ServiceBus.Messaging;

namespace MeliMessageSender.Controllers
{
    public class NotificationsController : ApiController
    {
	    private readonly QueueClient queueClient;

	    public NotificationsController(QueueClient queueClient)
		{
			this.queueClient = queueClient;
		}

        public IHttpActionResult Post([FromBody]dynamic value)
        {
			this.Log(value);
			var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(value);
			var message = new BrokeredMessage(serialized);
	        var ignoredProducts = Config.IgnoredProducts;
	        string resource = value.resource;
			if (ignoredProducts.Any(it => resource.Contains(it)))
		        return Ok();
			this.queueClient.Send(message);
	        return Ok();
        }

	    private void Log(dynamic value)
	    {
		    try
		    {
			    string userId = value.user_id;
			    string resource = value.resource;
			    System.Diagnostics.Trace.TraceInformation("{0} {1}", userId, resource);
		    }
		    catch {}
	    }
    }
}
