using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IACFPSController
{
    [AddComponentMenu("IAC/Axe Game/Managers/Pause Manager")]
    public class PauseManager : MonoBehaviour
    {

        public KeyCode pauseKey = KeyCode.Escape;

        public UnityEvent openPauseMenu, closePauseMenu;

        private bool isPaused = false;

        public void Update()
        {
            if (Input.GetKeyDown(pauseKey))
            {
                PauseGame();
            }
        }

        public void PauseGame()
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                Time.timeScale = 0;
                openPauseMenu.Invoke();
            }
            else
            {
                Time.timeScale = 1;
                closePauseMenu.Invoke();
            }

            ToggleMouseCursor(isPaused);
        }


        public void ToggleMouseCursor(bool onOff)
        {
            if (!onOff)
            { Cursor.lockState = CursorLockMode.Locked; }
            else
            { Cursor.lockState = CursorLockMode.None; }

            Cursor.visible = onOff;
        }

    }
}