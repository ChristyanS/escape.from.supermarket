using UnityEngine;
using UnityEngine.SceneManagement;

namespace Behaviours.UI
{
    public class MenuView : MonoBehaviour
    {

        private void Update()
        {
            if (Input.GetKey(KeyCode.Return))
            {
                SceneManager.LoadScene(1);
            }
        }
    }
}
