using Oathstring.ProjectRoad.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Oathstring
{
    public class PowerupMovement : MonoBehaviour
    {
        const int MOVEDIR = 1;
        private new Rigidbody rigidbody;
        private bool previousDisabled = false;
        private float speed;
        private float spawnZCoor;
        private float spawnYCoor = 1.1f;

        // Difficulty Manager
        private DifficultyManager difficultyManager;

        private PlayerEffect playerEffect;
        private PlayerStats playerStats;

        // Terrain
        private TerrainMovement terrainMovement;

        // sound
        private AudioSource powerupSound;

        [Header("Settings")]
        [Space]
        [SerializeField] MoveDirection moveDirection;
        [SerializeField] SpawnPosition spawnPosition;
        [SerializeField] Vector4 spawnCoor;
        [Space]
        [SerializeField, Range(0, 0.1f)] float calculateDelay = 0.001f;
        [SerializeField] PowerupType powerupType;
        // add some abilities...

        private void Awake()
        {
            difficultyManager = FindObjectOfType<DifficultyManager>();
            playerEffect = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEffect>();
            playerStats = playerEffect.GetComponent<PlayerStats>();
            terrainMovement = FindObjectOfType<TerrainMovement>();

            //normalSpeed *= dirBMultiply;

            rigidbody = GetComponent<Rigidbody>();
            powerupSound = GetComponent<AudioSource>();

            StartCoroutine(CalculateDelay());
        }
        private IEnumerator CalculateDelay() // delay need to avoid wrong calculating
        {
            yield return new WaitForSeconds(calculateDelay);

            CalculatePositions();

            yield return new WaitForSeconds(calculateDelay);
        }

        private void CalculatePositions()
        {
            if(moveDirection == MoveDirection.Direction_A)
            {
                if (spawnPosition == SpawnPosition.Left)
                {
                    Vector3 calculatedPos = new(spawnCoor.x, spawnYCoor, spawnZCoor);
                    rigidbody.position = calculatedPos;
                }
                else
                {
                    Vector3 calculatedPos = new(spawnCoor.y, spawnYCoor, spawnZCoor);
                    rigidbody.position = calculatedPos;
                }
            }

            else if(moveDirection == MoveDirection.Direction_B)
            {
                if (spawnPosition == SpawnPosition.Left)
                {
                    Vector3 calculatedPos = new(spawnCoor.z, spawnYCoor, spawnZCoor);
                    rigidbody.position = calculatedPos;
                }
                else
                {
                    Vector3 calculatedPos = new(spawnCoor.w, spawnYCoor, spawnZCoor);
                    rigidbody.position = calculatedPos;
                }
            }
        }

        private void OnEnable()
        {
            if (previousDisabled)
            {
                float randomPosValue = Random.Range((float)SpawnPosition.Left, (float)SpawnPosition.Right);

                if (randomPosValue < .5f)
                {
                    spawnPosition = SpawnPosition.Left;
                }

                else
                {
                    spawnPosition = SpawnPosition.Right;
                }

                StartCoroutine(CalculateDelay());
            }
        }

        private void OnDisable()
        {
            previousDisabled = true;
            StopAllCoroutines();
        }

        private void LateUpdate()
        {
            if (!playerStats.Crashed())
            {
                speed = -terrainMovement.GetDefaultSpeed() * difficultyManager.GetDifficultyMultiply();
                rigidbody.velocity = new(rigidbody.velocity.x, 0, MOVEDIR * speed * playerEffect.GetSpeedupEffectMultiply());
            }

            else
            {
                rigidbody.velocity = new();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Car Edge"))
            {
                GameObject objectPooling = GameObject.FindGameObjectWithTag("Object Pooling");
                gameObject.transform.parent = objectPooling.transform.GetChild(1);
                gameObject.SetActive(false);
            }

            Transform player = other.transform.parent;
            if (player && player.CompareTag("Player"))
            {
                //print(other.name);
                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                AudioSource playerAudio = player.transform.GetChild(3).transform.GetChild(1).GetComponent<AudioSource>();
                
                if(playerStats.GetHeart() < playerStats.GetMaxHeart())
                {
                    if (powerupType as HeartIncreasement)
                    {
                        HeartIncreasement heartIncreasement = powerupType as HeartIncreasement;
                        playerStats.AddHeart(heartIncreasement.GetHeart());

                        // move to pooling object
                        GameObject objectPooling = GameObject.FindGameObjectWithTag("Object Pooling");
                        gameObject.transform.parent = objectPooling.transform.GetChild(1);
                        playerAudio.PlayOneShot(powerupSound.clip);
                        gameObject.SetActive(false);
                    }
                }

                if(playerStats.GetSpeedBoost() < playerStats.GetMaxSpeedBoost())
                {
                    if (powerupType as Speedup)
                    {
                        Speedup speedup = powerupType as Speedup;
                        playerStats.AddSpeedBoost(speedup.GetSpeedup());

                        // move to pooling object
                        GameObject objectPooling = GameObject.FindGameObjectWithTag("Object Pooling");
                        gameObject.transform.parent = objectPooling.transform.GetChild(1);
                        playerAudio.PlayOneShot(powerupSound.clip);
                        gameObject.SetActive(false);
                    }
                }
            }
        }

        public Vector3 GetSpawnCoorLeft() => spawnCoor;
        public Vector3 GetSpawnCoorRight() => spawnCoor;
        public MoveDirection GetMoveDirection() => moveDirection;

        public void SetSpawnPos(SpawnPosition position, MoveDirection direction)
        {
            spawnPosition = position;
            moveDirection = direction;
        }

        public void SetCoorPos(float zPos)
        {
            spawnZCoor = zPos;
        }

        public void SetMovementMultiplier(float multply)
        {
            speed *= multply;
        }
    }
}
