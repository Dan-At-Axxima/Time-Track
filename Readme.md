# Time Tracker

A time tracking system designed for Axxima to manage and record employee work hours & activities, built using ASP.NET Core.

## Key features

- Employee Time Tracking: Employees can log their work hours against various clients, projects, and activities.
- Comments: Users can add comments to their time entries.
- Assignment Management: Admins or users can manage assignments, including adding or removing them for employees.
- Reporting and Freezing: The system supports the ability to "freeze" data up to certain dates (e.g., for payroll or billing cycles).
- Role-Based Access: Integration with Azure AD allows for secure, role-based access (e.g., admin functions are only visible to administrators).
- Database Integration: The application connects to a SQL Server database (Azure) to store all time tracking and user data.
- UI Components: Uses Telerik UI for ASP.NET Core for rich, interactive user interfaces, including a spreadsheet view to display a grid of activities & days.

## Technical Stack:

- Frontend: Razor Pages, Telerik UI components, Kendo UI
- Backend: ASP.NET Core, Entity Framework Core
- Database: SQL Server (Azure-hosted)
- Authentication: Microsoft Identity Platform (Azure AD)

# Installation Guide

1. Clone the repository https://github.com/Axxima/time-tracker

2. Download & install Visual Studio

3. A connection to a SQL server is required:
	- If the db doesn't exist in Azure - Create a copy from a backup of the existing SQL database:
		- Backups are planned to be stored as Blobs in Azure
			- on the same Blob storage resource that 'Axxima CMS' uses (to save costs)
		- Make sure you have access to an SQL server + database in Azure
			- Azure resource name: 'SQL Server (logical server)'
			- Once created, within the resource, select option 'Create database'
		- Use the program 'SQL Server Management Studio' to migrate the database to your own on in Azure.
			- Connect to the Azure DB in SSMS.
			- Load the backup file of the DB in SSMS.
			- Then right click the db, select 'Tasks' -> 'Migrate Database to Microsoft Azure SQL Database'
			- Follow the wizard's steps.
	- Connect this application to the database on Azure:
		- In Azure portal, find the SQL DB and view its connection string:
			- in Settings -> Connection strings -> ADO.NET (SQL authentication)
		- in appsettings.json, paste it into ConnectionStrings.TimeTrackerContext
			- Or, if developing locally, use appsettings.Development.json
		- You may need to add the fields 'User ID' and 'Password' to this string manually
			- they should match the User ID and Password you set for the DB.

4. Connection to AzureAd is required for user authentication:
	- In MS Entra portal, create/find an 'App registration' for Time Tracker
		- Paste the App registration's 'Application ID' into appsettings.json's AzureAd.ClientId
		- Paste your org's 'Tenant ID' into appsettings.json's AzureAd.TenantId

5. Telerik.UI.for.AspNet.Core package is required:
	- Axxima has a license for this package.
		- Download 'Progress Telerik UI for ASP.NET Core' from https://www.telerik.com/account/downloads
			- Complete the installer wizard
		- Download 'telerik-license.txt' from https://www.telerik.com/account/your-licenses/license-keys
			- Place the file in the root of this project (it will be ignored by git)

6. Build & run!

# Summary of codebase structure

The application follows a standard ASP.NET Core structure, separating concerns into controllers, data models, business logic, UI pages, and static assets. The main entry point is Program.cs, and the user interface is built with Razor Pages under the Pages/ directory. Configuration is managed via JSON files at the root.

1. Key Directories
	- Controllers/
		- Contains API controllers (e.g., DataAccessController.cs) that handle HTTP requests for data operations.
	- Data/
		- Entity and data model classes (e.g., Activities.cs, Assignments.cs, Employee.cs, TimeTrackerContext.cs).
		- TimeTrackerContext.cs is the Entity Framework DbContext, mapping models to database tables.
	- Functions/
		- Business logic and service interfaces/implementations (e.g., DataFunctions.cs, UserFunctions.cs, and their interfaces).
		- Encapsulates core operations like saving transactions, managing assignments, and user-related logic.
	- Models/
		- Data transfer objects and view models (e.g., Activity.cs, Users.cs, UpdateValues.cs).
		- Used for passing structured data between layers and to the UI.
	- Pages/
		- Razor Pages for the web UI (e.g., Index.cshtml, ManageActivities.cshtml, AdminFunctions.cshtml).
		- Each page typically has a .cshtml (markup/UI) and a .cshtml.cs (page model/code-behind) file.
		- Shared/ subfolder contains layout and partial views (e.g., _Layout.cshtml, _LoginPartial.cshtml).
	- Services/
		- Service interfaces (e.g., ICurrentUserService.cs) for dependency injection and cross-cutting concerns.
	- wwwroot/
		- Static web assets: CSS, JavaScript, images, and third-party libraries.

2. Entry Points
	- Program.cs
		- The main entry point for the ASP.NET Core application.
		- Configures services, authentication, middleware, and routing.
	- Pages/Index.cshtml & Index.cshtml.cs
		- The default landing page for the web UI.
		- Handles the main time-tracking interface.

3. Configuration & Settings
	- appsettings.json and appsettings.Development.json
		- Store environment-specific settings (database connection strings, Azure AD config, URLs, logging).
	- Properties/launchSettings.json
		- Development and launch profiles for running the application locally.

4. Other Notable Files
	- TimeTrackerRepo.csproj and TimeTrackerRepo.sln
		- Project and solution files for .NET build and dependency management.
	- Readme.md
		- Setup and deployment instructions.

# Dependencies and integrations

1. External Services
	- Microsoft Azure SQL Database
		- The application connects to a SQL Server database, which can be hosted on Azure.
		- Connection strings are managed in appsettings.json and appsettings.Development.json.
	- Microsoft Azure Active Directory (Azure AD)
		- Used for user authentication and role-based access.
		- Configured via the AzureAd section in the settings files.
		- Integrates with Microsoft Identity Platform for secure login.

2. Key Libraries & Packages
	- ASP.NET Core
		- The main web framework for building the application (Razor Pages, Controllers, Middleware).
	- Entity Framework Core
		- ORM for database access and migrations.
		- Packages: Microsoft.EntityFrameworkCore, Microsoft.EntityFrameworkCore.SqlServer.
	- Microsoft Identity Web
		- Handles authentication and integration with Azure AD.
		- Packages: Microsoft.Identity.Web, Microsoft.Identity.Web.UI.
	- Telerik UI for ASP.NET Core (Kendo UI)
		- Provides advanced UI components (grids, forms, etc.) for the web interface.
		- Requires a commercial license.
	- Serilog
		- Logging framework for structured logging to files.
		- Package: Serilog.AspNetCore.
	- Newtonsoft.Json
		- For JSON serialization/deserialization.

## Data Flow

1. User Interaction
	- Users interact with the web UI (Razor Pages, e.g., Pages/Index.cshtml).
	- Data is entered via forms, grids, or batch entry interfaces (using Telerik/Kendo UI components).

2. Frontend to Backend
	- User actions (e.g., saving hours, adding comments, managing assignments) trigger HTTP requests to backend API endpoints (in Controllers/DataAccessController.cs).
	- Example endpoints:
		- POST api/DataAccess/SaveActivity – Save a time entry
		- POST api/DataAccess/SaveComment – Save a comment
		- POST api/DataAccess/SaveCellData – Save cell data (hours/comments)
		- POST api/DataAccess/BatchSave – Batch save multiple entries
		- POST api/DataAccess/AddAssignment – Add a new assignment

3. Backend Processing
	- Controllers receive requests and call business logic in the Functions/ layer (e.g., DataFunctions.cs).
	- Business logic interacts with the TimeTrackerContext to read/write data to the database.
	- Example: DataFunctions.SaveTransaction() checks if a transaction exists for the given employee/date/activity, updates it if so, or inserts a new record otherwise.

4. Database Operations
	- All persistent data (users, assignments, activities, time entries, comments, etc.) is stored in the SQL Server database.
	- Entity Framework Core handles querying, updating, and saving changes.

5. Data Retrieval
	- When the UI needs to display data (e.g., loading a user’s assignments or time entries), it calls API endpoints like GetCellData or GetActivityList.
	- The backend fetches the relevant data from the database and returns it as JSON.

6. Session and Authentication
	- User identity and session state are managed via ASP.NET Core session middleware and Azure AD authentication.
	- Session data (e.g., current user) is used to scope data access and operations.