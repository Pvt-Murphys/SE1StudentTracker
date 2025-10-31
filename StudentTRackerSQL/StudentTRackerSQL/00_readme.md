# Student Tracker Database (POC, No Location Table)

This folder contains the Oracle SQL scripts to build the **Student Tracker** database for the proof‑of‑concept web app. It’s designed to be fast to set up locally and easy to reset during development.

> **DB:** Oracle XE 18c/21c (recommended) or Oracle 12c+  
> **Schema:** `STUDENT_TRACKER` (dedicated user)  
> **ASP.NET:** .NET 8, packages `Oracle.ManagedDataAccess.Core`, `Oracle.EntityFrameworkCore`

---

## Folder Contents

```
db/
├─ 00_readme.md                  # this file
├─ 01_tables.sql                 # CREATE TABLE (no Location table; uses location_text)
├─ 02_constraints.sql            # FKs and CHECK constraints
├─ 03_indexes.sql                # useful indexes
├─ 04_seed.sql                   # base data (roles, permissions)
├─ 05_views.sql                  # reporting/role views
├─ 06_procedures.sql             # sp_clock_in / sp_clock_out
├─ 07_triggers.sql               # audit trigger
├─ 07_security.sql               # (optional) roles, grants, app context, VPD
├─ 10_test.sql                   # optional smoke test
├─ 98_drop_app_objects.sql       # safe reset for app objects only
└─ 99_run_all.sql                # orchestrator to build everything
```

---

## 1) One‑Time Local Setup

Install **Oracle XE** and **SQL Developer**. Create a dedicated schema/user for the app.

Connect as `SYSTEM` in SQL Developer (local):
- Host: `localhost`
- Port: `1521`
- **Service name:** `XEPDB1` (XE 18c/21c) — *or* **SID:** `XE` (older XE 11g)

Run:
```sql
CREATE USER STUDENT_TRACKER IDENTIFIED BY "Strong#Password1"
  DEFAULT TABLESPACE USERS
  TEMPORARY TABLESPACE TEMP
  QUOTA UNLIMITED ON USERS;

GRANT CREATE SESSION TO STUDENT_TRACKER;
GRANT CREATE TABLE, CREATE VIEW TO STUDENT_TRACKER;
GRANT CREATE SEQUENCE, CREATE PROCEDURE, CREATE TRIGGER TO STUDENT_TRACKER;
-- Optional (used by security context):
GRANT EXECUTE ON DBMS_SESSION TO STUDENT_TRACKER;
```

Create a daily dev connection in SQL Developer:
- Connection Name: `StudentTracker (local)`  
- Username: `STUDENT_TRACKER`  
- Password: your password  
- Host: `localhost` | Port: `1521`  
- Choose **Service name**=`XEPDB1` (if Test fails, try **SID**=`XE`)

---

## 2) Build the Database

Connect as `STUDENT_TRACKER`, open **`db/99_run_all.sql`**, and hit **Run Script (F5)**.

This runs, in order:
1. `01_tables.sql`
2. `02_constraints.sql`
3. `03_indexes.sql`
4. `04_seed.sql`
5. `05_views.sql`
6. `06_procedures.sql`
7. `07_triggers.sql`
8. *(Optional)* `07_security.sql` — roles, grants, app context, VPD

> Need a clean slate? Run `98_drop_app_objects.sql` first, then `99_run_all.sql` again.

### Quick Smoke Test (optional)
```sql
@10_test.sql
SELECT session_id, session_type, location_text, clock_in_at, clock_out_at, duration_minutes, status
FROM time_session
ORDER BY session_id DESC FETCH FIRST 5 ROWS ONLY;
```

---

## 3) Why the Schema Looks Like This

- **Users & Roles:** `USER_ACCOUNT`, `ROLE` (+ `STUDENT_PROFILE`, `INSTRUCTOR_PROFILE`) keep identity clean and extensible.
- **Academic Scope:** `COURSE` → `SECTION` → `ENROLLMENT` constrains instructor visibility to *their students*.
- **Time Tracking:** `TIME_SESSION` stores clock in/out. For speed/flexibility, we removed the `LOCATION` table and use `location_text`.
- **Business Rules:** One open session per user; Class requires Section; `duration_minutes` computed when clocked out.
- **Audit:** `AUDIT_LOG` records before/after changes to session edits (via trigger).

### Views for the UI
- `V_TIMESESSION_FULL`: joined reporting view for dashboards
- `V_STUDENT_HOURS`: student-friendly subset (same columns)
- `V_INSTRUCTOR_STUDENT_MAP`: helper for filtering by instructor’s roster

---

## 4) Security Options

### Simple (recommended for POC)
Use app-side role checks (Student/Instructor/Admin) and filter queries accordingly.

### Strong (included, optional): DB Roles + VPD
- DB Roles: `APP_STUDENT`, `APP_INSTRUCTOR`, `APP_ADMIN`
- Grants: least-privilege to views/procs/tables
- Application Context: `ST_CTX` set by `PKG_SECURITY_CTX.SET_CONTEXT(user_id, role)`
- VPD policy on `V_TIMESESSION_FULL` auto-filters rows:
  - Student → only their rows
  - Instructor → only students they teach
  - Admin → all rows

> Enable by running `07_security.sql`, then set context once per request.

---

## 5) ASP.NET Connection (local)

NuGet: `Oracle.ManagedDataAccess.Core`, `Oracle.EntityFrameworkCore`

**appsettings.json**
```json
{
  "ConnectionStrings": {
    "AppDb": "User Id=STUDENT_TRACKER;Password=Strong#Password1;Data Source=localhost:1521/XEPDB1;Pooling=true;"
  }
}
```

**Program.cs**
```csharp
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseOracle(builder.Configuration.GetConnectionString("AppDb")));
```

**(Optional) Set VPD context per request**
```csharp
await db.Database.ExecuteSqlInterpolatedAsync($@"
BEGIN
  pkg_security_ctx.set_context(p_user_id => {userId}, p_role_name => {roleName});
END;");
```

---

## 6) Useful Queries

**Is user clocked in?**
```sql
SELECT session_id, session_type, section_id, clock_in_at
FROM time_session
WHERE user_id = :userId AND status = 'open';
```

**Student history**
```sql
SELECT session_type, location_text, section_id, clock_in_at, clock_out_at, duration_minutes, status
FROM time_session
WHERE user_id = :userId
ORDER BY clock_in_at DESC
FETCH FIRST 100 ROWS ONLY;
```

**Instructor (their students)**
```sql
SELECT ts.*
FROM time_session ts
WHERE ts.user_id IN (
  SELECT e.student_id
  FROM section s
  JOIN enrollment e ON e.section_id = s.section_id
  WHERE s.primary_instructor_id = :instructorId
)
ORDER BY ts.clock_in_at DESC;
```

---

## 7) Troubleshooting

- **ORA-12514 / ORA-12154:** Swap **Service name=`XEPDB1`** ↔ **SID=`XE`** and Test again.
- **Listener not running:** Start *OracleOraDB…TNSListener* service or run `lsnrctl status`.
- **Object exists:** Run `98_drop_app_objects.sql` then `99_run_all.sql`.
- **Procedure compile errors:** `SHOW ERRORS` after `06_procedures.sql`.
- **Time zones:** Timestamps are server time; convert in the app for display if needed.

---

## 8) Contributing

- Keep all DB changes in versioned SQL files under `db/`.
- Don’t commit secrets. Use User Secrets or environment variables for credentials.
- Keep seed data generic; never commit real student data.
