-- =============================================================
-- Seed: AppointmentsTypes
-- =============================================================
PRINT N'Seeding data: dbo.AppointmentsTypes';

;WITH S AS (
    SELECT *
    FROM (VALUES
        (N'Initial Interview'),
        (N'Follow-Up'),
        (N'Orientation'),
        (N'Background Check Review'),
        (N'Final Review')
    ) V([AppointmentTypeName])
)
MERGE dbo.AppointmentsTypes AS T
USING S
ON T.[AppointmentTypeName] = S.[AppointmentTypeName]
WHEN MATCHED AND T.[IsActive] <> 1
    THEN UPDATE SET
        T.[IsActive] = 1
WHEN NOT MATCHED BY TARGET
    THEN INSERT ([AppointmentTypeName], [IsActive], [DateCreated])
         VALUES (
             S.[AppointmentTypeName],
             1,
             GETDATE()
         );

PRINT N'✔ AppointmentsTypes seed complete.';