using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Exception
{
    class InvalidAttributeLevelException : System.ArgumentOutOfRangeException
    {
        public InvalidAttributeLevelException(string cause) : base(cause, "Attribute level out of bounds caused by: " + cause)
        { }
    }
}
