CREATE OR REPLACE VIEW v_timesession_full AS
SELECT
  ts.session_id,
  ts.user_id,
  ua.first_name, ua.last_name, ua.email, ua.role_id,
  r.name AS role_name,
  ts.session_type,
  ts.location_text,            -- << simple label instead of FK
  ts.section_id, s.section_code, c.course_code, c.course_name,
  ts.clock_in_at, ts.clock_out_at, ts.duration_minutes, ts.source, ts.status, ts.notes
FROM time_session ts
JOIN user_account ua ON ua.user_id = ts.user_id
JOIN role r          ON r.role_id  = ua.role_id
LEFT JOIN section s  ON s.section_id = ts.section_id
LEFT JOIN course c   ON c.course_id  = s.course_id;

CREATE OR REPLACE VIEW v_student_hours AS
SELECT * FROM v_timesession_full;

CREATE OR REPLACE VIEW v_instructor_student_map AS
SELECT DISTINCT s.primary_instructor_id AS instructor_id, e.student_id
FROM section s
JOIN enrollment e ON e.section_id = s.section_id;
