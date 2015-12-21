Application Name:
Road Weather Performance Management (RW-PM)
Version Number: 
v1.0 

Primary Functions:
RoadWeatherMobileApp - Collected mobility data for use by the RW-PM system, and display traffic mobility and road weather alerts to drivers.
Cloud - Collect data from the RoadWeatherMobileApp, and share it with two subsystems: Pikalert and INFLO.  The Cloud RW-PM system will receive processed output from Pikalert including
road weather information, road treatment recommendations, and pavement conditions.  The Cloud RW-PM system will receive traffic mobiltity information from the INFLO system including recommended
speed information and traffic backup locations.

Hardware Supported:
Cloud Based Services: Microsoft Azure
RoadWeatherMobileApp: Android 54.0+

Installation Instructions:
There is no installer included with this package. Only Source code is provided.

Description of the project:
The tool captures and analyzes applicable traffic mobility and road weather data continuously from multiple sources. This includes capturing road weather data from fixed weather 
stations (e.g., environmental sensor stations) and traffic mobility data from fixed speed sensors (e.g., inductive loop detectors) and/or subscription services. 
The Tool is capable of capturing also captures  data from mobile sources such as road weather maintenance vehicles, connected vehicles, and drivers’ personal mobile devices. 
The data are transmitted wirelessly and stored in suitable traffic mobility, road weather, and RW-PM databases.  The captured raw data are next analyzed by multiple processors 
operating in parallel and in real-time. One set of processors analyzes the raw data to compute local real-time traffic mobility and road weather performance “measures and metrics” 
and stores them in a “measures and metrics” database. Another set of processors analyzes the raw data and performance measures and metrics to develop real-time traffic control, 
road weather maintenance dispatch and motorist advisory recommendations. Traffic control and road weather maintenance managers personnel are alerted when DOT criteria for a 
weather event likely to degrade level of service (LOS) are met. Upon verification and authorization by the appropriate personnelmanagers: 
•	Control and dispatch recommendations are deployed to traffic management and road weather maintenance systems; 
•	Motorist advisories are deployed to connected vehicles and personal mobile devices; and
•	Road weather maintenance dispatch information and messages are issued to connected maintenance vehicles. 
This process continues iteratively throughout the weather event, adjusting dynamically until the LOS returns to normal (e.g. the event concludes, the roads are cleared, 
and traffic mobility returns to normal). Following the event, system performance processors aggregate the data, measures, and metrics and assess performance and
 effectiveness outcomes for the entire event. The outcomes are then used by traffic control and road weather maintenance managers personnel to refine and optimize 
 RW-PM strategies for implementation in responding to future weather events. System performance is evaluated seasonally and/or annually and strategies are updated 
 to enhance seasonal and annual performance.

Category:
Traffic Management

Subcategory:
Transportation

Suggested System requirements:
IDTO MDT Android Mobile Device:
Android Phone running Android 5.0+

Azure:
Cloud Services, Web Sites, and Web API

