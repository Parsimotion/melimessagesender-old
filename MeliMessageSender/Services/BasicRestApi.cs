using System.Threading.Tasks;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace MeliMessageSender.Services
{
	public abstract class BasicRestApi
	{
		protected readonly RestClient RestClient;

		protected BasicRestApi(string apiBaseUrl)
		{
			this.RestClient = new RestClient(apiBaseUrl);
			this.RestClient.ClearHandlers();
			this.RestClient.AddHandler("*", new JsonDeserializer());
		}

		protected abstract void Authorize(RestRequest request);
		protected virtual async Task<IRestResponse> ExecuteAuthorizedRequest(RestRequest request)
		{
			this.Authorize(request);
			var response = await this.RestClient.ExecuteTaskAsync(request);
			return response;
		}

		protected T ExecuteAuthorized<T>(Method method, string resource, object body = null) where T : new()
		{
			var request = this.BuildRequest(method, resource, body);
			this.Authorize(request);
			var respose = this.RestClient.Execute<T>(request);
			return respose.Data;
		}

		protected virtual RestRequest BuildRequest(Method method, string resource, object body = null, params Parameter[] parameters)
		{
			return this.BuildRequest(new JsonSerializer(), method, resource, body, parameters);
		}

		protected RestRequest BuildRequest(ISerializer serializer, Method method, string resource, object body, params Parameter[] parameters)
		{
			var request = new RestRequest(method)
			{
				RequestFormat = DataFormat.Json,
				Method = method,
				Resource = resource,
				JsonSerializer = serializer
			};
			request.AddHeader("Accept", "application/json");
			if (body != null) request.AddBody(body);
			foreach (var parameter in parameters)
			{
				request.AddParameter(parameter);
			}

			return request;
		}
	}
}