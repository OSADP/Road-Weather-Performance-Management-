/*
 * RWProbeWorker.cpp
 *
 *  Created on: Sep 22, 2015
 *      Author: ivp
 */

#include "RWProbeClient.h"

#include <iostream>
#include <iomanip>
#include <sstream>
#include <vector>

#include <boost/algorithm/string/predicate.hpp>

#include "cpprest/http_client.h"
//#include "cpprest/containerstream.h"
//#include "cpprest/filestream.h"
//#include "cpprest/producerconsumerstream.h"

using namespace std;
using namespace ::pplx;
using namespace utility;
using namespace concurrency::streams;
using namespace web;
using namespace web::http;
using namespace web::http::client;
using namespace web::json;

// The following namespace contains methods that are only used internal to this class.
// They do not need access to the class instance.
// This limits their visibility and keeps the header file much cleaner.

namespace RWProbeClient_Internal
{

const char* LOG_PREFIX = "RWProbeClient: ";

string FormatTime(string &t)
{
	// Time is in the format: "2015-11-28T16:00:00".
	// Change to format: YYYYMMDDHHMMSS.
	return t.substr(0,4) + t.substr(5,2) + t.substr(8,2) + t.substr(11,2) + t.substr(14,2) + t.substr(17,2);
}

void WriteField(ofstream &fs, json::object &probe, const char* key,
	bool isTime = false, bool addComma = true)
{
	if (probe[key].is_null())
	{
		fs << "-9999";
	}
	else
	{
		// For the most part, the value can be serialized straight to the stream generically
		// (I believe that json::value::serialize() is called).
		// However, some value must be converted to their actual type so that quotes are not added
		// or they can be converted to a different format.

		if (probe[key].is_string())
		{
			string str = probe[key].as_string();
			fs << (isTime ? FormatTime(str) : str);
		}
		else if (probe[key].is_boolean())
			fs << (probe[key].as_bool() ? "1" : "0");
		else
			fs << probe[key];
	}

	if (addComma)
		fs << ",";
}

void WriteField(ofstream &fs, json::object &probe, const char* key, const char* alternateKey,
	bool isTime = false, bool addComma = true)
{
	if (probe[key].is_null())
		WriteField(fs, probe, alternateKey, isTime, addComma);
	else
		WriteField(fs, probe, key, isTime, addComma);
}

// Convert the time value specified into a string suitable for querying using the web api.
// Example output: "2015-11-05 5:30:00".
std::string GetUtcDateTimeForQuery(time_t timeValue)
{
	tm* t = gmtime(&timeValue);
	std::ostringstream ss;

	ss << t->tm_year+1900 << "-";
	ss << setfill('0') << setw(2) << t->tm_mon+1 << "-";
	ss << setfill('0') << setw(2) << t->tm_mday << " ";
	ss << setfill('0') << setw(2) << t->tm_hour << ":";
	ss << setfill('0') << setw(2) << t->tm_min << ":";
	ss << setfill('0') << setw(2) << t->tm_sec;

	return ss.str();
}

std::string GetUtcDateTimeForFilename(time_t timeValue)
{
	tm* t = gmtime(&timeValue);
	std::ostringstream ss;

	ss << t->tm_year+1900 << "-";
	ss << setfill('0') << setw(2) << t->tm_mon+1 << "-";
	ss << setfill('0') << setw(2) << t->tm_mday << "_";
	ss << setfill('0') << setw(2) << t->tm_hour << ".";
	ss << setfill('0') << setw(2) << t->tm_min << ".";
	ss << setfill('0') << setw(2) << t->tm_sec;

	return ss.str();
}

}	// end namespace RWProbeClient_Internal

using namespace RWProbeClient_Internal;

// ****************** CLASS IMPLEMENTATION **********************

RWProbeClient::RWProbeClient(string outputFolder) :
	_outputFolder(outputFolder)
{

}

RWProbeClient::~RWProbeClient()
{

}

// Query for RWP probe data, writing values to a file.
// dataSinceTime - All probe data since this time is retrieved (when probeDatabaseId is -1).
// probeDatabaseId - The ID of the last database ID received.  All data after the specified ID is retrieved.
pplx::task<void> RWProbeClient::QueryToFileAsync(time_t dataSinceTime, int probeDatabaseId)
{
	// Create http_client to send the request.
	http_client client(U("http://rwpm.cloudapp.net/api/RWProbe/"));

	_lastQueryTime = time(0);

	cout << LOG_PREFIX << "Querying data    at UTC: " << GetUtcDateTimeForQuery(_lastQueryTime) << endl;

	// Build request URI.
	uri_builder builder(U("/Get"));

	if (probeDatabaseId == -1)
	{
		string utcDataSince = GetUtcDateTimeForQuery(dataSinceTime);
		builder.append_query(U("utcDataSince"), utcDataSince);
		cout << LOG_PREFIX << "Querying data since UTC: " << utcDataSince << endl;
	}
	else
	{
		builder.append_query(U("roadWeatherProbeInputsId"), probeDatabaseId + 1);
		cout << LOG_PREFIX << "Querying data  since ID: " << probeDatabaseId << endl;
	}

	// Manually build up an HTTP request with a header that specifies the content type and the request URI.
	http_request request(methods::GET);
	request.headers().set_content_type(U("application/json"));
	request.set_request_uri(builder.to_string());

	// Make an HTTP GET request and asynchronously process the response.
	return client.request(request)
		// The following code executes when the response is available
		.then([&](http_response response) -> pplx::task<json::value>
		{
			cout << LOG_PREFIX << "Received - Content Type: " << response.headers().content_type() << endl;
			cout << LOG_PREFIX << "Received - Content Length: " << response.headers().content_length() << " bytes" << endl;

			// If the status is OK extract the body of the response into a JSON value
			if (response.status_code() == status_codes::OK)
			{
				return response.extract_json();
			}
			else
			{
				// Return an empty JSON value.
				return pplx::task_from_result(json::value());
			}
		// Continue when the JSON value is available
		}).then([&](pplx::task<json::value> previousTask)
		{
			// Get the JSON value from the task and save the data to a file.
			try
			{
				json::value const & value = previousTask.get();
				SaveProbesToFile(value);
			}
			catch (http_exception const & e)
			{
				std::wcout << e.what() << std::endl;
			}
		});
}

void RWProbeClient::SaveProbesToFile(const json::value &v)
{
	if (v.is_null() || !v.is_array())
	{
		cout << LOG_PREFIX << "Invalid probes." << endl;
		return;
	}

	if (v.size() == 0)
	{
		cout << LOG_PREFIX << "No new probe data." << endl;
		return;
	}

	cout << LOG_PREFIX << "Saving probe data. Count: " << v.size() << endl;

	json::array probes = v.as_array();

	// Get a file name.
	if (!_outputFolder.empty() &&  !boost::algorithm::ends_with(_outputFolder, "/"))
		_outputFolder += "/";
	string filename = _outputFolder + "Probe_Data_" + GetUtcDateTimeForFilename(_lastQueryTime) + ".csv";

	// Open a file stream in overwrite mode (ios::app could be used to open in append mode).
	ofstream fs;
	fs.open (filename.c_str(), ios::out);

	// Write header for CSV file read by pikAlert.
	fs << "vehicle_id,time,latitude,longitude,heading,elevation,speed_mph,";
	fs << "air_temperature2,pressure,steering_angle,lights,wiper_status" << endl;

	for (auto probeIter = probes.cbegin(); probeIter != probes.cend(); probeIter++)
	{
		json::object probe = probeIter->as_object();

		if (probe["Id"].is_integer())
		{
			int id = probe["Id"].as_integer();
			if (id > LastDatabaseIdReceived)
				LastDatabaseIdReceived = id;
		}

		// Write fields in format expected by pikAlert.
		// Missing values are represented by -9999.

		// vehicle_id
		WriteField(fs, probe, "NomadicDeviceId");

		// time - Format: YYYYMMDDHHMMSS.
		WriteField(fs, probe, "DateGenerated", true);

		// latitude - Degrees North.
		WriteField(fs, probe, "GpsLatitude");

		// longitude - Degrees East.
		WriteField(fs, probe, "GpsLongitude");

		// heading - Degrees.
		WriteField(fs, probe, "GpsHeading");

		// elevation - Feet.
		WriteField(fs, probe, "GpsElevation");

		// speed_mph - Miles per hour.
		//             Speed is used if not null, otherwise GpsSpeed is used.
		WriteField(fs, probe, "Speed", "GpsSpeed");

		// air_temperature2 - Fahrenheit.
		WriteField(fs, probe, "AirTemperature");

		// pressure - hPa.
		WriteField(fs, probe, "AtmosphericPressure");

		// steering_angle - Degrees.
		WriteField(fs, probe, "SteeringWheelAngle");

		// lights - ParkingLightsOn = 128, FfogLightOn = 64, DaytimeRunningLightsOn = 32, AutomaticLightControlOn = 16,
		//          RightTurnSignalOn = 8, LeftTurnSignalOn = 4, HighBeamHeadlightsOn = 2, LowBeamHeadlightsOn = 1,
		//          HazardSignalOn = 24, AllLightsOff =  0.
		// Note that only true/false returned from DB.  So translate to 1 or 0 (whether low beams are on or off).
		WriteField(fs, probe, "HeadlightStatus");

		// wiper_status - Inactive = 0, Active = 1.
		WriteField(fs, probe, "WiperStatus", false, false);

		// Fields that come in JSON from cloud DB, but are not currently used:
		//
		// DateCreated - Date created by the DB cloud worker.  DateGenerated is used instead.
		// RightFrontWheelSpeed - No corresponding setting for pikAlert.
		// LeftFrontWheelSpeed - No corresponding setting for pikAlert.
		// LeftRearWheelSpeed - No corresponding setting for pikAlert.
		// RightRearWheelSpeed - No corresponding setting for pikAlert.

		// Field names that pikAlert could utilize, but that there are no corresponding values from DB.
		//
		// air_temperature - Fahrenheit temperature from canbus.
		// abs - notEquipped = 0, off = 1, on = 2, engaged = 3.
		// trac - notEquipped = 0, off = 1, on = 2, engaged = 3.
		// brake - 0 = inactive, 1 = active.
		// dewpoint - Fahrenheit.
		// humidity - Percent.
		// accel_lat - m/s^2.
		// accel_lon - m/s^2.
		// stab - notEquipped = 0, off = 1, on = 2; engaged = 3.
		// surface_temperature - Fahrenheit.
		// yaw_rate - Degrees per second.

		fs << endl;
	}

	fs.close();
}
