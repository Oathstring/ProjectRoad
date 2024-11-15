using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oathstring.ProjectRoad.Utility
{
    public class SpawnEvent : MonoBehaviour
    {
        private SpawnManager spawnManager;
        private BoxCollider areaBoxCollider;

        private PlayerStats playerStats;

        private Vector3 minSpawnPos;
        private Vector3 maxSpawnPos;

        private float maxTime = 2; // in second
        private float minTime = 1; // in second

        private readonly int additionalTime = 50;

        //private DifficultyManager difficultyManager;

        [Header("Settings")]
        [SerializeField] GameObject spawnArea;

        private void Awake()
        {
            spawnManager = GetComponent<SpawnManager>();
            areaBoxCollider = spawnArea.GetComponent<BoxCollider>();

            playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

            //difficultyManager = FindObjectOfType<DifficultyManager>();

            StartCoroutine(CarTypeSpawnLoop());
            StartCoroutine(PowerupTypeSpawnLoop());
        }

        private void Update()
        {
            minSpawnPos = areaBoxCollider.bounds.min;
            maxSpawnPos = areaBoxCollider.bounds.max;

            if (playerStats.Crashed()) StopAllCoroutines();
        }

        private void CarTypeSpawn()
        {
            spawnManager.SpawnCar(minSpawnPos.z, maxSpawnPos.z); // success (not complete)
            // prevent from spawning on camera by sphere collider on player car
        }

        private IEnumerator CarTypeSpawnLoop()
        {
            //float difficultyMultiply = difficultyManager.GetDifficultyMultiply();
            float counting = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(counting);

            CarTypeSpawn();
            StartCoroutine(CarTypeSpawnLoop());
        }

        private void PowerupTypeSpawn()
        {
            spawnManager.SpawnPowerup(minSpawnPos.z, maxSpawnPos.z); // success (not complete)
            // prevent from spawning on camera by sphere collider on player car
        }

        private IEnumerator PowerupTypeSpawnLoop()
        {
            //float difficultyMultiply = difficultyManager.GetDifficultyMultiply();
            float counting = Random.Range(minTime, maxTime + additionalTime);
            yield return new WaitForSeconds(counting);

            PowerupTypeSpawn();
            StartCoroutine(PowerupTypeSpawnLoop());
        }

        public void SetSpawnInterval(float min, float max)
        {
            minTime = min;
            maxTime = max;
        }
    }
}
