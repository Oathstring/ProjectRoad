using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Oathstring.ProjectRoad.Utility
{
    public class Menu : MonoBehaviour
    {
        private delegate void DecisionEvent();
        private DecisionEvent decision;

        private bool paused;
        private GameObject pauseMenu;
        private GameObject options;
        private GameObject decisionMaking;
        private PlayerStats playerStats;

        private AudioBus audioBus;

        [SerializeField] MenuType type;
        [Space]
        [SerializeField] ToggleGroup graphicToggleGroup;
        [Space]
        [SerializeField] GameObject middleGroup;
        [SerializeField] Toggle vsyncToggle;
        [Header("Main Menu")]
        [SerializeField] TextMeshProUGUI scoreText;
        [Header("Pause Menu")]
        [SerializeField] Button resumeBtn;
        [SerializeField] Button pauseBtn;
        [SerializeField] Button speedBoostBtn;

        private void Start()
        {
            if(type == MenuType.MainMenu)
            {
                string saveName = "Highest Score";
                string saveNameStack = "Highest Score Stack";

                if (PlayerPrefs.HasKey(saveName))
                {
                    float score = PlayerPrefs.GetFloat(saveName);
                    int scoreStack = PlayerPrefs.GetInt(saveNameStack);

                    scoreText.gameObject.SetActive(true);
                    if(scoreStack > 0)
                    {
                        scoreText.text = "HIGHEST SCORE\n" + score.ToString("0") + " (" + scoreStack + ")";
                    }

                    else if(score > 0)
                    {
                        scoreText.text = "HIGHEST SCORE\n" + score.ToString("0");
                    }

                    else
                    {
                        scoreText.gameObject.SetActive(false);
                    }
                }

                else
                {
                    scoreText.gameObject.SetActive(false);
                }
            }

            else
            {
                pauseMenu = transform.GetChild(0).gameObject;
                options = transform.GetChild(1).gameObject;


                pauseMenu.transform.localScale = Vector3.zero;
                options.transform.localScale = Vector3.zero;

                playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
            }

            decisionMaking = transform.GetChild(2).gameObject;
            decisionMaking.transform.localScale = Vector3.zero;

            audioBus = GameObject.FindGameObjectWithTag("Audio Bus").GetComponent<AudioBus>();

#if !UNITY_STANDALONE
            middleGroup.SetActive(false);
            vsyncToggle = null;
#endif
            LoadGraphicSettings();
            LoadDisplaySettings();
            LoadAudioVolumeSettings();
        }

        private void LoadGraphicSettings()
        {
            string saveName = "Graphic Toggle";
            int indexSaved = PlayerPrefs.GetInt(saveName, 2);
            graphicToggleGroup.transform.GetChild(indexSaved).GetComponent<Toggle>().isOn = true;
        }

        private void LoadDisplaySettings()
        {
            string saveName = "Vsync Toggle";
            int vsyncCount = PlayerPrefs.GetInt(saveName, 1);
            
            if(vsyncToggle) vsyncToggle.isOn = Convert.ToBoolean(vsyncCount);
        }

        private void LoadAudioVolumeSettings()
        {
            Slider[] sliderVolumes = GetComponentsInChildren<Slider>();

            foreach(Slider sliderVolume in sliderVolumes)
            {
                string saveName = sliderVolume.transform.parent.name;
                sliderVolume.value = PlayerPrefs.GetFloat(saveName, .5f);

                audioBus.SetVolume(sliderVolume.value, saveName); // value and slider volume name
            }
        }

        private void Update()
        {
            if(type == MenuType.PauseMenu)
            {
                // input trigger
                if (Input.GetKeyDown(KeyCode.Escape) && !playerStats.Crashed())
                {
                    paused = true;

                    if (pauseMenu.transform.localScale == Vector3.one)
                    {
                        paused = !paused;
                    }

                    PauseMenu();
                }

                else if (playerStats.Crashed() &&  !paused)
                {
                    paused = true;
                    resumeBtn.interactable = !playerStats.Crashed();

                    PauseMenu();
                }

                // time update
                if (paused && !playerStats.Crashed())
                {
                    Time.timeScale = 0;
                }

                else if(!playerStats.Crashed())
                {
                    Time.timeScale = 1;
                }

#if !UNITY_STANDALONE
                pauseBtn.gameObject.SetActive(!paused); // set active by reverse paused state
                speedBoostBtn.gameObject.SetActive(!paused);
#else
                pauseBtn.gameObject.SetActive(false); // set active by reverse paused state
                speedBoostBtn.gameObject.SetActive(false);
#endif
            }
        }

        private void PauseMenu()
        {
            // visual update
            if (options.transform.localScale != Vector3.one)
            {
                if (paused)
                {
                    pauseMenu.transform.localScale = Vector3.one;
                    options.transform.localScale = Vector3.zero;
                }

                else
                {
                    pauseMenu.transform.localScale = Vector3.zero;
                    options.transform.localScale = Vector3.zero;
                }
            }

            else
            {
                pauseMenu.transform.localScale = Vector3.one;
                options.transform.localScale = Vector3.zero;
            }
        }

        public void ResumeBtn()
        {
            paused = false;

            pauseMenu.transform.localScale = Vector3.zero;
            options.transform.localScale = Vector3.zero;
        }

        public void PauseBtn()
        {
            paused = true;

            if (pauseMenu.transform.localScale == Vector3.one)
            {
                paused = !paused;
            }

            PauseMenu();
        }

        public void OptionsBtn()
        {
            pauseMenu.transform.localScale = Vector3.zero;
            options.transform.localScale = Vector3.one;
        }

        public void RestartBtn()
        {
            if (type == MenuType.PauseMenu)
            {
                PlayerStats playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

                if (playerStats.Crashed())
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }

                else
                {
                    DecisionTrigger("Restart Game");
                }
            }
        }

        public void CloseBtn()
        {
            pauseMenu.transform.localScale = Vector3.one;
            options.transform.localScale = Vector3.zero;
        }

        //decision btn
        public void DecisionTrigger(string decisionSelected)
        {
            decisionMaking.transform.localScale = Vector3.one;

            if(decisionSelected == "Restart Game")
            {
                decision += Restart;
            }

            else if(decisionSelected == "Reset Settings")
            {
                decision += ResetSettings;
            }

            else if(decisionSelected == "Return To Menu")
            {
                decision += ReturnToMenu;
            }

            else if(decisionSelected == "Quit")
            {
                decision += Quit;
            }
        }

        public void YesBtn()
        {
            decision();
            decision = null;
            decisionMaking.transform.localScale = Vector3.zero;
        }

        public void NoBtn()
        {
            decision = null;
            decisionMaking.transform.localScale = Vector3.zero;
        }

        private void ResetSettings()
        {
            Slider[] sliderVolumes = GetComponentsInChildren<Slider>();

            foreach (Slider sliderVolume in sliderVolumes)
            {
                string sliderSaveName = sliderVolume.transform.parent.name;
                PlayerPrefs.DeleteKey(sliderSaveName);
            }

            string graphicToggleSaveName = "Graphic Toggle";
            PlayerPrefs.DeleteKey(graphicToggleSaveName);

            string vsyncToggleSaveName = "Vsync Toggle";
            PlayerPrefs.DeleteKey(vsyncToggleSaveName);

            LoadGraphicSettings();
            
            if(vsyncToggle) LoadDisplaySettings();

            LoadAudioVolumeSettings();
        }

        private void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void ReturnToMenu()
        {
            LoadingScreen loadingScreen = GameObject.FindGameObjectWithTag("Loading Object").GetComponent<LoadingScreen>();
            loadingScreen.LoadScene("Menu");
            paused = false;
        }

        private void Quit()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            print("Quit");
#endif
        }

        public void OnGraphicToggleChanged()
        {
            string saveName = "Graphic Toggle";

            for (int i = 0; i < graphicToggleGroup.transform.childCount; i++)
            {
                if(graphicToggleGroup.transform.GetChild(i).name == graphicToggleGroup.GetFirstActiveToggle().name)
                {
                    QualitySettings.SetQualityLevel(i);
                    PlayerPrefs.SetInt(saveName, i);
                    if (vsyncToggle) LoadDisplaySettings();
                }
            }
        }

        public void OnVsyncToggleChanged()
        {
            string saveName = "Vsync Toggle";

            QualitySettings.vSyncCount = Convert.ToInt32(vsyncToggle.isOn);
            PlayerPrefs.SetInt(saveName, QualitySettings.vSyncCount);

            //print(QualitySettings.vSyncCount);
        }

        public void OnVolumeChanged(Slider volumeType)
        {
            string saveName = volumeType.transform.parent.name;
            PlayerPrefs.SetFloat(saveName, volumeType.value);

            audioBus.SetVolume(volumeType.value, saveName); // value and slider volume name
        }

        public bool Paused() => paused;
    }

    public enum MenuType
    {
        MainMenu, PauseMenu
    }
}
