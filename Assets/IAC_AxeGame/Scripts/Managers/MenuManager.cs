using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IACFPSController.Managers
{

    [AddComponentMenu("IAC/Axe Game/Managers/Menu Manager")]
    public class MenuManager : MonoBehaviour
    {

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.F5))
            {
                ResetScene();
            }
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Exit();
            }
        }

        public void LoadScene(int sceneNum)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(sceneNum);
        }

        public void ResetScene ()
        {
            int thisSceneNum = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(thisSceneNum);
        }

        public void Exit()
        {
            Application.Quit();

            #if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
            #endif
        }

    }
    
}