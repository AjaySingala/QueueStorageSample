using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Queue; // Namespace for Queue storage types

namespace QueueStorageSample
{
    class Program
    {
        static CloudStorageAccount storageAccount;
        static CloudQueueClient queueClient;
        static CloudQueue queue;

        static void Main(string[] args)
        {
            try
            {
                // Parse the connection string and return a reference to the storage account.
                storageAccount = CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting("StorageConnectionString"));

                // Create the Queue service client
                queueClient = storageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a container.
                queue = queueClient.GetQueueReference("myqueue");

                // Create the queue if it doesn't already exist
                queue.CreateIfNotExists();

                InsertMessage("Hello world!");
                GetQueueLength();
                PeekMessage();
                //ChangeMessageConent();
                GetQueueLength();
                PeekMessage();
                DequeueMessage();
                InsertMessage("My name is Ajay Singala.");
                InsertMessage("How's the Josh?");
                GetQueueLength();
                PeekMessage();
                DequeueMessage();
                GetQueueLength();
                PeekMessage();
                GetQueueLength();
            }
            finally
            {
                DeleteQueue();
            }

            Console.WriteLine("Press <ENTER> to END...");
            Console.ReadLine();
        }

        private static void InsertMessage(string msg)
        {
            // Create a message and add it to the queue.
            CloudQueueMessage message = new CloudQueueMessage(msg);
            queue.AddMessage(message);
        }

        private static void PeekMessage()
        {
            // Peek at the message in the front of a queue
            // without removing it from the queue 
            CloudQueueMessage peekedMessage = queue.PeekMessage();

            // Display message.
            Console.WriteLine(peekedMessage.AsString);
        }

        private static void ChangeMessageConent()
        {
            // Get the message from the queue and update the message contents.
            CloudQueueMessage message = queue.GetMessage();
            message.SetMessageContent("Updated contents.");
            queue.UpdateMessage(message,
                TimeSpan.FromSeconds(10.0),  // Make it invisible for another 10 seconds.
                MessageUpdateFields.Content | MessageUpdateFields.Visibility);

            Console.WriteLine("ZZZZing for 10 seconds...");
            Thread.Sleep(10000);    // Sleep for 10 seconds.
        }

        // 2-steps
        // 1. Call GetMessage(), to get the next message in a queue.
        // A message returned from GetMessage becomes invisible to any other code 
        // reading messages from this queue.
        // By default, this message stays invisible for 30 seconds.
        // 2. To finish removing the message from the queue, call DeleteMessage().
        private static void DequeueMessage()
        {
            // Get the next message
            CloudQueueMessage retrievedMessage = queue.GetMessage();

            //Process the message in less than 30 seconds, and then delete the message
            queue.DeleteMessage(retrievedMessage);
            Console.WriteLine("{0} message deleted from queue", 
                retrievedMessage.AsString);
        }

        private static void GetQueueLength()
        {
            // Fetch the queue attributes.
            queue.FetchAttributes();

            // Retrieve the cached approximate message count.
            int? cachedMessageCount = queue.ApproximateMessageCount;

            // Display number of messages.
            Console.WriteLine("Number of messages in queue: " + cachedMessageCount);
        }

        private static void DeleteQueue()
        {
            // Delete the queue.
            queue.Delete();
            Console.WriteLine("Queue deleted...");
        }
    }
}
