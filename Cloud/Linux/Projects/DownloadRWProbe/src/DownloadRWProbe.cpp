//============================================================================
// Name        : DownloadRWProbe.cpp
// Author      : 
// Version     :
// Copyright   : Your copyright notice
// Description : Hello World in C++, Ansi-style
//============================================================================

#include <stdlib.h>
#include <iostream>
#include <sstream>
#include <vector>
#include "RestSamples.h"
#include "RWProbeClient.h"
#include "Manifest.h"

using namespace std;

//const string _outputFolder = "/d2/rw_probe_data";
//const string _outputFolder = "/home/ivp/RWPM/output";

int main(int argc, char* argv[])
{
	//RestSamples::BingSearch();
	//RestSamples::ProbeQuery();

	// Read manifest file.
	Manifest manifest;

	RWProbeClient rwProbeWorker(manifest.OutputFolder);

	// Send time of next query to now.
	time_t timeOfNextQuery = time(0);

	// On the first query, get all data for the number of seconds specified in the manifest.
	time_t dataSinceTime = time(0) - manifest.InitialDataSeconds;

	bool isFirst = true;
	pplx::task<void> queryTask;

	while (true)
	{
		// Wait for previous query to complete if it is not already done.
		while (!isFirst && !queryTask.is_done())
		{
			sleep(1);
		}

		// Wait until is time to query again.
		while (time(0) < timeOfNextQuery)
		{
			sleep(1);
		}

		time_t now = time(0);

		int lastId = -1;
		if (!isFirst) lastId = rwProbeWorker.LastDatabaseIdReceived;

		// Query for data, writing to a file.
		queryTask = rwProbeWorker.QueryToFileAsync(dataSinceTime, lastId);

		isFirst = false;

		// Set the next query time to the number of seconds in the future specified by the manifest setting.
		timeOfNextQuery = now + manifest.QueryFrequencySeconds;

		// On the next query, get all data since the last query.
		dataSinceTime = now;
	}

	return 0;
}
