# BlogVilla

**BlogVilla** is a web-based platform that allows users to create, view, edit, and delete blogs, interact with other users through likes and comments, and manage blog drafts and cancellations. The platform includes an authentication system for users, with role-based access control (regular users and admin), ensuring that only authorized users can perform specific actions such as editing or deleting blogs. Admins have additional privileges for managing blog visibility.

## Purpose

The purpose of BlogVilla is to provide a full-fledged blogging platform where users can share their thoughts, interact with content from other users, and manage their own blog posts. The platform is designed to be user-friendly and secure, offering features like authentication, authorization, content management, and more.

---

## Main Functionalities

### 1. **User Authentication & Authorization**

-   User login and registration with JWT-based authentication.
-   Authorization roles such as Admin and Regular Users.
-   Admins can remove any blog post, while users can manage their own posts.

### 2. **Blog Management**

-   Users can create, view, edit, and delete blog posts.
-   Blog posts can be marked as **Draft** or **Canceled**.
-   Each blog post includes a title, content, creation and last updated timestamps, and a cover image.
-   **Cover Image Upload**: Users can upload and display a cover image for their blog posts.

### 3. **Like and Comment System**

-   Users can like and unlike blog posts.
-   Users can add comments to blogs, and admins can delete any inappropriate comments.
-   A tab system shows the number of likes and comments for each blog.

### 4. **Profile Management**

-   Users can view their profile and the profiles of other users, including information such as username, profile photo, and email address.

### 5. **Admin Features**

-   Admins can remove blog posts that are not drafts or canceled.
-   Admins can delete any comments that violate the community guidelines.

---

## Technologies Used

### Backend:

-   **ASP.NET Core MVC**
-   **Entity Framework Core**
-   **Dependency Injection**
-   **Repository Pattern**
-   **SQL Database** for storing blog, user, and interaction data.

### Frontend:

-   **Bootstrap** for responsive UI design.
-   **HTML/CSS** for custom styling.
-   **JavaScript** for interactivity.

### Other Libraries/Tools:

-   **JWT Authentication** for secure user login and role-based access.
-   **AutoMapper** for object mapping.
-   **SignalR (optional)** for real-time updates (if used for chat or notifications).
-   **Mail Services** (optional) for sending notifications to users.

---

## Prerequisites

Before running the project, ensure you have the following installed:

-   [.NET SDK 6.0+](https://dotnet.microsoft.com/download/dotnet/6.0)
-   [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or another compatible database)

---

## Installation & Setup

### Step 1: Clone the Repository

```bash
git clone https://github.com/yourusername/BlogVilla.git
cd BlogVilla
```

### Step 2: Configure the Database

-   Open `appsettings.json` and update the `ConnectionStrings` section to match your database configuration:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=BlogVillaDb;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;"
}
```

### Step 3: Apply Migrations

To set up the database schema, run the following command:

```bash
dotnet ef database update
```

This will apply the necessary migrations and create the database tables.

### Step 4: Run the Application

You can now run the application using the .NET CLI:

```bash
dotnet run
```

Alternatively, open the solution in Visual Studio and run it from there.

The app will be accessible at `http://localhost:5000` (or whatever port is configured).
