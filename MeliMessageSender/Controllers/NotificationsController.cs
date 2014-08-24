﻿using System;
using System.Web.Http;
using Microsoft.ServiceBus.Messaging;

namespace MeliMessageSender.Controllers
{
    public class NotificationsController : ApiController
    {
	    private readonly QueueClient queueClient;

	    public NotificationsController(QueueClient queueClient)
		{
			this.queueClient = queueClient;
		}

        public void Post([FromBody]dynamic value)
        {
			this.Log(value);
			var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(value);
			var message = new BrokeredMessage(serialized);
			this.queueClient.Send(message);
        }

	    private void Log(dynamic value)
	    {
		    try
		    {
			    string userId = value.user_id;
			    string resource = value.resource;
			    System.Diagnostics.Trace.TraceInformation("{0} {1}", userId, resource);
		    }
		    catch {}
	    }
    }
}
