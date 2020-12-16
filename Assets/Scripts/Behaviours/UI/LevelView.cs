using UnityEngine;

namespace Behaviours.UI
{
    public class LevelView : MonoBehaviour
    {
        void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

    }
}
