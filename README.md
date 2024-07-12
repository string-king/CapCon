# Capital Connect

Created as a project for Web Applications with C# course and JavaScript course.

CapCon is a platform designed to facilitate lending between companies and individuals. Companies can request loans, and users can offer loans to fulfill those requests. Once a loan is accepted, companies can manage their payments and track their loan status. For more info, refer to the project proposal PDF.

## Running the App

1. **Compose SQL Service:**
   - Navigate to the project directory and use Docker to start the SQL service:
     ```sh
     docker-compose up -d sql
     ```

2. **Start the Backend:**
   - Inside the `WebApp` directory, run the following command to start the backend:
     ```sh
     dotnet run
     ```
   - This will launch the backend and a simple version of the ASP.NET pages.

3. **Install Frontend Dependencies:**
   - Navigate to the `frontend` directory and install the required npm packages:
     ```sh
     npm install
     ```

4. **Start the Frontend:**
   - In the `frontend` directory, start the frontend client:
     ```sh
     npm run dev
     ```

## API Documentation

The API documentation is accessible through Swagger while the backend is running. Visit the following URL (replace `backendport` with your actual backend port):

```sh
https://localhost:backendport/swagger/
```


## Technologies Used
- Vue with TypeScript
- Docker
- .NET
- EF Core
- Swagger
- JWT
- PostgreSQL

## Author
Robert King, 2024
