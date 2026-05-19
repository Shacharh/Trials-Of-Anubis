using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace IACFPSController.Gameplay
{
    [AddComponentMenu("IAC/Axe Game/Objects/Key Pickup")]
    public class KeyPickUp : MonoBehaviour
    {
        [Tooltip("What tag does the player have?")]
        public string playerTag = "Player";

        public DoorMechanic lockedDoor;

        public bool visualizeKeyUnlocked;

        [Header("Visaulize Unlocked")]
        public GameObject unlockedKeyObject;

        private bool isPickedUp = false;

        private void Start()
        {            
            isPickedUp = true;


            if(visualizeKeyUnlocked && unlockedKeyObject)
            {
                unlockedKeyObject.SetActive(false);
            }
            Invoke("EnablePickup",1f);
        }


        private void OnTriggerEnter(Collider other)
        {
            if(isPickedUp)
            {
                return;
            }

            if(other.CompareTag(playerTag))
            {
                lockedDoor.AcquiredKey();
                isPickedUp = true;
                if(visualizeKeyUnlocked)
                {
                    PlaceKeyVisually();
                }
                Destroy(this.gameObject);
               
            }
        }

        public void PlaceKeyVisually()
        {
            if(unlockedKeyObject)
            {
                unlockedKeyObject.SetActive(true);
            }
        }

        public void EnablePickup()
        {
            isPickedUp = false;
        }
    }
}

