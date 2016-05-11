﻿
namespace Assets.Scripts.Commands
{
    /*
     * command template
     * each command should derive from this or its subclasses
     * each command should have a unique name for easier identification
     * the name should be equal to its name in the command console
     * "use" should implement the functionality of the command and handle argument parsing
     */
    abstract class Command
    {
        string Name;

        public Command(string commandName)
        {
            Name = commandName;
        }

        /*
         * use command with the given arguments
         * returns TRUE, if the command was useable
         * returns FALSE, if any issue occured
         */
        abstract public bool use(string[] args);
    }
}