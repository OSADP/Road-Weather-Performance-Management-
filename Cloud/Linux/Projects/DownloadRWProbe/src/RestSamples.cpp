/*
 * RestSamples.cpp
 *
 *  Created on: Sep 22, 2015
 *      Author: ivp
 */

#include <cpprest/http_client.h>
#include <cpprest/filestream.h>

using namespace utility;                    // Common utilities like string conversions
using namespace web;                        // Common features like URIs.
using namespace web::http;                  // Common HTTP functionality
using namespace web::http::client;          // HTTP client features
using namespace concurrency::streams;       // Asynchronous streams

#include "RestSamples.h"

RestSamples::RestSamples()
{
	// TODO Auto-generated constructor stub

}

RestSamples::~RestSamples()
{
	// TODO Auto-generated destructor stub
}

// static.
void RestSamples::BingSearch()
{
	auto fileStream = std::make_shared<ostream>();

    // Open stream to output file.
	pplx::task<void> requestTask = fstream::open_ostream(U("results.html")).then([=](ostream outFile)
	{
		*fileStream = outFile;

		// Create http_client to send the request.
		http_client client(U("http://www.bing.com/"));

		// Build request URI and start the request.
		uri_builder builder(U("/search"));
		builder.append_query(U("q"), U("Casablanca CodePlex"));
		printf("Query: %s\n", builder.to_string().c_str());

		return client.request(methods::GET, builder.to_string());
	})

	// Handle response headers arriving.
	.then([=](http_response response)
	{
		printf("Received response status code:%u\n", response.status_code());

		// Write response body into the file.
		return response.body().read_to_end(fileStream->streambuf());
	})

	// Close the file stream.
	.then([=](size_t)
	{
    	return fileStream->close();
	});

	// Wait for all the outstanding I/O to complete and handle any exceptions
	try
	{
		requestTask.wait();
	}
	catch (const std::exception &e)
	{
		printf("Error exception:%s\n", e.what());
	}
}

// static.
void RestSamples::ProbeQuery()
{
	auto fileStream = std::make_shared<ostream>();

    // Open stream to output file.
	pplx::task<void> requestTask = fstream::open_ostream(U("results.html")).then([=](ostream outFile)
	{
		*fileStream = outFile;


		// Create http_client to send the request.
		http_client client(U("http://rwpm.cloudapp.net/api/RWProbe/"));

		// Build request URI and start the request.

		uri_builder builder(U("/Get"));
		builder.append_query(U("utcDataSince"), U("2015-11-05 5:30:00"));

		printf("Query: %s\n", builder.to_string().c_str());

		return client.request(methods::GET, builder.to_string());
	})

	// Handle response headers arriving.
	.then([=](http_response response)
	{
		printf("Received response status code:%u\n", response.status_code());

		// Write response body into the file.
		return response.body().read_to_end(fileStream->streambuf());
	})

	// Close the file stream.
	.then([=](size_t)
	{
    	return fileStream->close();
	});

	// Wait for all the outstanding I/O to complete and handle any exceptions
	try
	{
		requestTask.wait();
	}
	catch (const std::exception &e)
	{
		printf("Error exception:%s\n", e.what());
	}
}

