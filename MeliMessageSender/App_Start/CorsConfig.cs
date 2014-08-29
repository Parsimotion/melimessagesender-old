using System.Web.Http;
using Thinktecture.IdentityModel.Http.Cors.WebApi;

namespace MeliMessageSender.App_Start
{
	public static class CorsConfig
	{
		public static void RegisterCors(HttpConfiguration httpConfig)
		{
			var corsConfig = new WebApiCorsConfiguration { DefaultCacheDuration = 15000 };
			corsConfig.AllowAll();
			corsConfig.RegisterGlobal(httpConfig);
		}
	}
}