using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IACFPSController.Gameplay
{
[AddComponentMenu("IAC/Axe Game/Objects/Door Mechanic")]
    public class DoorMechanic : MonoBehaviour
    {

        public UnityEvent unlockEvent;

        public KeyCode unlockInput = KeyCode.E;
        public GameObject inputHint;
        
        public int numberOfKeys = 1;

        private bool hasKeys = true;
        private bool isInvoked = false;
        private bool canUnlock = false;
        private bool doorOpen = false;
        public bool canCloseDoor = false;

        private int remainingKeys;

        public UnityEvent closeDoorEvent;
        


        void Start()
        {
            hasKeys = false;
            isInvoked = false;
            canUnlock = false;
            doorOpen = false;
            remainingKeys = numberOfKeys;
            
            if(remainingKeys == 0)
            {
                hasKeys = true;
            }
        }

         void Update()
        {
            if(!canUnlock)
            {
                return;
            }
            if(hasKeys && !isInvoked)
            {
                if(unlockInput == KeyCode.None)
                {
                    OpenDoor();
                }
                else if(Input.GetKeyDown(unlockInput))
                {
                    OpenDoor();
                }
            }
            if(doorOpen && canCloseDoor)
            {
                if(unlockInput == KeyCode.None)
                {
                    CloseDoor();
                }
                else if(Input.GetKeyDown(unlockInput))
                {
                    CloseDoor();
                }
            }
        }

        public void NearDoor(bool isNear)
        {
            canUnlock = isNear;
            if(inputHint)
            {
                inputHint.SetActive(isNear);
            }
        }

        public void AcquiredKey ()
        {
            remainingKeys --;

            if(remainingKeys <= 0)
            {
                hasKeys = true;
            }
        }

        public void OpenDoor()
        {
            if(!doorOpen)
            {
                unlockEvent.Invoke();
            }
            StartCoroutine(DelayedDoorState(true));
        }
        public void CloseDoor()
        {
            if(doorOpen)
            {
                closeDoorEvent.Invoke();
            }
            StartCoroutine(DelayedDoorState(false));
        }

    private IEnumerator DelayedDoorState(bool state)
		{
            yield return new WaitForSeconds(0.1f);

            doorOpen = state;   
            isInvoked = state; 
		}
    }
}
