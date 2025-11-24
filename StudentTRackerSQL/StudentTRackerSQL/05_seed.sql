INSERT INTO role(name) VALUES ('Student');
INSERT INTO role(name) VALUES ('Instructor');
INSERT INTO role(name) VALUES ('Admin');

INSERT INTO permission(code, description) VALUES ('VIEW_SELF_HOURS','Student can view their own tracking history');
INSERT INTO permission(code, description) VALUES ('VIEW_ENROLLED_STUDENTS','Instructor can view enrolled students’ sessions');
INSERT INTO permission(code, description) VALUES ('ADMIN_MANAGE_USERS','Admin can view and change user accounts');
INSERT INTO permission(code, description) VALUES ('ADMIN_MANAGE_SESSIONS','Admin can view and change tracking records');

INSERT INTO role_permission(role_id, permission_id)
SELECT r.role_id, p.permission_id FROM role r JOIN permission p
ON ( (r.name='Student'   AND p.code='VIEW_SELF_HOURS')
  OR (r.name='Instructor' AND p.code='VIEW_ENROLLED_STUDENTS')
  OR (r.name='Admin'      AND p.code IN ('ADMIN_MANAGE_USERS','ADMIN_MANAGE_SESSIONS')) );

COMMIT;
