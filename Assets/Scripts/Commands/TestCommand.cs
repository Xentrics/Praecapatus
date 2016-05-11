using UnityEngine;

namespace Assets.Scripts.Commands
{
    /*
     * a simple test command
     * can be used to test the correctness of the command parser
     */
    class TestCommand : Command
    {
        public TestCommand() : base("test")
        {}

        public TestCommand(string commandName) : base(commandName)
        {}

        public override bool use(string[] args)
        {
            string arg_line = "";
            foreach (string s in args)
                arg_line += s + ",";

            MonoBehaviour.print("Test command executed. args: " + arg_line);
            return true;
        }
    }
}
