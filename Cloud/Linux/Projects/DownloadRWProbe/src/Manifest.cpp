/*
 * Manifest.cpp
 *
 *  Created on: Sep 25, 2015
 *      Author: ivp
 */

#include "Manifest.h"

#include <fstream>
#include <iostream>
#include <string>
#include <sys/stat.h>

#include "cpprest/json.h"

using namespace std;
using namespace web;
using namespace web::json;

inline bool FileExists(const string& filename)
{
	struct stat buffer;
	return (stat (filename.c_str(), &buffer) == 0);
}

string GetSelfFullPath()
{
	char buffer[PATH_MAX];
	ssize_t length = ::readlink("/proc/self/exe", buffer, sizeof(buffer)-1);

	if (length != -1)
	{
		buffer[length] = '\0';
		return string(buffer);
	}

	return "";
}

string GetSelfFolder()
{
	string path = GetSelfFullPath();
	if (path.empty())
		return path;
	return path.substr(0, path.find_last_of("\\/"));
}

string GetFileBackOneFolder(string path)
{
	uint index = path.find_last_of("\\/");
	if (path.empty() || index == string::npos)
		return path;

	string folder = path.substr(0, index);
	uint index2 = folder.find_last_of("\\/");
	if (folder.empty() || index2 == string::npos)
		return path;

	return folder.substr(0, index2) + path.substr(index) ;
}

Manifest::Manifest() :
	OutputFolder(""),
	InitialDataSeconds(300),
	QueryFrequencySeconds(300)
{
	string selfPath = GetSelfFullPath();
	if (selfPath.empty())
	{
		cout << "Path to manifest not determined.  Using default settings." << endl;
		return;
	}

	string filename = selfPath + ".manifest";
	if (!FileExists(filename))
	{
		// Try to find the file one folder back in case it is running out of the Debug folder
		// and the manifest file is one folder back.
		string filename2 = GetFileBackOneFolder(filename);
		if (!FileExists(filename2))
		{
			cout << "Manifest not found: " << filename << endl;
			cout << "Using default settings." << endl;
			return;
		}
		filename = filename2;
	}

	cout << "Reading manifest." << endl;

	ifstream ifs(filename);
	string content((istreambuf_iterator<char>(ifs) ), (istreambuf_iterator<char>()));

	if (content.empty())
	{
		cout << "Manifest is empty: " << filename << endl;
		cout << "Using default settings." << endl;
		return;
	}

	//cout << "JSON:" << endl << content << endl;

	json::value settings = json::value::parse(content);

	if (!settings.is_object())
		return;

	if (settings["OutputFolder"].is_string())
		OutputFolder = settings["OutputFolder"].as_string();

	if (settings["QueryFrequencySeconds"].is_integer())
		QueryFrequencySeconds = settings["QueryFrequencySeconds"].as_integer();

	if (settings["InitialDataSeconds"].is_integer())
		InitialDataSeconds = settings["InitialDataSeconds"].as_integer();

}

Manifest::~Manifest()
{

}
