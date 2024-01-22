Backend Coding Challenge

Overview

The goal of the project is to implement two endpoints, which are:

    An endpoint that takes a long URL as input and returns a shortened version of that URL.
    An endpoint that can be accessed through the shortened URL and redirects the user to the original long URL.

Requirements

    The shortened URL should be a unique key that can be used to retrieve the original long URL.
    The shortened URL should be accessible through a web browser.
    The shortened URL should redirect the user to the original long URL.

System Requirements

    A functional installation of the .NET Core 7.0 SDK
    A code editor you're comfortable with.

Getting Started

    Clone the project repository
    Run the application with your IDE of choice or by running dotnet run --project ./src/Api/Api.csproj in the project root directory in a terminal.
    Implement the required endpoints. Basic scaffolding for them already exists in the <project_root>/src/Api/Endpoints/Url directory.
    Test your implementation using a web browser or a tool like curl

## Dev

### 1. Endpoints
#### 1.1 -> CreateShortUrl
AA. CC.
1. Creates a entry on the db and hash the newly created Id returning it to the consumer
2. Same website www.x.com -> will be stored x times with different ids
3. Requires AuthData.WriterPolicy for authorization
4. Validates:
    - payload exists
    - payload is url


#### 1.2 -> RedirectToUrlCommand
1. Requires no authorization
2. If no result is found will return not found 404
3. If id exists the redirection is immediate


### 2. Authorization and Authentication
In order to leverage the dotnet user-jwts to create tokens for the app:
``` 
dotnet user-jwts create --name WriterYes --claim "writer=true"
``````
This tool will create a token for user named WriterYes having a custom claim: writer = true

### 3. DB and Caching
A cache is in place with redis to alleviate get stress on the db
The cache invalidation is done just trough TTL of the item on 5 minutes

### 4. Metrics
Metrics are collected by prometheus on a scrape interval of 10s. 
For the sake of this project just a histogram was created for collecting data times used

### 5. Database migrations
```
    dotnet ef migrations add MigrationName
    dotnet ef database update
```

