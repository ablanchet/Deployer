-- parameters 
--	0 : Database name, 
--	1 : Path to the backup file

BACKUP DATABASE [{0}] TO  DISK = N'{1}' WITH NOFORMAT, INIT,  NAME = N'{0}-Full Database Backup (Done by DBSetup console app)', SKIP, NOREWIND, NOUNLOAD,  STATS = 10;
