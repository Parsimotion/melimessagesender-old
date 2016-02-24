using System.Web.Http;
using Microsoft.WindowsAzure.Storage.Queue;

namespace MeliMessageSender.Controllers
{
    public class NotificationsController : ApiController
    {
		private readonly CloudQueue cloudQueue;

		public NotificationsController(CloudQueue cloudQueue)
		{
			this.cloudQueue = cloudQueue;
		}

        public IHttpActionResult Post([FromBody]dynamic value)
        {
			this.Log(value);
			string message = Newtonsoft.Json.JsonConvert.SerializeObject(value);
			var cloudQueueMessage = new CloudQueueMessage(message);
			this.cloudQueue.AddMessage(cloudQueueMessage);
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
