# EduSync Backend API

A .NET 8.0 API backend for the EduSync education platform, providing authentication, course management, and assessment functionality.

## Technologies Used

- ASP.NET Core 8.0
- Entity Framework Core
- JWT Authentication
- Azure SQL Database
- Azure Blob Storage

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (local or Azure)
- Azure Storage Account (for production)

### Setup Instructions

1. Clone the repository
   ```
   git clone https://github.com/Simranbansal03/edusync_backend.git
   cd edusync_backend
   ```

2. Configure your environment
   - Copy `appsettings.example.json` to `appsettings.json`
   - Update database connection string and other settings in `appsettings.json`

3. Run database migrations
   ```
   dotnet ef database update
   ```

4. Run the application
   ```
   dotnet run
   ```

5. Access the API at `https://localhost:5001` or `http://localhost:5000`

## Features

- User authentication and authorization (JWT)
- Role-based access control (Student/Instructor)
- Course management
- File upload to Azure Blob Storage
- Assessment functionality

## API Endpoints

The API provides the following endpoints:

- `/api/auth` - Authentication endpoints
- `/api/courses` - Course management
- `/api/assessments` - Assessment functionality
- `/api/users` - User management

## Deployment

### Azure Deployment

Follow these steps to deploy to Azure:

1. Create an Azure App Service
2. Set up Azure SQL Database
3. Configure Azure Blob Storage
4. Deploy using Visual Studio or Azure CLI

For detailed deployment instructions, see the [Deployment Guide](/docs/deployment.md).

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details. 