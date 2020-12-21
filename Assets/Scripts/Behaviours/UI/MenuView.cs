using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Behaviours.UI
{
    public class MenuView : MonoBehaviour
    {
        public GameObject loadPanel;

        private void Update()
        {
            if (Input.GetKey(KeyCode.Return))
            {
                loadPanel.SetActive(true);
                StartCoroutine(LoadScene());
            }
        }
        
        private IEnumerator LoadScene()
        {
            var scene = SceneManager.LoadSceneAsync(1);
            while (!scene.isDone)
            {
                yield return null;
                Time.timeScale = 1f;
            }
        }
    }
}