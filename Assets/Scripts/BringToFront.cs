namespace Assets.Scripts
{
    class BringToFront : UnityEngine.MonoBehaviour
    {
        void OnEngable()
        {
            transform.SetAsLastSibling(); // bring element to the front
        }
    }
}
