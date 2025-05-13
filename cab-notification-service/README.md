# Cab Post Service

## About The Project

Cab Post Service is a microservice that is built by .NET 5 and it take over a position in Cab VN system as post management system with functionalities: Manage posts, comments, reports, donates,...

Readmore about Cab VN project at: [Cab VN Deployment](https://gitlab.com/cabvn/cab-deployment/cabvn-deployment)

---

### Built With

* [.NET 5](https://devblogs.microsoft.com/dotnet/introducing-net-5/)
* [Cassandra](https://cassandra.apache.org/)

## Getting Started

This guide can help you run this service independently on local machine with .NET SDK.

### Prerequisites

- .NET 5 SDK or Docker 
- Cassandra server

### Installation

1. Navigate to Cab Post Service source 
   ```sh
   cd src/CabPostService
   ```
2. Build and run application
   ```sh
   dotnet run
   ```
3. API document will be explored at 
   > URL: https://localhost:9003/swagger
4. Stop application
   > press CTRL+C on the current cli

## Usage

- If the application can not work, please update the Cassandra connection string at `appsettings.json` by key `Cassandra`.

## License

This project belongs to Cab VN owner

## Contact

 - Developer: [Nam Phan](https://www.facebook.com/profile.php?id=100035765298647) - fsthanhnamsf@gmail.com
