# servicebus-message-poller

This repository contains 2 simple projects to play with Azure Service Bus Queues. 

## Message Poller Service

Is a Hosted Service that polls messages sent to a given Azure Service Bus Queue.

## Queue Sender

Is a simple console application that sends a few messages to the queue.

## How do I run these?

### Create a namespace and queue in the Azure Portal

This code uses a passwordless authentication mechanism, so you'll need to run az login at the terminal.

### Add the namespace and queue name to the appsettings.json files

{
  "AzureServiceBus": {
    "FullyQualifiedNamespace": "YOUR-SERVICE-BUS-NAMESPACE-HERE.servicebus.windows.net",
    "QueueName": "YOUR-QUEUE-NAME-HERE"
  }
}

### Set the MessagePollerService Project as the default one and run it.

### Optionally run the QueueSender to drop some messages to the queue.

This could also be done through the Azure Service Portal.

If everything's ok, the MessagePollerService Console will replay any messages sent to the queue.
