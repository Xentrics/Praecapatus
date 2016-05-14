using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Exception
{
    class GameLogicException : ApplicationException
    {
        public GameLogicException(string msg) : base(msg)
        { }
    }
}
