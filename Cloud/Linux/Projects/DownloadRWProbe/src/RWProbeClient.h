/*
 * RWProbeWorker.h
 *
 *  Created on: Sep 22, 2015
 *      Author: ivp
 */

#ifndef RWPROBECLIENT_H_
#define RWPROBECLIENT_H_

#include <string>
#include "cpprest/json.h"

class RWProbeClient
{
public:
	RWProbeClient(std::string outputFolder);
	virtual ~RWProbeClient();

	pplx::task<void> QueryToFileAsync(time_t dataSinceTime, int probeDatabaseId);

	int LastDatabaseIdReceived = -1;

private:
	pplx::task<void> HttpGetAsync(time_t dataSinceTime);
	void SaveProbesToFile(const web::json::value &v);

	std::string _outputFolder;
	time_t _lastQueryTime = 0;
};

#endif /* RWPROBECLIENT_H_ */
