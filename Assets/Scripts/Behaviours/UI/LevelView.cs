using Behaviours.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Behaviours.UI
{
    public class LevelView : MonoBehaviour
    {
        public int scene;

        void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.R))
            {
                SceneManager.LoadScene(scene);
                VirtualInputManager.Instance.EnableAllControls(true);
            }

            if (Input.GetKey(KeyCode.Alpha1))
            {
                SceneManager.LoadScene(1);
                VirtualInputManager.Instance.EnableAllControls(true);
            }

            if (Input.GetKey(KeyCode.Alpha2))
            {
                SceneManager.LoadScene(2);
                VirtualInputManager.Instance.EnableAllControls(true);
            }
        }
    }
}