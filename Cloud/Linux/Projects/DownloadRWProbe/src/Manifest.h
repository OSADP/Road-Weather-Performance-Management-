/*
 * Manifest.h
 *
 *  Created on: Sep 25, 2015
 *      Author: ivp
 */

#ifndef MANIFEST_H_
#define MANIFEST_H_

#include <string>

class Manifest
{
public:
	Manifest();
	virtual ~Manifest();

	// The output folder to write all probe data queried.
	std::string OutputFolder;

	// The number of seconds of back data to query.
	uint InitialDataSeconds;

	// The frequency in seconds to query for data.
	// All data since the last query is polled.
	uint QueryFrequencySeconds;
};

#endif /* MANIFEST_H_ */
