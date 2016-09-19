namespace Assets.Scripts
{
    [System.Obsolete("PraeObject has been deprecated. Use 'InteractionComponent' instead!", true)]
    public interface Interactable
    {
        float weight
        {
            get;
            set;
        }

        bool inanimate
        {
            get;
            set;
        }

        UnityEngine.Vector3 position    // transform position
        {
            get;
        } 

        float meleeRange                // only important for entities or any kind of attacker
        {
            get;
        }

        string description
        {
            get;
        }

        bool addInteraction(object interaction); // add a stuff like dialogs, knowledge, etc. here //TODO: needs more specification later on
        bool hasInteraction();                   
        bool tryInteract(PraeObject caller);   // test for existing interactions and prepare interaction interface stuff
    }
}
