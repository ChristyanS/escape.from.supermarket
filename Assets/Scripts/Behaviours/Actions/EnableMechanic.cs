using System.Collections;
using Behaviours.Managers;
using Enuns;
using UnityEngine;
using UnityEngine.UI;

namespace Behaviours.Actions
{
    public class EnableMechanic : MonoBehaviour
    {
        public Mechanics mechanics;
        public Text textComponent;
        public string hintText;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                VirtualInputManager.Instance.EnableControl(mechanics, true);
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