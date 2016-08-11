using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Storage.Queue;

namespace MeliMessageSender.Controllers
{
	[RoutePrefix("/notifications/merchantorders")]
	public class MerchantOrdersNotificationsController : NotificationsController
	{
		public MerchantOrdersNotificationsController(CloudQueue cloudQueue) : base(cloudQueue)
		{
		}

		[Route("{userId}"), HttpPost]
		public IHttpActionResult MerchantOrderMessage(long userId)
		{
			var value = new
			{
				resource = "/merchantorders/" + GetMerchantOrderId(),
				user_id = userId,
				topic = "merchantorders"
			};
			return this.LogValueAndAddMessage(value);
		}

		private string GetMerchantOrderId()
		{
			return Request.GetQueryNameValuePairs()
				.Where(param => param.Key == "merchant_order")
				.Select(param => param.Value)
				.FirstOrDefault();
		}
	}
}