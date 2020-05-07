using System;
using System.Collections;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace SampleQueueConsoleApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Environment.SetEnvironmentVariable(
                "AZURE_STORAGE_CONNECTION_STRING",
                "<placeholder>"
            );

            var connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            Console.WriteLine("Queue connection string: " + connectionString ?? "null");

            var queueName = "colins-sample-queue";
            Console.WriteLine("Queue name: " + queueName + "\n");

            //QueueServiceClient queueServiceClient;

            QueueClient queueClient = new QueueClient(connectionString, queueName);

            await peekTheQueue(queueClient);


            // programmatically add messages to the queue
            Console.WriteLine("Adding 3 messages to the queue...");
            await queueClient.SendMessageAsync("A programmatically inserted queue message.");
            await queueClient.SendMessageAsync("A second programmatically inserted queue message.");
            await queueClient.SendMessageAsync("A third programmatically inserted queue message.");
            Console.WriteLine("Messages added.\n");

            await peekTheQueue(queueClient);

            // receive one message
            QueueMessage[] messagesReceived = await queueClient.ReceiveMessagesAsync(maxMessages: 1);

            await peekTheQueue(queueClient);

            /*
             *      DO PROCESS LOGIC FOR MESSAGES RECEIVED
             */
            foreach(var msg in messagesReceived)
            {
                Console.WriteLine($"Processed message {msg.MessageId}.");
            }

            await peekTheQueue(queueClient);

            foreach(var msg in messagesReceived)
            {
                await queueClient.DeleteMessageAsync(msg.MessageId, msg.PopReceipt);
            }

            await peekTheQueue(queueClient);

            Console.WriteLine("End of demo.  Thanks!\n");
        }

        /// <summary>
        /// Peek at messages in the queue!
        /// </summary>
        private static async Task peekTheQueue(QueueClient queueClient)
        {
            Console.WriteLine("Peeking at the queue:");

            PeekedMessage[] peekedMessages = await queueClient.PeekMessagesAsync(maxMessages: 10);
            foreach (PeekedMessage msg in peekedMessages)
            {
                Console.WriteLine($"\t ID: {msg.MessageId}, \t Text: {msg.MessageText}");
            }

            Console.WriteLine("Done peeking.\n");
        }
    }
}
