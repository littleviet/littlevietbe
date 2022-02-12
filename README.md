# LittleVietBE #


### What is this repository for? ###

* Backend for littleviet.es

### Setting Up Development Configurations ###
1. Retrieve the secrets.json file for Development purpose from the project Credentials Page on Confluence

2. Run the script in \scripts\development\set-user-secrets.ps1 (make sure your working directory is ...\scripts\development, and that .net CLI is installed)

**Notes**: The script is unsigned so you'll have to temporarily disable ScriptExecution policy on Windows machines using 
```
Set-ExecutionPolicy Unrestricted -Scope Process -Force
```
The above code will disable ExecutionPolicy for your current Powershell process, you **must** then run the script in the **same** terminal (using command `.\set-user-secrets.ps1`)
### Who do I talk to? ###

* Repo owner or admin
