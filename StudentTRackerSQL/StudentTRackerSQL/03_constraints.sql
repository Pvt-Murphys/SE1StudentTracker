ALTER TABLE user_account
  ADD CONSTRAINT fk_user_role
  FOREIGN KEY (role_id) REFERENCES role(role_id);

ALTER TABLE student_profile
  ADD CONSTRAINT fk_student_user
  FOREIGN KEY (user_id) REFERENCES user_account(user_id)
  ON DELETE CASCADE;

ALTER TABLE instructor_profile
  ADD CONSTRAINT fk_instructor_user
  FOREIGN KEY (user_id) REFERENCES user_account(user_id)
  ON DELETE CASCADE;

ALTER TABLE section
  ADD CONSTRAINT fk_section_course
  FOREIGN KEY (course_id) REFERENCES course(course_id);

ALTER TABLE section
  ADD CONSTRAINT fk_section_instructor
  FOREIGN KEY (primary_instructor_id) REFERENCES user_account(user_id);

ALTER TABLE enrollment
  ADD CONSTRAINT fk_enrollment_section
  FOREIGN KEY (section_id) REFERENCES section(section_id)
  ON DELETE CASCADE;

ALTER TABLE enrollment
  ADD CONSTRAINT fk_enrollment_student
  FOREIGN KEY (student_id) REFERENCES user_account(user_id)
  ON DELETE CASCADE;

ALTER TABLE time_session
  ADD CONSTRAINT fk_timesession_user
  FOREIGN KEY (user_id) REFERENCES user_account(user_id);

ALTER TABLE time_session
  ADD CONSTRAINT fk_timesession_section
  FOREIGN KEY (section_id) REFERENCES section(section_id);

ALTER TABLE role_permission
  ADD CONSTRAINT fk_rp_role FOREIGN KEY (role_id) REFERENCES role(role_id);

ALTER TABLE role_permission
  ADD CONSTRAINT fk_rp_perm FOREIGN KEY (permission_id) REFERENCES permission(permission_id);

ALTER TABLE audit_log
  ADD CONSTRAINT fk_audit_actor FOREIGN KEY (actor_user_id) REFERENCES user_account(user_id);

-- If it's a Class session, a Section must be provided
ALTER TABLE time_session ADD CONSTRAINT ck_timesession_class_section
  CHECK ( (session_type <> 'Class') OR (section_id IS NOT NULL) );
