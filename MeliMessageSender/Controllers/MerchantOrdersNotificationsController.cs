using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Storage.Queue;

namespace MeliMessageSender.Controllers
{
	public class MerchantOrdersNotificationsController : NotificationsController
	{
		public MerchantOrdersNotificationsController(CloudQueue cloudQueue) : base(cloudQueue)
		{
		}

		public IHttpActionResult Post(long id)
		{
			if (!IsMerchantOrder()) return this.Ok();

			var userId = long.Parse(Request.RequestUri.Segments[2]);
			var merchantOrderId = id;
			var value = new
			{
				resource = "/merchantorders/" + merchantOrderId,
				user_id = userId,
				topic = "merchantorders"
			};

			return this.LogValueAndAddMessage(value);
		}

		private bool IsMerchantOrder()
		{
			return Request.GetQueryNameValuePairs()
				.Any(param => param.Key == "topic" && param.Value == "merchant_order");
		}
	}
}