-- Seed one instructor, one student, one course/section, a location
INSERT INTO user_account(role_id,email,password_hash,first_name,last_name)
SELECT role_id,'instructor@example.edu','x','Alex','Instructor' FROM role WHERE name='Instructor';
INSERT INTO instructor_profile(user_id,department,title)
SELECT user_id,'Health Sciences','Professor' FROM user_account WHERE email='instructor@example.edu';

INSERT INTO user_account(role_id,email,password_hash,first_name,last_name)
SELECT role_id,'student@example.edu','x','Sam','Student' FROM role WHERE name='Student';
INSERT INTO student_profile(user_id,student_number,program_name,cohort_year)
SELECT user_id,'S12345','Medical','2026' FROM user_account WHERE email='student@example.edu';

INSERT INTO course(course_code,course_name,term,credit_hours)
VALUES ('MED-101','Intro to Clinical Practice','FA25',3);

INSERT INTO section(course_id,section_code,term,primary_instructor_id)
SELECT c.course_id,'001','FA25', (SELECT user_id FROM user_account WHERE email='instructor@example.edu')
FROM course c WHERE c.course_code='MED-101';

INSERT INTO enrollment(section_id, student_id, status)
SELECT s.section_id, (SELECT user_id FROM user_account WHERE email='student@example.edu'), 'enrolled'
FROM section s WHERE s.section_code='001';

INSERT INTO location(name,type,is_active) VALUES ('Clinical Site A','ClinicalSite','Y');
INSERT INTO location(name,type,is_active) VALUES ('Library Study Zone','StudyArea','Y');

COMMIT;

-- Clock in/out happy path
BEGIN
  sp_clock_in(
    p_user_id      => (SELECT user_id FROM user_account WHERE email='student@example.edu'),
    p_session_type => 'Study',
    p_location_id  => (SELECT location_id FROM location WHERE name='Library Study Zone')
  );
END;
/

BEGIN
  sp_clock_out(
    p_user_id => (SELECT user_id FROM user_account WHERE email='student@example.edu')
  );
END;
/

-- Quick check
SELECT session_id, session_type, clock_in_at, clock_out_at, duration_minutes, status
FROM time_session
ORDER BY session_id DESC FETCH FIRST 5 ROWS ONLY;
