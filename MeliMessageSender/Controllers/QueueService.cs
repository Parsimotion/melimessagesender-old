using Microsoft.WindowsAzure.Storage.Queue;

namespace MeliMessageSender.Controllers
{
	public class QueueService
	{
		private readonly CloudQueue _cloudQueue;

		public QueueService(CloudQueue cloudQueue)
		{
			_cloudQueue = cloudQueue;
		}

		public virtual void AddMessage(CloudQueueMessage message)
		{
			_cloudQueue.AddMessage(message);
		}
	}
}