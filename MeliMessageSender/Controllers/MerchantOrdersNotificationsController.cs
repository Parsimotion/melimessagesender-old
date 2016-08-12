using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Storage.Queue;

namespace MeliMessageSender.Controllers
{
	//[RoutePrefix("/notifications/merchantorders")]
	public class MerchantOrdersNotificationsController : NotificationsController
	{
		public MerchantOrdersNotificationsController(CloudQueue cloudQueue) : base(cloudQueue)
		{
		}

		//[Route("{userId}"), HttpPost]
		public IHttpActionResult Post(long id)
		{
			var merchantOrderId = GetMerchantOrderId();
			if (string.IsNullOrEmpty(merchantOrderId)) return this.Ok();

			var value = new
			{
				resource = "/merchantorders/" + GetMerchantOrderId(),
				user_id = id,
				topic = "merchantorders"
			};

			return this.LogValueAndAddMessage(value);
		}

		private string GetMerchantOrderId()
		{
			var queryString = Request.GetQueryNameValuePairs();
			if (!queryString.Any(param => param.Key == "topic" && param.Value == "merchant_order")) return "";

			return queryString
				.Where(param => param.Key == "id")
				.Select(param => param.Value)
				.First();
		}
	}
}