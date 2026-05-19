using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IACFPSController.Managers
{
[AddComponentMenu("IAC/Axe Game/Managers/Position Debugger")]
    public class DebugManager : MonoBehaviour
    {

        public Transform[] debugPositions;

        public KeyCode cyclePositions = KeyCode.BackQuote;
        private int currentPosition;

        private StarterAssets.FirstPersonController firstPersonController;

        void Start()
        {
            firstPersonController = FindObjectOfType<StarterAssets.FirstPersonController>();
        }


        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1) && debugPositions.Length > 0)
            {
                currentPosition = 0;

                firstPersonController.TeleportPlayer(debugPositions[0]);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2) && debugPositions.Length > 1)
            {
                currentPosition = 1;
                firstPersonController.TeleportPlayer(debugPositions[1]);

            }
            else if(Input.GetKeyDown(KeyCode.Alpha3) && debugPositions.Length > 2)
            {
                currentPosition = 2;
                firstPersonController.TeleportPlayer(debugPositions[2]);

            }
            else if(Input.GetKeyDown(KeyCode.Alpha4) && debugPositions.Length>3)
            {
                currentPosition = 3;
                firstPersonController.TeleportPlayer(debugPositions[3]);

            }
            else if(Input.GetKeyDown(KeyCode.Alpha5) && debugPositions.Length > 4)
            {
                currentPosition = 4;
                firstPersonController.TeleportPlayer(debugPositions[4]);
            }

            if(Input.GetKeyDown(cyclePositions))
            {
                if(debugPositions.Length <= 0)
                {
                    return;
                }
                
                currentPosition ++;
                if(currentPosition >= debugPositions.Length)
                {
                    currentPosition = 0;
                }

                firstPersonController.TeleportPlayer(debugPositions[currentPosition]);

            }

            
        }
            
    }
}
