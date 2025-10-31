------------------------------------------------------------------------------
-- 1) APP ROLES
------------------------------------------------------------------------------
BEGIN
  EXECUTE IMMEDIATE 'CREATE ROLE APP_STUDENT';
EXCEPTION WHEN OTHERS THEN IF SQLCODE != -01921 THEN RAISE; END IF; -- ignore "role exists"
END;
/
BEGIN
  EXECUTE IMMEDIATE 'CREATE ROLE APP_INSTRUCTOR';
EXCEPTION WHEN OTHERS THEN IF SQLCODE != -01921 THEN RAISE; END IF;
END;
/
BEGIN
  EXECUTE IMMEDIATE 'CREATE ROLE APP_ADMIN';
EXCEPTION WHEN OTHERS THEN IF SQLCODE != -01921 THEN RAISE; END IF;
END;
/

------------------------------------------------------------------------------
-- 2) GRANTS (LEAST PRIVILEGE)
--  Students: view their own hours via v_student_hours + clock procs
--  Instructors: view sessions via v_timesession_full (filtered by VPD) + map
--  Admins: full DML on core tables + views + procs
------------------------------------------------------------------------------

-- STUDENT
GRANT SELECT ON v_student_hours         TO APP_STUDENT;
GRANT EXECUTE ON sp_clock_in            TO APP_STUDENT;
GRANT EXECUTE ON sp_clock_out           TO APP_STUDENT;

-- INSTRUCTOR
GRANT SELECT ON v_timesession_full      TO APP_INSTRUCTOR;
GRANT SELECT ON v_instructor_student_map TO APP_INSTRUCTOR;

-- ADMIN
GRANT SELECT, INSERT, UPDATE, DELETE ON role               TO APP_ADMIN;
GRANT SELECT, INSERT, UPDATE, DELETE ON user_account       TO APP_ADMIN;
GRANT SELECT, INSERT, UPDATE, DELETE ON student_profile    TO APP_ADMIN;
GRANT SELECT, INSERT, UPDATE, DELETE ON instructor_profile TO APP_ADMIN;
GRANT SELECT, INSERT, UPDATE, DELETE ON course             TO APP_ADMIN;
GRANT SELECT, INSERT, UPDATE, DELETE ON section            TO APP_ADMIN;
GRANT SELECT, INSERT, UPDATE, DELETE ON enrollment         TO APP_ADMIN;
GRANT SELECT, INSERT, UPDATE, DELETE ON time_session       TO APP_ADMIN;
GRANT SELECT, INSERT, UPDATE, DELETE ON audit_log          TO APP_ADMIN;
GRANT SELECT, INSERT, UPDATE, DELETE ON permission         TO APP_ADMIN;
GRANT SELECT, INSERT, UPDATE, DELETE ON role_permission    TO APP_ADMIN;

GRANT SELECT ON v_timesession_full      TO APP_ADMIN;
GRANT SELECT ON v_student_hours         TO APP_ADMIN;
GRANT SELECT ON v_instructor_student_map TO APP_ADMIN;

GRANT EXECUTE ON sp_clock_in            TO APP_ADMIN;
GRANT EXECUTE ON sp_clock_out           TO APP_ADMIN;

------------------------------------------------------------------------------
-- 3) APPLICATION CONTEXT (so VPD can know "who" the end user is)
--    Your app will call: PKG_SECURITY_CTX.SET_CONTEXT(:user_id, :role_name)
------------------------------------------------------------------------------

-- Create a named context for our app
BEGIN
  EXECUTE IMMEDIATE 'CREATE OR REPLACE CONTEXT ST_CTX USING PKG_SECURITY_CTX';
END;
/

-- Package to set the context from the app
CREATE OR REPLACE PACKAGE pkg_security_ctx AS
  PROCEDURE set_context(p_user_id IN NUMBER, p_role_name IN VARCHAR2);
END pkg_security_ctx;
/
CREATE OR REPLACE PACKAGE BODY pkg_security_ctx AS
  PROCEDURE set_context(p_user_id IN NUMBER, p_role_name IN VARCHAR2) IS
  BEGIN
    DBMS_SESSION.SET_CONTEXT('ST_CTX', 'USER_ID',   TO_CHAR(p_user_id));
    DBMS_SESSION.SET_CONTEXT('ST_CTX', 'ROLE_NAME', UPPER(TRIM(p_role_name)));
  END;
END pkg_security_ctx;
/
SHOW ERRORS

--
