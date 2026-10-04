USE Fusion;

-- Run this once when the Users table already exists.
ALTER TABLE Users
ADD COLUMN PasswordHash VARCHAR(255) NULL AFTER Email;
