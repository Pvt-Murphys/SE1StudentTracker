# SE1StudentTracker
Proof of concept application that tracks students' locations and times, and allows instructors to view it.

## Function
The application contains three main functions: login/account management, time recording, and record viewing. 

### Account System
The account system requires an email and password to make a user's account, and requests that the user specify on account creation what type of user they are- whether they are a teacher, a student, or an administrator. An account, once created, can be used to log into the application, and the application will only allow access to the pages that the user has permissions to depending on their role.

Users are also capable of changing their account information, such as email and password, via a management system.

(In a full release version of the application, the only account type that could be freely made would be the student account--all other roles would only be applied by an administrator, to prevent persons from acquiring uneccessary permissions and doing harm to the system.)

### Time Recording
The "Time" page is only accessible to students. This page includes a selector for the student's location, to be voluntarily disclosed, and options to clock in or clock out. This is to fulfill the project requirements- theoretically, a better solution would be to include GPS tracking on the client device, but that could not be implemented in the development timeframe. Data from the clock in operation is recorded in the database as a single open record, which is then closed with the total duration (including start and end time) when the user selects the clock out option.

### Record Viewing
The "Students" page is only accessible to teachers. This page includes a field with which you can request records for a specific students, based on the student's email. Once queried, this will return a list of the student's time entries.
In a full release version of the application, functionality would be added to total up the time a specific student has entered into the database, and select specific time periods to list all the students that have entered records in that time period.



