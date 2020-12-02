using UnityEngine;
using UnityEngine.SceneManagement;

namespace Behaviours.UI
{
    public class MenuView : MonoBehaviour
    {

        private void Update()
        {
            if (Input.anyKey)
            {
                SceneManager.LoadScene(1);
            }
        }
    }
}
