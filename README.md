# RateLimiting
Rate-limiting module that stops a particular requestor from making too many http requests within a particular period of time.

I chose to leave the check on an interface to be easily used anywhere on the system. It has optional parameters with default values.

With each request that the user makes it is possible to identify the endpoint through the RequestName parameter and the user through his ip, this information is saved in memory and analyzed in each call.
If the user makes calls within the allowed limit, for this check there is a counter that stores the number of attempts made, he is successful in his requests, otherwise he receives error 429.
