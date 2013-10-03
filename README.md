# Deployer #

Deployer is a simple Windows console application that try to ease deployment process by automating tasks. 
Deployer wants to be :

* Easy to use, even for the first time
* Rock solid, *I don't want to crash my database because of it*
* Easy to extend, *I want to easily implement my own task, with my own configuration requirements*

How to use it
-------------

Just type


    Deployer.exe -?

in any command line and see wich commands are available.

Current there is very basic commands :

* -? : show help
* -s or --setup : start a command line wizard to build the configuration
* -b or --backup : try to do brutal backup of a given database (based on the connection string) and save it in a given directory (already based on the connection string)
* -r or --restore : try to restore the last backup file to the given database. Be careful with that, there no coming back !

Next commands to implement are 

1. -db or --db-setup : run another program in order to trigger a database migration (not the easiest command)
2. -z or --unzipto : unzip a given .zip archive to a given folder (for example, to copy dlls or files in a website directory)

Hey, I wanna make my own *action* !
-----------------------------------

That's pretty easy.

The first step is to implement a new class that implements the **IAction** interface. *Deployer* will automatically load all classes that implements the **IAction** interface available in the **Deployer.Actions** assembly. So if you want your action automatically loaded, implement your action in the right project.

Wow there is only one step !

For more informations about actions, just look the **IAction** interface documentation.

### Hum ... And if I want to save some configuration ? ###

1. You'll have to update the **ISettings** interface and its Xml implementation
2. You'll have to update the SettingsConfigurator action (the command line wizard)
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