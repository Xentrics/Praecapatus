using Assets.Scripts.Abilities;
using Assets.Scripts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Commands
{
    class UseAbilityCommand : AbstractCommand
    {
        public static string cmdName = "abi";

        public UseAbilityCommand() : base()
        {
        }

        public override string getCommandName()
        {
            return UseAbilityCommand.cmdName;
        }

        public override bool use(string[] args, PlayerController pc)
        {
            string abiName = args[0];
            foreach (EAbilities A in Enum.GetValues(typeof(EAbilities)))
            {
                if (abiName.Equals(A.ToString())) // find the ability to execute
                {
                    pc.executeAbilityWith(A); // let the player controller handle the rest
                    return true;
                }
            }

            throw new ArgumentException("Could not find ability with name " + abiName);
        }
    }
}
