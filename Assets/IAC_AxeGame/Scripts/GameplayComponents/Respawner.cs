using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IACFPSController.Managers;


namespace IACFPSController.Gameplay
{
    [AddComponentMenu("IAC/Axe Game/Objects/Respawn Zone")]
    public class Respawner : MonoBehaviour
    {
        private TimeManager timeManager;

        public string playerTag;// = "Player";

        [Header ("Zone Settings")]
        public bool loseTime = true;
        public bool respawnPlayer = true;

        [Header ("Zone Values")]
        public float timeValue = -5f;

        public Transform respawnLocation;
        private Transform playerObject;
        private StarterAssets.FirstPersonController firstPersonController;
        private IEnumerator coroutine;


        private void Awake()
        {
            timeManager = FindObjectOfType<TimeManager>();
            firstPersonController = FindObjectOfType<StarterAssets.FirstPersonController>();
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                if (!other.CompareTag("Player"))
                {
                    other.transform.position = respawnLocation.position;
                    return;
                }

                playerObject = other.gameObject.transform;

                if (timeManager && loseTime)
                {
                    timeManager.AddTime(timeValue);
                }
                if(respawnPlayer && firstPersonController)
                {
                    firstPersonController.TeleportPlayer(respawnLocation);
                }
            }
        }

    }
}
