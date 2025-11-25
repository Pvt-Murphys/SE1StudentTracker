# Developer Guide

### Identity Functions
This project uses Microsoft's Identity framework for its authentication and role-based access. Currently, SSO is not enabled. Refer to Microsoft's ASP .Net Identity documentation for more details.

https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity

### Page Functions

This is a brief overview of each page in the application and how it accomplishes its task.

Register Page:
located in Areas/Identity/Pages/Account/Register.cshtml
Uses Identity's basic template to provide a service where you can register an account with the application, and also select a permission level that the account will posess. If users select the Admin role, they inherit all other roles as well, whereas Teachers and Students only recieve their namesake roles. When a new user registers, they are automatically logged in. The SSO code is currently commented out, and could be reintegrated with some work.

Login Page:
located in Areas/Identity/Pages/Account/Login.cshtml
Uses Identity's basic template to allow a user to log into a user account using the account's email and password. Currently, no password recovery functions are available. The SSO code is currently commented out, and could be reintegrated with some work.

Note that a separate login page does exist in /Login.cshtml. This page is not used and does not work.

Logout Page
located in Areas/Identity/Pages/Account/Logout.cshtml
Simple on-click "page" that logs the current user out of their account and redirects them to the Login page. Is not a real *page*. 

_LoginPartial.cshtml
located in Pages/Shared/_LoginPartial.cshtml
Navigation bar that displays different pages based on if the user is logged in or not. When the user is logged out, it displays Register and Log In, but when logged in, it displays Time, Students, Admin, the user account page, and Logout. These pages are all visible to users without the appropriate role, but visiting the page does not allow access to the page's function.

User Account Information
Location unknown- page included by default by Identity.
Allows the user to change their account information.

Admin
located in /Admin.cshtml
Allows a user with the Admin role to execute a SQL query directly against the database. This page was designed with more functionality and ease of use in mind, but time constraints forced those features to be cut.

Error
located in /Error.cshtml
The standard 404 error page. All nonexistant pages are redirected to it.

Index
located in /Index.cshtml
The website's homepage. Does nothing.

Privacy
located in /Privacy.cshtml
Displays the site's privacy policy, if it had one. This prototype does not need a privacy policy.

Students
located in /Students.cshtml
Allows users with the Teacher role to query the database for time records of specific students, using their email to identify them.

Time
located in /Time.cshtml
Allows users with the Student role to "clock in" to the system, submitting a new record to the database with the "open" status, so long as they are not already clocked in and they have selected a location. selecting "clock out" closes the most recent record in that location.


### Database Integration

This application uses SQLite for ease of use and setup. The default connection string is found in /appsettings.json, and supporting code is found in /Program.cs, /Data/AppDbContext.cs, and /Data/DesignTimeDbContextFactory.cs.

/Data/AppDbContext.cs:
This file ensures that the DB is correctly set up on first running, and opens the connection to the database. Currently, it also enables foreign keys, but this function is not being utilized by the application.

/Data/DesignTimeDbContextFactory.cs:
uses AppDbContext.cs to build the database context for the application.


### Other Files

/Data/IdentitySeed.cs
This file adds the roles to the Identity system if the database does not have them.

/Program.cs
Defines the startup process for the application. Assembles all the separate pieces of the project and launches them together. Currently contains depreciated test code for assigning a test account all roles in the application.

