using System.Threading.Tasks;
using System.Web.Http;
using MeliMessageSender.App_Start;
using Microsoft.WindowsAzure.Storage.Queue;

namespace MeliMessageSender.Controllers
{
    public class NotificationsController : ApiController
    {
		private readonly QueueService _queueService;
	    private readonly RedisService redisService;

	    public NotificationsController(QueueService queueService, RedisService redisService)
		{
			this._queueService = queueService;
			this.redisService = redisService;
		}

        public async Task<IHttpActionResult> Post([FromBody]Notification value)
        {
			this.Log(value);
	        if (!await redisService.IsUniqueNotification(value.resource)) return Ok();
	        var message = Newtonsoft.Json.JsonConvert.SerializeObject(value);
	        var cloudQueueMessage = new CloudQueueMessage(message);
	        this._queueService.AddMessage(cloudQueueMessage);
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

	public class Notification
	{
		public int user_id { get; set; }
		public string resource { get; set; }
		public string topic { get; set; }
		public string received { get; set; }
		public string sent { get; set; }
	}
}
