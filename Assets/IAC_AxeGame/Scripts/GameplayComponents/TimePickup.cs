using System.Collections;
using System.Collections.Generic;
using IACFPSController.Managers;
using UnityEngine;
using UnityEngine.Timeline;

namespace IACFPSController.Gameplay
{
    [AddComponentMenu("IAC/Axe Game/Objects/Time Pickup")]
    public class TimePickup : MonoBehaviour
    {
        [Tooltip("How much time/score to add")]
        public int timeValue = 1;

        [Tooltip("What tag does the player have?")]
        public string playerTag = "Player";

        private bool isPickedUp = false;

        private TimeManager timeController;


        private void Awake()
        {
            timeController = FindObjectOfType<TimeManager>();
            
            isPickedUp = true;
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
                isPickedUp = true;

                AddTimeToPlayer();

            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(isPickedUp)
            {
                return;
            }

            if (collision.collider.CompareTag(playerTag))
            {
                isPickedUp = true;

                AddTimeToPlayer();
            }
        }

        public void AddTimeToPlayer()
        {
            if (timeController)
            {
                timeController.AddTime(timeValue);
                Destroy(this.gameObject);
            }
        }


        public void EnablePickup()
        {
            isPickedUp = false;
        }
    }
}
