# RitualOS Logging Guide

RitualOS writes logs to the `logs` folder next to the application executable. `app.log` stores general information such as startup and shutdown events. Access attempts are recorded in `access.log` by the SigilLock service.

Logs are plain text and rotate daily. If you encounter issues, review the latest log files before reporting bugs.
