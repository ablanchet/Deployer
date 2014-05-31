# Deployer #

Deployer is a simple Windows console application that try to ease deployment process by automating tasks. 
Deployer wants to be :

* Easy to use, even for the first time
* Rock solid, *I don't want to crash my database because of it*
* Easy to extend, *I want to easily implement my own task, with my own configuration requirements*

How to use it
-------------

Just type


    Deployer.exe

in any command line and see wich commands are available.

Current there is very basic commands :

	-show-help                 Show the program usage. All commands available,
	
							   and their descriptions
	-backup                    Do a quick backup of the configured database.
	-backup --as=VALUE         Do a quick backup with a specific name for the
							   backup.
							   
	-dbsetup                   Run the DBSetup to the configured database
	-dbsetup --from=VALUE      DBSetup from a specific backup. Backup/Restore/
							   DBSetup/Restore.
	-dbsetup --no-refresh      DBSetup will not refresh views.
	-dbsetup --on-azure        DBSetup target azure database.
	
	-restore                   Restore the last backup file to the configured
							   database
	-restore --from=VALUE      Restore the last backup file with a specific name.
	
	-settings-configurator     Configure the application with a nice walkthroug
	-settings-displayer        Display the configuration that will be use while
							   run and other operations

Next commands to implement are 

1. -file-deploy : unzip a given .zip archive to a given folder (for example, to copy dlls or files in a website directory)

Hey, I wanna make my own *action* !
-----------------------------------

That's pretty easy.

The first step is to implement a new class that implements the **IAction** interface. *Deployer* will automatically load all classes that implements the **IAction** interface available in the **Deployer.Actions** assembly and in the **Deployer.Actions** namespace. So if you want your action automatically loaded, implement your action in the right project.

Wow there is only one step !

For more information about actions, just look the **IAction** interface documentation.

### Conventions ###
#### Naming conventions ####
How the action "command line trigger" like "-show-help" are detected / computed ? Just by processing the class name. For example the -show-help action is implemented by the ShowHelpAction class.

So all actions must implement IAction interface, and must be implemented in the Deployer.Actions namespace. Why is that ? It allow the develop to check if there is no duplicates actions names :)
#### Error handling ####

The error handling is done via the IActivityLogger. Do not return false or throw a new Exception. Just log something like :

	logger.Error(ex, "Oops an error occurred in this process, see the error details n°{0}", 3712);

And the error will break the rest of the process and will be displayed in the console. 

### Hum ... And if I want to save some configuration ? ###

1. You'll have to update the **ISettings** interface and its XML implementation (or add another implementation)
2. You'll have to update the SettingsConfiguratorAction (the command line wizard)
3. And you're done, your configuration will be loaded next time

Contribute
----------
Just clone the repo, run the solution in Visual Studio, and you're done.

I'm interested in three contributions :

1. Is it working on Mono ?
2. Check if actions prototypes are not conflicted
3. Actions implementations :)

License
-------
Do What The Fuck You Want With It (Public License)
http://www.wtfpl.net/