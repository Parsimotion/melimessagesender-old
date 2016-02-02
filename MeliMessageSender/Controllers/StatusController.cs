using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Storage.Queue;

namespace MeliMessageSender.Controllers
{
	public class StatusController
	{
		[Route("status/redis")]
		public StatusDto Post()
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