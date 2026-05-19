using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IACFPSController.Gameplay
{
    [AddComponentMenu("IAC/Axe Game/Objects/Elevator Parenting")]
    public class ElevatorParenting : MonoBehaviour
    {
        public string playerTag = "Player";
        private FirstPersonController _firstPersonScript;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                if (_firstPersonScript == null)
                    _firstPersonScript = other.GetComponent<FirstPersonController>();
                _firstPersonScript.ChangeState(CharacherState.Elavator);
            }
        }


        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                if (_firstPersonScript == null)
                    _firstPersonScript = other.GetComponent<FirstPersonController>();
                _firstPersonScript.ReturnToOldState();

            }
        }

    }
}