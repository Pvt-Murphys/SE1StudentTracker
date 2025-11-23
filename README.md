# SE1StudentTracker
Proof of concept application that tracks students' locations and times, and allows instructors to view it. This application offers a fully fledged account system to allow a user to create three different types of accounts (student, teacher, or administrator) and access different aspects of the application that pertains to their specific role. For instance, a teacher can access the students page to view time records containing info about all students clock-in/clock-out information. The administrator role does exist, but as of the final feature implementation sprint, this role does not offer the functionality we wished to provide it due to time constraints. Location can be tracked through information kept in the clock records of a student. For instance, when a student clocks in, they will be allowed to select a pre-specified location from a dropdown that will be kept inside that clock record. This allows a teacher to see where a student currently should be. Live GPS tracking is not possible currently due to 1) This application is only a proof of concept 2) Complexity coupled with time constraints 3) Lack of funding.

## Function
The application contains three main functions: login/account management, time recording, and record viewing. These are pretty self explanatory as to what they allow you to do, but for clarification, here is the user flow:

                                                                                                          -------> STUDENT ROLE ---> TIME PAGE ---> STUDENT CLOCKS IN/OUT
                                                                                                          |
LOGIN ---> USER CHOOSES ROLE (ADMIN, TEACHER, STUDENT) ---> ACCOUNT CREATED; USER GRANTED ACCESS TO APP -- ------> TEACHER ROLE ---> STUDENTS PAGE ---> TEACHER VIEWS ALL STUDENTS CLOCK IN INFORMATION
                                                                                                          |
                                                                                                          -------> ADMIN ROLE (NOT YET IMPLEMENTED) ---> ADMINISTRATOR MODIFIES OTHER ACCOUNTS 

### Account System
The account system requires an email and password to make a user's account, and requests that the user specify on account creation what type of user they are- whether they are a teacher, a student, or an administrator. An account, once created, can be used to log into the application, and the application will only allow access to the pages that the user has permissions to depending on their role. This account is made persistent across instances of the application, meaning that upon closing of the web app the account that the user created will still exist and will still be signed in until signed out of. This function utilizes the ASP.NET Identity system which is an authentication system that allows the creation and management of roles, users, user information such as passwords, and of course authentication. 

Users are also capable of changing their account information, such as email and password, via a management system.

(In a full release version of the application, the only account type that could be freely made would be the student account--all other roles would only be applied by an administrator, to prevent persons from acquiring uneccessary permissions and doing harm to the system.)

### Time Recording
The "Time" page is only accessible to students. This page includes a selector for the student's location, to be voluntarily disclosed, and options to clock in or clock out. This is to fulfill the project requirements- theoretically, a better solution would be to include GPS tracking on the client device, but that could not be implemented in the development timeframe. Data from the clock in operation is recorded in the database as a single open record, which is then closed with the total duration (including start and end time) when the user selects the clock out option. 

For clock out functionality, the ASP.NET Identity system worked wonders to allow "clocked in" persistence in the sense that if a user contains an open clock record (clocked in but not yet out), the clock out button will only be available. In short, the UI will react based on a users clock status and will disable the clock out button if the user is not yet clocked out and vice versa.

### Record Viewing
The "Students" page is only accessible to teachers. This page includes a field with which you can request records for a specific students, based on the student's email. Once queried, this will return a list of the student's time entries.
In a full release version of the application, functionality would be added to total up the time a specific student has entered into the database, and select specific time periods to list all the students that have entered records in that time period.

This functionality works by



