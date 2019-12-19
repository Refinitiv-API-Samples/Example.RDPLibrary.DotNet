## Overview

The *Delivery* layer provides interfaces to retrieve streaming data, static data via request/reply services, real-time alerts through cloud-based queues as well as bulk data through file services.  While the Delivery interfaces bring greater control and flexibility, they still offer easy to use, intuitive features to the developer.  As such, this layer targets professional developers but can also be used by citizen developers with good programming skills.

The following is a list of examples using the Delivery layer within the API.  Each example targets one or more access channels that support that capability.  Refer to the GlobalSettings/Session.cs within the package for more details.

### Streaming 

The streaming examples focus on both basic and advanced usage of the streaming data services within the platform. 

- **MarketPrice**
  At the most basic level, retrieve level 1 streaming market data from the platform.
- **Collection**
  Enhance your simple example to specify multiple items within a batch.  For each item, define the specific fields of interest.
- **MarketByPrice**
  Retrieve a more advanced, level 2 market data item by specifying interest for a streaming price book from the platform.

### Request/Reply 

The Request/Reply examples refer to data services that utilize Refinitiv's Data Platform endpoint functionality.

- **Endpoint**
  Introductory example showing how to retrieve historical pricing data points.

- **Endpoint_Params**
  Demonstrate how to retrieve a news headline and and its associated story text.

### Queues

Utilizing alert mechanisms within the platform, retrieve messages using underlying AWS Message Queue technology.

- **Headline_Alerts**
  Delivers real-time news headline alerts to the console.

  