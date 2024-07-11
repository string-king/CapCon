# Capital Connect

Created as a project for Web Aplications with C# course and Javascript course.

CapCon is a platform designed to facilitate lending between companies and individuals. Companies can request loans, and users can offer loans to fulfill those requests. Once a loan is accepted, companies can manage their payments and track their loan status. For more info refer to project proposal pdf.


## Running the app.

1. Compose sql service in docker-compose.yml with Docker using command: `docker-compose up -d sql`.
2. Inside WebApp directory in backend run command `dotnet run` - backend starts to run and simple ASP.NET pages version launches.
3. Inside the frontend directory run command `npm install`.
4. Inside the frontend directory run command `npm run dev` - frontend client starts to run.

## API documentation

API documentation is accessible through Swagger while backend is running.
```
https://localhost:backendport/swagger/
```

## Technologies used
* Vue with Typescript
* Docker
* .NET
* EF Core
* Swagger
* JWT
* Postgres


## Author
Robert King, 2024