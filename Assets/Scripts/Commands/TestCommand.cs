using Assets.Scripts.Entity;
using System;
using UnityEngine;

namespace Assets.Scripts.Commands
{
    /*
     * a simple test command
     * can be used to test the correctness of the command parser
     */
    class TestCommand : AbstractCommand
    {
        public TestCommand() : base()
        {}

        public override string getCommandName()
        {
            throw new NotImplementedException();
        }

        public override bool use(string[] args, PlayerController pc)
        {
            string arg_line = "";
            foreach (string s in args)
                arg_line += s + ",";

            MonoBehaviour.print("Test command executed. args: " + arg_line);
            return true;
        }
    }
}
