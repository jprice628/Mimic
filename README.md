# Introduction 
This project seeks to improve testing of microservices by providing a simple service virtualization tool that is programmable via HTTP.

Service virtualization is to microservice testing what mock, fake, dummy objects, etc. are to unit testing. It allows the tester the ability to isolate the service under test from its dependencies. This means that the tester no longer needs to configure working instances of dependencies in the test environment. Additionally, deployment pipelines don't get bottlenecked and disrupted by the need to have static test environments.

In a typical scenario, one without service virtualization, someone wanting to test a service needs to identify the service's dependencies, ensure that those dependencies are installed, and configure the dependencies to produce an outcome that fits the test case. This is time consuming, and it requires an extreme knowledge of the inner workings of each dependency. It can also have a very volatile characteristic. If any of the dependencies change, new knowledge must be acquired, the test case must be updated, and the test environment reconfigured.

Now lets assume that we have more than one team working attempting to test their own service. There are a couple of options without virtualization. 
1) Have separate distinct test environments for each team. This is excessively expensive considering that all dependencies have to be installed and maintained in all test environments. 
2) Deal with the consequences of teams interacting with each other. What happens when two team try to use the same data? What happens when two teams require a different configuration of the same dependency?
3) Make each team queue up for the test environment, and only allow one team at a time to use it. This obviously doesn't scale well and is very inefficient.

None of these are good options, so lets do some service virtualization instead...

# Getting Started

## Installation
Currently, the project has a CI/CD pipeline, so as developers change the code, it is automatically built, tested, and deployed to a host where it can be accessed.

This severely limits the ability to use the tool, so changes will be coming soon that allow it to be consumed in other ways.

## Latest Releases
- Build 90
    - Rebranded from "VirtualService" to Mimic.
    - Released on [Dockerhub](https://hub.docker.com/r/jprice628/mimic/)
- Build 83
    - No new operational features.
    - Cleaned up using statements in the code.
    - Reviewed code comments.
- Build 83
    - Fixed several places in the code where string matching was case-sensitive and should not have been.
    - Removed some old placeholder classes and tests.
    - Addressed many of the TODO's left in the code.
- Build 81
    - Added functional testing to the deployment pipeline.
- Build 77
    - Added functional test project and tests for basic service operations.
- Build 75
    - Fixed bug where services wouldn't match on URL encoded requests.
- Build 74
    - Added the ability to query a virtual service to see its stats and the last request that was made to it.

# Basic Usage

Once Mimic is deployed, it can be programmed using your favorite HTTP tool. If you don't have a tool, you can try [Postman](https://www.getpostman.com/)

## Adding a Service
To add a new virtual service, POST it to http://localhost:51769/__vs/services. Be careful not to post services that have the same IDs or method-path combination. Doing so will result in a BadRequest message. When your service is added properly, you will receive an OK response with the ID of your new service. The following is the body of the POST the post message for a sample service.
        
    # You can write comments in your body by prefixing the line with a hash.

    ## Basics ##
    
    # You don't have to set an ID. If you don't, one will be generated for you.
    Id: 7cfddc3e-7ec8-444c-82b9-cbe83a134fce
    
    ## Request ##    
    # This section defines the requests that your service will respond to.
    
    Method: POST
    Path: /api/things/53
    
    ## Response ##
    # This section defines how your service will respond.

    ContentType: application/json
    StatusCode: 200
    
    # The body "comment" below is special. Everything that follows it
    # will be interpretted as the body of your response message.
    # Body
    {
        "color":"Red",
        "shape":"round"
    }

## Invoking a Service
Simply send you HTTP request using the method and path you defined when adding your service. For the example above

    POST http://localhost:51769/api/things/53

The service will respond as defined.

## Querying a Service
Querying a service allows you to see whether or not the service has been invoked, how many times it has been invoked, and the body of the last request message that invoked it. To query the service above, use the service's ID as in the following:

    GET http://localhost:51769/__vs/services/7cfddc3e-7ec8-444c-82b9-cbe83a134fce

This will return a response like this

    CallCount: 3

    LastRequestBody:
    {
        "message":"Lorem ipsum dolor sit amet..."
    }

## Deleting a Service
Once you are done testing, you might want to clean up. You can delete a service using its ID.

    DELETE http://localhost:51769/__vs/services/7cfddc3e-7ec8-444c-82b9-cbe83a134fce

You can also delete all services like this

    DELETE http://localhost:51769/__vs/services