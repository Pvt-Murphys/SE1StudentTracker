CREATE OR REPLACE PROCEDURE sp_clock_in(
  p_user_id      IN NUMBER,
  p_session_type IN VARCHAR2,
  p_location_txt IN VARCHAR2 DEFAULT NULL,  -- << changed
  p_section_id   IN NUMBER DEFAULT NULL,
  p_source       IN VARCHAR2 DEFAULT 'web',
  p_notes        IN VARCHAR2 DEFAULT NULL
) AS
  v_open_count NUMBER;
  v_role_name  role.name%TYPE;
BEGIN
  SELECT r.name INTO v_role_name
  FROM user_account ua JOIN role r ON r.role_id=ua.role_id
  WHERE ua.user_id = p_user_id AND ua.status='active';

  SELECT COUNT(*) INTO v_open_count FROM time_session
  WHERE user_id = p_user_id AND status='open';
  IF v_open_count > 0 THEN
    RAISE_APPLICATION_ERROR(-20001, 'User already has an open session.');
  END IF;

  IF p_session_type = 'Class' AND p_section_id IS NULL THEN
    RAISE_APPLICATION_ERROR(-20002, 'Class sessions require section_id.');
  END IF;

  INSERT INTO time_session (user_id, session_type, location_text, section_id,
                            clock_in_at, source, status, notes, created_at)
  VALUES (p_user_id, p_session_type, p_location_txt, p_section_id,
          SYSTIMESTAMP, NVL(p_source,'web'), 'open', p_notes, SYSTIMESTAMP);
END;
/
SHOW ERRORS

CREATE OR REPLACE PROCEDURE sp_clock_out(
  p_user_id IN NUMBER
) AS
  v_session_id time_session.session_id%TYPE;
BEGIN
  SELECT session_id INTO v_session_id
  FROM time_session
  WHERE user_id = p_user_id AND status='open'
  FOR UPDATE;

  UPDATE time_session
     SET clock_out_at = SYSTIMESTAMP,
         status       = 'closed',
         updated_at   = SYSTIMESTAMP
   WHERE session_id = v_session_id;
END;
/
SHOW ERRORS
