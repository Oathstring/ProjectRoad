using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oathstring
{
    public class VolumeControl : MonoBehaviour
    {
        private AudioBus audioBus;
        private AudioSource audioSource;

        [SerializeField] AudioType type;

        private void Awake()
        {
            audioBus = GameObject.FindGameObjectWithTag("Audio Bus").GetComponent<AudioBus>();
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if(audioSource.volume != audioBus.GetVolume(type))
            {
                audioSource.volume = audioBus.GetVolume(type);
            }
        }
    }

    public enum AudioType
    {
        SFX, Music
    }
}
