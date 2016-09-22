namespace Assets.Scripts
{
    class BringToFront : UnityEngine.MonoBehaviour
    {
        void OnEnable()
        {
            transform.SetAsLastSibling(); // bring element to the front
        }
    }
}
