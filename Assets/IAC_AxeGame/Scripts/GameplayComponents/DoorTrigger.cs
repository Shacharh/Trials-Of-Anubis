using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IACFPSController.Gameplay;

namespace IACFPSController.Gameplay
{
[AddComponentMenu("IAC/Axe Game/Objects/Door Trigger")]
    public class DoorTrigger : MonoBehaviour
    {
        DoorMechanic doorController;

        public string triggerTag = "Player";

        void Start()
        {
            doorController = GetComponentInParent<DoorMechanic>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag(triggerTag))
            {
                doorController.NearDoor(true);
            }
        }

        
        private void OnTriggerExit(Collider other)
        {
             if(other.CompareTag(triggerTag))
            {
                doorController.NearDoor(false);
            }
        }
    }
}
