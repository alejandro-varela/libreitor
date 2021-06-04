# aspnet-core-3-jwt-authentication-api

ASP.NET Core 3.1 - JWT Authentication API

For documentation and instructions check out https://jasonwatmore.com/post/2019/10/11/aspnet-core-3-jwt-authentication-tutorial-with-example-api

get de users sin autenticarme:
	curl localhost:4000/users/ -kv

post para autenticarme:
	curl localhost:4000/users/authenticate -kv -X POST -d'{\"username\":\"ale\", \"password\":\"1234\"}'  -H"Content-Type: application/json" 
	
get de users con autenticación:
	curl localhost:4000/users/ -kv -H"Authorization: Bearer PONER_TOKEN_ACA"

get de users con autenticación trucha:
	curl localhost:4000/users/ -kv -H"Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJuYmYiOjE2MTk0NjEwMDQsImV4cCI6MTYyMDA2NTgwNCwiaWF0IjoxNjE5NDYxMDA0fQ.Ly9zJD-OHhzygWWtY692Z6gWZT3Eaq7XBY69J1x1Fgo"
