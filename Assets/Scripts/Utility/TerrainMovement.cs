using Oathstring.ProjectRoad.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Oathstring
{
    public class TerrainMovement : MonoBehaviour
    {
        const int MOVEDIR = -1;

        private DifficultyManager difficultyManager;
        private PlayerEffect playerEffect;
        private PlayerStats playerStats;
        float speed;

        [SerializeField] float defaultSpeed = 20;
        [SerializeField] float moveEdge = -1500;

        [SerializeField] Vector3 seamlessPos = new(0,0, 2000f);

        private void Awake()
        {
            playerEffect = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEffect>();
            playerStats = playerEffect.GetComponent<PlayerStats>();
        }

        private void FixedUpdate()
        {
            if (transform.position.z <= moveEdge)
            {
                transform.position += seamlessPos;
            }

            speed = defaultSpeed * difficultyManager.GetDifficultyMultiply() * playerEffect.GetSpeedupEffectMultiply();
        }

        private void OnEnable()
        {
            difficultyManager = FindObjectOfType<DifficultyManager>();
        }

        private void LateUpdate()
        {
            if (!playerStats.Crashed())
            {
                transform.Translate(0, 0, MOVEDIR * speed * Time.deltaTime);
            }
        }

        public float GetDefaultSpeed() => defaultSpeed;
    }
}
