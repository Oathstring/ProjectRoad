using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Oathstring
{
    public class PlayerStats : MonoBehaviour
    {
        private delegate void StatsEvent();
        private StatsEvent statsEvent;

        private PlayerEffect playerEffect;

        private bool crash;

        private int heart;
        private readonly int minHeart = 0;

        private int speedBoost;
        private readonly int minSpeedBoost = 0;
        private readonly int defaultSpeedBoost = 3;

        [Header("Stats")]
        [SerializeField] int maxHeart = 5;
        [SerializeField] int maxSpeedBoost = 2;

        [Header("Ability Input Key")]
        [SerializeField] KeyCode speedBoostKey = KeyCode.Space;

        [Header("UI")]
        [SerializeField] Transform heartHolder;
        [SerializeField] Transform speedBoostHolder;
        [SerializeField] GameObject crashedBackground;

        private void Awake()
        {
            playerEffect = GetComponent<PlayerEffect>();

            heart = maxHeart;
            speedBoost = defaultSpeedBoost;

            statsEvent += StatsRange;
            statsEvent += UpdateHeartImage;
            statsEvent += UpdateSpeedBoostImage;
            statsEvent += OnCrashEffect;
        }

        private void Update()
        {
            statsEvent();

            if (Input.GetKeyDown(speedBoostKey) && !playerEffect.HasSpeedup() && !crash && speedBoost > 0)
            {
                playerEffect.SpeedupStart(6);
                speedBoost--;
            }
        }

        private void StatsRange()
        {
            if (heart <= minHeart) heart = 0;
            if (speedBoost <= minSpeedBoost) speedBoost = 0;

            if (heart >= maxHeart) heart = maxHeart;
            if (speedBoost >= maxSpeedBoost) speedBoost = maxSpeedBoost;
        }

        private void UpdateHeartImage()
        {
            // update heart image display
            for (int hIndex = 0; hIndex < heartHolder.childCount; hIndex++)
            {
                if (hIndex < heart)
                {
                    heartHolder.GetChild(hIndex).gameObject.SetActive(true);
                }

                else
                {
                    heartHolder.GetChild(hIndex).gameObject.SetActive(false);
                }
            }
        }

        private void UpdateSpeedBoostImage() 
        {
            // update speed boost image display
            for (int sbIndex = 0; sbIndex < speedBoostHolder.childCount; sbIndex++)
            {
                if (sbIndex < speedBoost)
                {
                    speedBoostHolder.GetChild(sbIndex).gameObject.SetActive(true);
                }

                else
                {
                    speedBoostHolder.GetChild(sbIndex).gameObject.SetActive(false);
                }
            }
        }

        private void OnCrashEffect()
        {
            if (crash)
            {
                playerEffect.StartEngineSmoke();
                crashedBackground.SetActive(true);
            }
        }

        public bool Crashed() => crash;
        public int GetHeart() => heart;
        public int GetSpeedBoost() => speedBoost;
        public int GetMaxHeart() => maxHeart;
        public int GetMaxSpeedBoost() => maxSpeedBoost;
        public void Damage()
        {
            heart--;
        }

        public void AddHeart(int count)
        {
            heart += count;
        }

        public void AddSpeedBoost(int count)
        {
            speedBoost += count;
        }

        public void SetCrashStat(bool stat)
        {
            crash = stat;
        }

        public void SpeedBoostBtn()
        {
            if (!playerEffect.HasSpeedup() && !crash && speedBoost > 0)
            {
                playerEffect.SpeedupStart(6);
                speedBoost--;
            }
        }
    }
}
