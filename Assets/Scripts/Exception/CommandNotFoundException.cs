using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Exception
{
    class CommandNotFoundException : NullReferenceException
    {
        public CommandNotFoundException(string commandName) : base("Could not find command: " + commandName)
        {}
    }
}
