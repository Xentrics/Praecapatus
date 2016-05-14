using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Commands;
using Assets.Scripts.Exception;
using Assets.Scripts.Player;

namespace Assets.Scripts
{
    class CommandParser
    {
        Dictionary<string, AbstractCommand> commandList;
        PlayerController playerC;

        public CommandParser(PlayerController pc)
        {
            playerC = pc;
            initiate();
        }

        private void initiate()
        {
            // instantiate all commands
            commandList = new Dictionary<string, AbstractCommand>();
            commandList.Add("test", new TestCommand());
            commandList.Add(UseAbilityCommand.cmdName, new UseAbilityCommand());
        }

        public void parseCommandLine(String cmdline, PlayerController pc)
        {
            if (cmdline == null)
                throw new ArgumentNullException("parseCommandLine: given cmdline was empty!");

            String[] input = cmdline.Split(' '); // seperate by empty spaces
            if (input.Length > 0)
            {
                AbstractCommand cmd = null;
                commandList.TryGetValue(input[0], out cmd);
                if (cmd == null)
                    throw new CommandNotFoundException(input[0]);
                else
                    cmd.use(input.Skip(1).ToArray(), pc);
            }
        }
    }
}
