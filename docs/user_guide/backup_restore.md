# Backup and Restore

Use the BackupService to create encrypted archives of your RitualOS data. Backups include rituals, codex entries and settings.

1. Choose a password for encryption.
2. Call `BackupService.CreateBackup("backup.rba", password)` to create an archive.
3. Restore with `BackupService.RestoreBackup("backup.rba", password)`.

Backups help safeguard your practice data and can be stored off-site for extra security.
