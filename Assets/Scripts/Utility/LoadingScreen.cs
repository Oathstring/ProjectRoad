using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Oathstring.ProjectRoad.Utility
{
    public class LoadingScreen : MonoBehaviour
    {
        private TextMeshProUGUI loadingText;

        private void Awake()
        {
            loadingText = GetComponentInChildren<TextMeshProUGUI>(); 
        }

        /*private void Update()
        {
            
        }*/

        public void LoadScene(string sceneName)
        {
            transform.localScale = Vector3.one;
            StartCoroutine(LoadingAsync(sceneName));
        }

        private IEnumerator LoadingAsync(string sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / .9f) * 100;

                loadingText.text = "Loading...\n" + progress.ToString("0") + "%";
                yield return null;
            }
        }
    }
}
