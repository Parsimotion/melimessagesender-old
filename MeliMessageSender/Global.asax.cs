using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MeliMessageSender
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			FormatterConfig.RegisterFormatter(GlobalConfiguration.Configuration.Formatters);			

		}
	}

	public static class FormatterConfig
	{
		public static void RegisterFormatter(MediaTypeFormatterCollection formatters)
		{
			formatters.Insert(0, JsonMediaTypeFormatter);
			formatters.Remove(formatters.XmlFormatter);
		}

		public static JsonMediaTypeFormatter JsonMediaTypeFormatter
		{
			get
			{
				var formatter = new JsonMediaTypeFormatter
				{
					SerializerSettings = new JsonSerializerSettings()
				};
				return formatter;
			}
		}
	}
}