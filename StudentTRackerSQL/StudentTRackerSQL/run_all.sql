SET ECHO ON
SET FEEDBACK ON
WHENEVER SQLERROR EXIT FAILURE

-- Run in order. You can comment out parts when iterating quickly.
@01_schema.sql
@02_tables.sql
@03_constraints.sql
@04_indexes.sql
@05_seed.sql
@06_views.sql
@07_security.sql
@08_procedures.sql
@09_triggers.sql
@10_test.sql

PROMPT === Build complete ===
