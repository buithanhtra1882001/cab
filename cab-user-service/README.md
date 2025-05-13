# Cab User Service

## About The Project

Cab User Service is a microservice that is built by .NET 5 and it take over a position in Cab VN system as user management system with functionalities: Manage user profile, user friend interaction, chatting...

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

1. Navigate to Cab User Service source 
   ```sh
   cd src/CabUserService
   ```
2. Build and run application
   ```sh
   dotnet run
   ```
3. API document will be explored at 
   > URL: https://localhost:9002/swagger
4. Stop application
   > press CTRL+C on the current cli

## Usage

- If the application can not work, please update the Cassandra connection string at `appsettings.json` by key `Cassandra`.

## License

This project belongs to Cab VN owner

## Contact

 - Ex-developer: [Au Nguyen](https://www.facebook.com/AuNguyenCNTT) - quocauk54cntt@gmail.com
 - Developer: Nguyen Dinh Khanh
 - Developer: Ha Duy
