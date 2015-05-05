using System.Web.Http;
using Microsoft.WindowsAzure.Storage.Queue;

namespace MeliMessageSender.Controllers
{
	public class PeekController : ApiController
	{
		private readonly CloudQueue queueClient;

		public PeekController(CloudQueue queueClient)
		{
			this.queueClient = queueClient;
		}

		public IHttpActionResult Get()
		{
			var message = this.queueClient.PeekMessage();

			if (message != null)
			{
				return Ok(message);
			}
			return Ok();
		}
	}
}
