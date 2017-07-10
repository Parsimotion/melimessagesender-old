using System;
using System.Configuration;
using System.Threading.Tasks;
using MeliMessageSender.Models;
using RestSharp;

namespace MeliMessageSender.Services
{
	public class MercadolibreApi : BasicRestApi
	{
		public MercadolibreApi(string apiBaseUrl) : base(apiBaseUrl) { }

		public async Task<UserInformation> GetUserInformationAsync(int userId)
		{
			var request = BuildRequest(Method.GET, "/users/me");
			request.AddHeader("Authorization", GetAuthorizationToken(userId));
			var response = await ExecuteAuthorizedRequest(request);
			return DeserializeResponse<UserInformation>(response.Content);
		}

		private T DeserializeResponse<T>(string response)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(response);
		}

		private string GetAuthorizationToken(int userId)
		{
			var password = ConfigurationManager.AppSettings["MercadolibreMasterToken"];
			var credentials = $"{userId}:{password}";
			var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(credentials));
			return $"Basic {token}";
		}

		protected override void Authorize(RestRequest request)
		{
			request.AddQueryParameter("authenticationType", "mercadolibre");
		}
	}
}