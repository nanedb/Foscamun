-- Migration Script: Remove ICJ from Committees table
-- This script removes any ICJ entries from the Committees table
-- ICJ is now managed separately with CommID = -1 in the application logic

-- Remove ICJ from Committees table if it exists
DELETE FROM Committees WHERE Name = 'ICJ';

-- Remove any country lists associated with ICJ (if CommID was known)
-- This is a safety measure in case there were countries assigned
DELETE FROM CountryLists WHERE CommID NOT IN (SELECT CommID FROM Committees);

-- Verify the changes
SELECT 'Committees after cleanup:' as Info;
SELECT * FROM Committees ORDER BY Name;

SELECT 'ICJ table status:' as Info;
SELECT COUNT(*) as RecordCount FROM ICJ;
