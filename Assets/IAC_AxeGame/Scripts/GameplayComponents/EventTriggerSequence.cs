using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IACFPSController.Gameplay
{
    [AddComponentMenu("IAC/Axe Game/Objects/EventTriggerSequence")]
    public class EventTriggerSequence : MonoBehaviour
    {
        public string playerTag = "Player";

        public UnityEvent[] triggeredEvents;

        public float initialDelay = 0f;
        public float delayedTriggerTime = 0f;
        public bool triggerOnce = false;
        private bool triggered = false;

        private void OnEnable()
        {
            triggered = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag(playerTag) && !triggered)
            {
                triggered = true;
                StartCoroutine("CallEvent");
                
            }    
        }

        private IEnumerator CallEvent()
        {
            yield return new WaitForSeconds(initialDelay); // Delay for 2 seconds

            for (int i = 0; i < triggeredEvents.Length; i++)
            {
                triggeredEvents[i].Invoke();
           

                yield return new WaitForSeconds(delayedTriggerTime); // Delay for 2 seconds

            }
            if(triggerOnce)
            {
                Destroy(this.gameObject);
            }
            else
            {
                triggered = false;
            }
        }
    }
}