using Oathstring.ProjectRoad.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace Oathstring
{
    public class PlayerScore : MonoBehaviour
    {
        private PlayerCarMovement playerMovement;
        private PlayerEffect playerEffect;
        private PlayerStats playerStats;
        private float score;
        private float highestScore;
        private int scoreStack;
        private int highestScoreStack;
        private DifficultyManager difficultyManager;

        [Header("Settings")]
        [SerializeField] float scoreSpeedMultiplier = 200;
        [SerializeField] float maxScore = 9999999;
        [Header("UI")]
        [SerializeField] TextMeshProUGUI scoreText;
        [SerializeField] TextMeshProUGUI highestScoreText;
        [Header("Difficulty Setup")]
        [SerializeField] int maxEasyScore = 10000;
        [SerializeField] int maxNormalScore = 16000;
        [SerializeField] int maxHardScore = 50000;

        private void Awake()
        {
            playerMovement = GetComponent<PlayerCarMovement>();
            playerEffect = GetComponent<PlayerEffect>();
            playerStats = GetComponent<PlayerStats>();
            difficultyManager = FindObjectOfType<DifficultyManager>();

            string saveName = "Highest Score";
            string saveNameStack = "Highest Score Stack";
            highestScore = PlayerPrefs.GetFloat(saveName);
            highestScoreStack = PlayerPrefs.GetInt(saveNameStack);
            if(highestScoreStack > 0)
            {
                highestScoreText.text = "HIGHEST SCORE\n" + highestScore.ToString("0") + " (" + highestScoreStack + ")";
            }

            else
            {
                highestScoreText.text = "HIGHEST SCORE\n" + highestScore.ToString("0");
            }
        }

        private void Update()
        {
            if (!playerStats.Crashed())
            {
                UpdateScore();
                UpdateScoreStack();
                UpdateScoreText();
                UpdateDifficulty();
            }

            else if (playerStats.Crashed())
            {
                string saveName = "Highest Score";
                string saveNameStack = "Highest Score Stack";

                if(highestScoreStack < scoreStack)
                {
                    highestScore = score;
                    highestScoreStack = scoreStack;
                    PlayerPrefs.SetFloat(saveName, highestScore);
                    PlayerPrefs.SetInt(saveNameStack, highestScoreStack);

                    highestScoreText.gameObject.SetActive(true);
                    scoreText.gameObject.SetActive(false);

                    if (highestScoreStack > 0)
                    {
                        highestScoreText.text = "NEW HIGHEST SCORE\n" + highestScore.ToString("0") + " (" + highestScoreStack + ")";
                    }

                    else
                    {
                        highestScoreText.text = "NEW HIGHEST SCORE\n" + highestScore.ToString("0");
                    }
                }

                else if (highestScore < score && highestScoreStack <= scoreStack)
                {
                    highestScore = score;
                    PlayerPrefs.SetFloat(saveName, highestScore);

                    highestScoreText.gameObject.SetActive(true);
                    scoreText.gameObject.SetActive(false);

                    if (highestScoreStack > 0)
                    {
                        highestScoreText.text = "NEW HIGHEST SCORE\n" + highestScore.ToString("0") + " (" + highestScoreStack + ")";
                    }

                    else
                    {
                        highestScoreText.text = "NEW HIGHEST SCORE\n" + highestScore.ToString("0");
                    }
                }

                else
                {
                    highestScoreText.gameObject.SetActive(true);
                    scoreText.text = "SCORE\n" + score.ToString("0");
                }
            }
        }

        private void UpdateScore()
        {
            score += Time.deltaTime * scoreSpeedMultiplier * difficultyManager.GetDifficultyMultiply() * playerEffect.GetSpeedupEffectMultiply();
        }

        private void UpdateScoreStack()
        {
            if(score >= maxScore)
            {
                score = 0;
                scoreStack++;
            }
        }

        private void UpdateScoreText()
        {
            if (scoreStack > 0)
            {
                scoreText.text = score.ToString("0") + " (" + scoreStack + ")";
            }

            else
            {
                scoreText.text = score.ToString("0");
            }
        }

        private void UpdateDifficulty()
        {
            if (score >= maxHardScore) difficultyManager.SetDifficulty(Difficulty.VeryHard);
            else if (score >= maxNormalScore) difficultyManager.SetDifficulty(Difficulty.Hard);
            else if (score >= maxEasyScore) difficultyManager.SetDifficulty(Difficulty.Normal);
        }
    }
}
