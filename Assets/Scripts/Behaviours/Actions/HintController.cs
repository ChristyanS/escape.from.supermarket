using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Behaviours.Actions
{
    public class HintController : MonoBehaviour
    {
        public TextMeshProUGUI textComponent;
        public string hintText;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                textComponent.text = hintText;
                textComponent.enabled = true;
                StartCoroutine(TimeToShowHint());
            }
        }

        private IEnumerator TimeToShowHint()
        {
            yield return new WaitForSeconds(5);
            textComponent.text = string.Empty;
            textComponent.enabled = false;
            Destroy(gameObject);
        }
    }
}