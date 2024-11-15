using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Oathstring.ProjectRoad.Utility
{
    // for development only
    public class TestingShortcut : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
#if UNITY_EDITOR
            Application.targetFrameRate = 60;
#endif
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q))
            {
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                print("Quit");
#endif 
            }
             
            if (Input.GetKeyDown(KeyCode.R))
            { 
                SceneManager.LoadScene(1);
                Time.timeScale = 1;
            }
        }
    }
}
