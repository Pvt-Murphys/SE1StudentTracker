-- Example: audit updates to time_session made by admins via app (you can call into this from app code).
-- For POC, we’ll add a generic trigger that captures BEFORE/AFTER on UPDATE.

CREATE OR REPLACE TRIGGER trg_timesession_audit
AFTER UPDATE ON time_session
FOR EACH ROW
BEGIN
  INSERT INTO audit_log (actor_user_id, target_table, target_pk, action, before_json, after_json, reason)
  VALUES (
    /* actor_user_id: supply via app (e.g., a context variable). For POC we’ll store NULL. */
    NULL,
    'TIME_SESSION',
    TO_CHAR(:OLD.session_id),
    'UPDATE',
    JSON_OBJECT(
      'clock_in_at' VALUE TO_CHAR(:OLD.clock_in_at, 'YYYY-MM-DD HH24:MI:SS'),
      'clock_out_at' VALUE TO_CHAR(:OLD.clock_out_at, 'YYYY-MM-DD HH24:MI:SS'),
      'status' VALUE :OLD.status
    ) FORMAT JSON,
    JSON_OBJECT(
      'clock_in_at' VALUE TO_CHAR(:NEW.clock_in_at, 'YYYY-MM-DD HH24:MI:SS'),
      'clock_out_at' VALUE TO_CHAR(:NEW.clock_out_at, 'YYYY-MM-DD HH24:MI:SS'),
      'status' VALUE :NEW.status
    ) FORMAT JSON,
    'POC auto audit'
  );
END;
/
SHOW ERRORS
