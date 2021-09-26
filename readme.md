# Welcome to Site Activity Report 👋

> Website activity and reporting program that adds activity events by duration for the recent hours

## Summary

A Api to register and get a report of a website activity

## Usage

To start the Api run:
```sh
dotnet run --project src/CrossOver.WebsiteActivity/CrossOver.WebsiteActivity.csproj
```

The server will run at port `5001` and can be used through the following endpoints:

To register new activities
```
POST /activity/{key}
{
"value": 4
}
response:
(200)
{}
```


To check total value for a activity
```
GET /activity/{key}/total

response:
{
"value": 500
}
```



## Run tests

Run all the unit tests with

```sh
dotnet test tests/CrossOver.WebsiteActivity.Tests/CrossOver.WebsiteActivity.Tests.csproj --collect:\"XPlat Code Coverage\" --results-directory:TestResults
```

## Author

👤 **Victor Alves**

* Github: [@victoralvesp](https://github.com/victoralvesp)
* LinkedIn: [@victoralvesp](https://linkedin.com/in/victoralvesp)

## Show your support

Give a ⭐️ if this project helped you!

