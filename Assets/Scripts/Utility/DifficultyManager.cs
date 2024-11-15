using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Oathstring.ProjectRoad.Utility
{
    public class DifficultyManager : MonoBehaviour
    {
        private float difficultyMultiply = 1;
        private float nextDifficultyMultiply = 1;
        private readonly float difficultyBuffer = 15;

        private Difficulty difficulty;
        private Difficulty nextDifficulty;
        private SpawnEvent spawnEvent;

        [Header("Settings")]
        [SerializeField] float normalMultiply = 1.2f;
        [SerializeField] float hardMultiply = 1.15f;
        [SerializeField] float veryHardMultiply = 1.22f;
        
        private void Awake()
        {
            difficulty = Difficulty.Easy;
            spawnEvent = FindObjectOfType<SpawnEvent>();
            spawnEvent.SetSpawnInterval(0.9f, 1.8f);
        }

        private void Update()
        {
            if (nextDifficulty != difficulty)
            {
                difficulty = nextDifficulty;
                OnDifficultyChanged();
            }

            if (difficultyMultiply < nextDifficultyMultiply) difficultyMultiply += Time.deltaTime / difficultyBuffer;
        }

        private void OnDifficultyChanged()
        {
            if(difficulty == Difficulty.Normal)
            {
                nextDifficultyMultiply *= normalMultiply;
                spawnEvent.SetSpawnInterval(0.7f, 1.6f);
            }

            else if (difficulty == Difficulty.Hard)
            {
                nextDifficultyMultiply *= hardMultiply;
                spawnEvent.SetSpawnInterval(0.5f, 1.4f);
            }

            else if (difficulty == Difficulty.VeryHard)
            {
                nextDifficultyMultiply *= veryHardMultiply;
                spawnEvent.SetSpawnInterval(0.2f, .9f);
            }
        }

        public float GetDifficultyMultiply() => difficultyMultiply;

        public void SetDifficulty(Difficulty difficulty)
        {
            nextDifficulty = difficulty;
        }
    }

    public enum Difficulty
    {
        Easy, Normal, Hard, VeryHard
    }
}
