TinyUrl generation service, similar to tinyurl.com

Running
================
Open in your favorite IDE and either:

* Run the included NUnit tests covering the required functionality below
* Run the Samples.TinyUrl.Console host to get a very simple REPL-like command interface

Functionality
================
Supports:
* Creating and Deleting aliases (short urls) from original values (long urls).
* Retrieving the original values (long url) mapped to a given alias (short url).
* Getting the hit count of the # of times a given alias (short url) has been retrieved.
* Creating custom/specified aliases (short urls) to a given original value (long url) if it is not already in use.
* Partitioning all of the above by a given group/domain (such that multiple organizations/domains/etc. may have overlapping aliases for example).

Fully thread safe currently - most thread safety is currently assumed in the storage layer (which is currently in-memory)

Architectural/Design Decisions
================
Not much to this currently, however generally it's been built to perform or allow performing in the future:

* Favoring retrieval speed over creation speed
* Extending with an actual persistent storage layer of some kind in various places without requiring a full/significant refactor

Restrictions
================
* Currently everything is transient/in-memory only (i.e. nothing persists across service restarts)
* There is currently nothing requiring or validating that URLs are valid, proper, etc.
  * By default we return actual URLs prepended with a 'default' base domain of 'https://surl.com/<alias>'
  * The "long url" you receive back will be the actual value initially sent - nothing verifies it's actually a URL
