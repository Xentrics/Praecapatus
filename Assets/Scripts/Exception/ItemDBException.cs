namespace Assets.Scripts.Exception
{
    class ItemDBException : System.Exception
    {
        public int itemId;

        public ItemDBException() : base("Cannot find item in database")
        { }

        public ItemDBException(int itemId) : base("Cannot find item in database")
        {
            this.itemId = itemId;
        }

        public override string ToString()
        {
            return "Cannot find item in database with id : [" + itemId + "]";
        }
    }
}
