# Mini Social Media Feed task

A mini social media feed project built with ASP.NET Core and Entity Framework Core.

## Features

- User registration and login
- Create posts
- Retrieve posts from user and people the user follows
- Follow and unfollow users
- Like posts
- Post feed with pagination and caching

## Technologies Used

- ASP.NET Core 8.0
- Entity Framework Core
- SQL Server
- JSON Web Tokens (JWT) for authentication
- NewtonSoft.Json
- AutoMapper for object mapping
- In-memory caching

## Getting Started

### Prerequisites

- .NET Core SDK 
- SQL Server (or InMemory database)

### Installation

1. Clone the repository: 
- git clone https://github.com/Reagan-C/MiniFeed

2. Navigate to the project directory: 
- cd MiniFeed

3. Install the dependencies:
- dotnet restore

4. Configure the database connection string:
- Open the `appsettings.json` file.
- Locate the `ConnectionStrings` section.
- Update the `DefaultConnection` string with your database connection details.

5. Run the database migrations:
- dotnet ef database update

6. Start the application:
- dotnet run

7. The API will be accessible at `https://localhost:7001` and `http://localhost:7002`.

## API Endpoints

- `POST /api/auth/register`: Register a new user.
- `POST /api/auth/login`: Log in and obtain an authentication token.
- `POST /api/auth/assignRole`: Assign admin role.
- `POST /api/posts/add`: Create a new post.
- `GET /api/posts/{id}`: Fetch a post by Id.
- `DELETE /api/posts/{postId}`: Delete a post.
- `PUT /api/posts/{id}`: Update a post.
- `GET /api/posts/feed`: Retrieve the post feed for the authenticated user.
- `POST /api/user/{username}/follow`: Follow another user.
- `POST /api/user/{username}/unfollow`: Unfollow another user.
- `POST /api/user/{postId}/like`: Like a post.

For detailed information on request and response formats, please refer to the API documentation on swagger.

