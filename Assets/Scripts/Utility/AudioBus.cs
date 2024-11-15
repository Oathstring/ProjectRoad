using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Oathstring
{
    public class AudioBus : MonoBehaviour
    {
        private delegate void AudioBusEvent();
        private AudioBusEvent audioBusEvent;

        private GameObject sFXObj;
        private AudioSource[] sFXSources;
        private int registeredSFX;

        private GameObject musicObj;
        private AudioSource[] musicSources;
        private int registeredMusic;

        [Header("Volumes")]
        [SerializeField][Range(0, 1)] float master;
        [SerializeField][Range(0, 1)] float sFX;
        [SerializeField][Range(0, 1)] float music;

        private void Awake()
        {
            // Getting all ready
            sFXObj = transform.GetChild(0).gameObject;
            musicObj = transform.GetChild(1).gameObject;

            // registering SFX
            sFXSources = sFXObj.GetComponentsInChildren<AudioSource>();
            registeredSFX = sFXSources.Length;

            // registering Music
            musicSources = musicObj.GetComponentsInChildren<AudioSource>();
            registeredMusic = musicSources.Length;

            // add event 
            audioBusEvent += RegisterAddedSFX;
            audioBusEvent += RegisterAddedMusic;
            audioBusEvent += OnSFXVolumeChanged;
            audioBusEvent += OnMusicVolumeChanged;
        }

        private void Update()
        {
            audioBusEvent();
        }

        // events
        private void RegisterAddedSFX() // register new sfx
        {
            if (registeredSFX != sFXObj.transform.childCount)
            {
                sFXSources = sFXObj.GetComponentsInChildren<AudioSource>();
            }
        }

        private void RegisterAddedMusic() // register new music
        {
            if (registeredMusic != musicObj.transform.childCount)
            {
                musicSources = musicObj.GetComponentsInChildren<AudioSource>();
            }
        }

        private void OnSFXVolumeChanged()
        {
            foreach(AudioSource sfxSource in sFXSources)
            {
                if(sfxSource.volume != sFX * master)
                {
                    sfxSource.volume = sFX * master;
                }
            }
        }

        private void OnMusicVolumeChanged()
        {
            foreach (AudioSource musicSource in musicSources)
            {
                if(musicSource.volume != music * master)
                {
                    musicSource.volume = music * master;
                }
            }
        }

        // functions
        public void BtnSound(AudioClip clip)
        {
            for (int i = 0; i < sFXSources.Length; i++)
            {
                if (sFXSources[i].name == "UI_Button")
                {
                    AudioSource ui_Btn = sFXSources[i];
                    ui_Btn.PlayOneShot(clip);
                }
            }
        }

        public void SetVolume(float value, string volumeType)
        {
            if(volumeType == "MasterVol") // must same as slider parent name
            {
                master = value;
            }

            else if(volumeType == "SFXVol") // must same as slider parent name
            {
                sFX = value;
            }

            else if(volumeType == "MusicVol") // must same as slider parent name
            {
                music = value;
            }
        }

        public float GetVolume(AudioType type)
        {
            if(type == AudioType.SFX)
            {
                return sFX * master;
            }

            else if(type == AudioType.Music)
            {
                return music * master;
            }

            return 0;
        }
    }
}
