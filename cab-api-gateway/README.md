# Cab API Gateway

## About The Project

Cab API Gateway is a microservice that is built by .NET 5 and it take over a position in Cab VN system as a router with centralize authentication, besides logging, rate limiting or transform requests.

Readmore about Cab VN project at: [Cab VN Deployment](https://gitlab.com/cabvn/cab-deployment/cabvn-deployment)

---

### Built With

- [.NET 5](https://devblogs.microsoft.com/dotnet/introducing-net-5/)
- [Ocelot](https://threemammals.com/ocelot/)

## Getting Started

This guide can help you run this service independently on local machine with .NET SDK.

### Prerequisites

- .NET 5 SDK or Docker

### Installation

1. Navigate to Cab API Gateway source
   ```sh
   cd src/CabApiGateway
   ```
2. Build and run application
   ```sh
   dotnet run
   ```
3. API document will be explored at
   > URL: https://localhost:9000/swagger
4. Stop application
   > press CTRL+C on the current cli

## Usage

- Routing configurations can be found at `OcelotRoutes.Development`.
- This application can work as swagger proxy, so it can load microservices' swagger, the configuration can be found at `appsettings.json` by key `GroupSwaggerConfigs`.

### How to add new Ocelot upstream for a new service

1. Create new `ocelot.<service-name>.json` file in `OcelotRoutes.Development` and `OcelotRoutes`.

2. Add new swager mapping in `appsettings.json`. Example:

```
{
   "SwaggerName": "Payment Service V1",
   "SwaggerEndpoint": "http://localhost:9004/swagger/v1/swagger.json",
   "UpstreamTemplate": "/{version}/payment-service/{everything}",
   "DownstreamTemplate": "/api/{version}/{everything}"
}
```

3. Add new enviroment in `docker-composer.override.yaml`. Example below:

```
- GroupSwaggerConfigs**4**SwaggerName=CabPaymentService V1
- GroupSwaggerConfigs**4**SwaggerEndpoint=http://cab-payment-service/swagger/v1/swagger.json
```

## License

This project belongs to Cab VN owner

## Contributors:

- Orginal developer / Ex-developer: [Au Nguyen](https://www.facebook.com/AuNguyenCNTT) - quocauk54cntt@gmail.com
- Ex-developer: Phan Thành Nam
- Ex-developer: Nguyễn Hạ Duy
- Developer: Nguyễn Đình Khánh
