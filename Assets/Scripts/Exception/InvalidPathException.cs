namespace Assets.Scripts.Exception
{
    class InvalidPathException : System.Exception
    {
        public InvalidPathException(string err_str) : base(err_str) { }
    }
}
