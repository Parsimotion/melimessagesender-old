using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Storage.Queue;

namespace MeliMessageSender.Controllers
{
	[RoutePrefix("status")]
	public class StatusController : ApiController
	{
		[Route("redis")]
		public StatusDto Get()
		{
			var status = RedisService.RedisStatus ? "active" : "down";
			return new StatusDto{Status = status};
		}
	}

	public class StatusDto
	{
		public virtual string Status { get; set; }
	}
}