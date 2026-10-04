USE Fusion;

-- Run this once when the Users table already exists.
ALTER TABLE Users
ADD COLUMN Role VARCHAR(20) NOT NULL DEFAULT 'User' AFTER PasswordHash;

-- For this lesson only, promote one user manually after registration.
-- Replace the email before running this statement.
UPDATE Users
SET Role = 'Admin'
WHERE Email = 'aarav.auth@example.com';
