using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Entity;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Commands
{
    class WriteLineCommand : AbstractCommand
    {
        public static readonly string cmdName = "w";

        public override string getCommandName()
        {
            return WriteLineCommand.cmdName;
        }

        public override bool use(string[] args, PlayerController pc)
        {
            Constants.chatManager.addLine( (args == null) ? null : String.Join(" ", args));
            return true;
        }
    }
}
