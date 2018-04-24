using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MeliMessageSender
{
	public static class Config
	{
		private static string GetSetting(string property) { return ConfigurationManager.AppSettings[property]; }
		public static string[] IgnoredProducts => GetSetting("IGNORED_PRODUCTS").Split(';');
	}
}