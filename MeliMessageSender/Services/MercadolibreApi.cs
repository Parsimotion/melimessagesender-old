using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MeliMessageSender.Models;
using RestSharp;

namespace MeliMessageSender.Services
{
	public class MercadolibreApi : BasicRestApi
	{
		public MercadolibreApi(string apiBaseUrl) : base(apiBaseUrl) { }

		public UserInformation GetUserInformation(int userId)
		{
			throw new NotImplementedException();
		}

		protected override void Authorize(RestRequest request)
		{
			throw new NotImplementedException();
		}
	}
}