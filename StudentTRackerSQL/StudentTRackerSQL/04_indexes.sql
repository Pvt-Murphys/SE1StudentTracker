CREATE INDEX ix_user_email           ON user_account(email);

CREATE INDEX ix_ts_user_in           ON time_session(user_id, clock_in_at);
CREATE INDEX ix_ts_section_in        ON time_session(section_id, clock_in_at);
CREATE INDEX ix_ts_status            ON time_session(status);
-- Optional text search helper:
CREATE INDEX ix_ts_location_text     ON time_session(location_text);
