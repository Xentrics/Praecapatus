using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Commands;
using Assets.Scripts.Exception;

namespace Assets.Scripts
{
    static class CommandParser
    {
        static bool isInitiated = false;
        static Dictionary<string, Command> commandList;

        private static void initiate()
        {
            // instantiate all commands
            commandList = new Dictionary<string, Command>();
            commandList.Add("test", new TestCommand());

            isInitiated = true;
        }

        public static void parseCommandLine(String cmdline)
        {
            if (!isInitiated)
                initiate();

            if (cmdline == null)
                throw new ArgumentNullException("parseCommandLine: given cmdline was empty!");

            String[] input = cmdline.Split(' '); // seperate by empty spaces
            if (input.Length > 0)
            {
                Command cmd = null;
                commandList.TryGetValue(input[0], out cmd);
                if (cmd == null)
                    throw new CommandNotFoundException(input[0]);
                else
                    cmd.use(input.Skip(1).ToArray());
            }
        }

    }
}
