param location string = resourceGroup().location
param serviceBusNamespaceName string = 'consoles-sandbox'
param requestTopicName string = 'request-topic'
param responseTopicName string = 'response-topic'
param requestSubscriptionName string = 'request-subscription'
param responseSubscriptionName string = 'response-subscription'

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' = {
  name: serviceBusNamespaceName
  location: location
  sku: {
    name: 'Standard'  // Change to 'Basic' or 'Premium' as per your requirement
    tier: 'Standard'
  }
}

resource requestTopic 'Microsoft.ServiceBus/namespaces/topics@2022-10-01-preview' = {
  parent: serviceBusNamespace
  name: requestTopicName
  properties: {
    defaultMessageTimeToLive: 'P14D'  // 14 days
    maxSizeInMegabytes: 1024  // 1 GB
  }
}

resource responseTopic 'Microsoft.ServiceBus/namespaces/topics@2022-10-01-preview' = {
  parent: serviceBusNamespace
  name: responseTopicName
  properties: {
    defaultMessageTimeToLive: 'P14D'  // 14 days
    maxSizeInMegabytes: 1024  // 1 GB
  }
}

resource responseSubscription 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2022-10-01-preview' = {
  parent: responseTopic
  name: responseSubscriptionName
  properties: {
    defaultMessageTimeToLive: 'P14D'  // 14 days
    maxDeliveryCount: 10
    deadLetteringOnMessageExpiration: true
  }
}

resource requestSubscription 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2022-10-01-preview' = {
  parent: requestTopic
  name: requestSubscriptionName
  properties: {
    defaultMessageTimeToLive: 'P14D'  // 14 days
    maxDeliveryCount: 10
    deadLetteringOnMessageExpiration: true
  }
}

output requestTopicName string = requestTopic.name
output responseTopicName string = responseTopic.name
output requestSubscriptionName string = requestSubscription.name
output responseSubscriptionName string = responseSubscription.name
