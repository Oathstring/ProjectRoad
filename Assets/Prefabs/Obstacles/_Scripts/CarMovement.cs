using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Oathstring.ProjectRoad.Utility;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using static UnityEngine.GraphicsBuffer;

namespace Oathstring
{
    public class CarMovement : MonoBehaviour
    {
        const int MOVEDIR = 1;
        private new Rigidbody rigidbody;
        private delegate void CarBehavior();
        private CarBehavior carBehavior;
        private float idleTime;
        private float timer;
        private readonly int minimumIdleTime = 3;
        private bool canTurn = true;
        private bool previousDisabled = false;
        private bool calculating = false;
        private float speed;
        private float yRot;
        private AudioSource sfxSource;
        private string CurrentBehavior
        {
            get
            {
                return carBehavior.Method.Name;
            }
        }

        // Need To Calculate
        private int turnDir;
        private Vector3 leftEdge;
        private Vector3 rightEdge;
        private Vector3 leftCheck;
        private Vector3 rightCheck;

        private float move;
        private readonly float sensitivity = 3;

        // Difficulty Manager
        private DifficultyManager difficultyManager;

        private PlayerEffect playerEffect;
        private PlayerStats playerStats;

        [Header("Settings")]
        [SerializeField] int maximumIdleTime = 15;
        [Space]
        [SerializeField] int rSTurnDir;
        [SerializeField] int lSTurnDir;
        [Space]
        [SerializeField] float turnSpeed;
        [SerializeField] float normalSpeed = -15;
        [SerializeField] float dirBMultiply = 1;
        [Space]
        [SerializeField] float turnRotateSpeed = 1;
        [SerializeField] float turnClamp = 6;
        [SerializeField] float turnSmooth = 0.2f;
        [Space]
        [SerializeField] Vector3 rSpawnLeftEdge;
        [SerializeField] Vector3 rSpawnRightEdge;
        [Space]
        [SerializeField] Vector3 lSpawnLeftEdge;
        [SerializeField] Vector3 lSpawnRightEdge;
        [Space]
        [SerializeField] MoveDirection moveDirection;
        [SerializeField] SpawnPosition spawnPosition;
        [SerializeField] Vector3 spawnCoorLeft;
        [SerializeField] Vector3 spawnCoorRight;
        [Space]
        [SerializeField, Range(0, 0.1f)] float calculateDelay = 0.001f;

        private void Awake()
        {
            difficultyManager = FindObjectOfType<DifficultyManager>();
            playerEffect = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEffect>();
            playerStats = playerEffect.GetComponent<PlayerStats>();

            normalSpeed *= dirBMultiply;

            rigidbody = GetComponent<Rigidbody>();
            carBehavior = Idle;
            /*float randomPosValue = Random.Range((float)SpawnPosition.Left, (float)SpawnPosition.Right);

            if (randomPosValue < .5f)
            {
                spawnPosition = SpawnPosition.Left;
            }

            else
            {
                spawnPosition = SpawnPosition.Right;
            }

            print("value at:" + randomPosValue);*/

            StartCoroutine(CalculateDelay());
            /*CalculateTurnDir();
            CalculateEdges();
            CalculateEdgesCheck();*/

            idleTime = 0;
            timer = 0;
            idleTime = Random.Range(minimumIdleTime, maximumIdleTime);
            
            canTurn = true;
            sfxSource = GetComponentInChildren<AudioSource>();
        }
        private IEnumerator CalculateDelay() // delay need to avoid wrong calculating
        {
            yield return new WaitForSeconds(calculateDelay);

            CalculatePositions();

            yield return new WaitForSeconds(calculateDelay);

            CalculateTurnDir();
            CalculateEdges();
            CalculateEdgesCheck();

            calculating = false;
        }

        private void CalculatePositions()
        {
            calculating = true;

            if (spawnPosition == SpawnPosition.Left)
            {
                Vector3 calculatedPos = new(spawnCoorLeft.x, transform.position.y, spawnCoorLeft.z);
                rigidbody.position = calculatedPos;
            }
            else
            {
                Vector3 calculatedPos = new(spawnCoorRight.x, transform.position.y, spawnCoorRight.z);
                rigidbody.position = calculatedPos;
            }
        }

        private void CalculateTurnDir()
        {
            if (spawnPosition == SpawnPosition.Left)
                turnDir = lSTurnDir;

            else turnDir = rSTurnDir;
        }

        private void CalculateEdges()
        {
            if (spawnPosition == SpawnPosition.Left)
            {
                leftEdge = transform.position;
                rightEdge = lSpawnRightEdge + transform.position;
            }

            else
            {
                leftEdge = rSpawnLeftEdge + transform.position;
                rightEdge = transform.position;
            }
        }

        private void CalculateEdgesCheck() // new bug fixed
        {
            if (moveDirection == MoveDirection.Direction_A)
            {
                // Positive Result
                leftCheck = Check.ResultVector3(leftEdge);

                //Negative Result
                rightCheck = Check.ResultVector3(rightEdge);
            }

            if (moveDirection == MoveDirection.Direction_B)
            {
                //Negative Result
                leftCheck = Check.ResultVector3(leftEdge);

                //Positive Result
                rightCheck = Check.ResultVector3(rightEdge);
            }
        }

        private void OnEnable()
        {
            if (previousDisabled)
            {
                move = 0;
                carBehavior = Idle;
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
                /*CalculateTurnDir();
                CalculateEdges();
                CalculateEdgesCheck();*/

                timer = 0;
                idleTime = Random.Range(minimumIdleTime, maximumIdleTime);

                canTurn = true;
            }
        }

        private void OnDisable()
        {
            previousDisabled = true;
            StopAllCoroutines();
        }

        private void Update()
        {
            speed = normalSpeed * difficultyManager.GetDifficultyMultiply();

            if (playerStats.Crashed() && sfxSource.isPlaying) sfxSource.Pause();
        }

        private void FixedUpdate()
        {
            if (!calculating)
            {
                if (!playerStats.Crashed())
                {
                    carBehavior();
                    rigidbody.velocity = new(rigidbody.velocity.x, 0, MOVEDIR * speed * playerEffect.GetSpeedupEffectMultiply());
                }

                else
                {
                    rigidbody.velocity = new();
                }

                // Turn Rotation
                if (CurrentBehavior == "Idle")
                {
                    yRot = Mathf.Lerp(yRot, 0, turnSmooth);
                    Vector3 turnRot = new(rigidbody.rotation.x, yRot, rigidbody.rotation.z);
                    transform.eulerAngles = turnRot;
                }

                else
                {
                    if (moveDirection == MoveDirection.Direction_A)
                    {
                        if (move != 0)
                        {
                            yRot += Time.fixedDeltaTime * move * turnRotateSpeed;
                            yRot = Mathf.Clamp(yRot, -turnClamp, turnClamp);
                            transform.eulerAngles = transform.up * yRot;
                        }
                    }

                    if(moveDirection == MoveDirection.Direction_B)
                    {
                        if (move != 0)
                        {
                            yRot -= Time.fixedDeltaTime * move * turnRotateSpeed;
                            yRot = Mathf.Clamp(yRot, -turnClamp, turnClamp);
                            transform.eulerAngles = transform.up * yRot;
                        }
                    }
                }
            }
        }

        private void Idle()
        {
            if(!calculating && canTurn)
            {
                if (timer < idleTime)
                {
                    timer += Time.deltaTime;
                }

                else
                {
                    timer = 0;
                    move = Mathf.MoveTowards(move, 0, sensitivity * Time.deltaTime);

                    if (moveDirection == MoveDirection.Direction_A)
                    {
                        //fixed
                        if (turnDir == 1)
                        {
                            //print("Waktu nya belok kanan");
                            carBehavior = RightTurn;
                        }

                        if (turnDir == -1)
                        {
                            //print("Waktu nya belok kiri");
                            carBehavior = LeftTurn;
                        }
                    }

                    if(moveDirection == MoveDirection.Direction_B)
                    {
                        //fixed
                        if (turnDir == -1)
                        {
                            //print("Waktu nya belok kanan");
                            carBehavior = RightTurn;
                        }

                        if (turnDir == 1)
                        {
                            //print("Waktu nya belok kiri");
                            carBehavior = LeftTurn;
                        }
                    }
                }
            }
        }

        private void RightTurn()
        {
            if (moveDirection == MoveDirection.Direction_A)
            {
                if (transform.position.x >= rightEdge.x)
                {
                    turnDir = -1;
                    rigidbody.velocity = new();
                    idleTime = Random.Range(minimumIdleTime, maximumIdleTime);
                    carBehavior = Idle;
                }

                else
                {
                    move = Mathf.MoveTowards(move, turnDir, sensitivity * Time.deltaTime);
                    rigidbody.velocity = move * turnSpeed * transform.right;
                }
            }

            if (moveDirection == MoveDirection.Direction_B)
            {
                if (transform.position.x <= rightEdge.x)
                {
                    turnDir = 1;
                    rigidbody.velocity = new();
                    idleTime = Random.Range(minimumIdleTime, maximumIdleTime);
                    carBehavior = Idle;
                }

                else
                {
                    move = Mathf.MoveTowards(move, turnDir, sensitivity * Time.deltaTime);
                    rigidbody.velocity = move * turnSpeed * transform.right;
                }
            }
        }

        private void LeftTurn()
        {
            if(moveDirection == MoveDirection.Direction_A)
            {
                if (transform.position.x <= leftEdge.x)
                {
                    turnDir = 1;
                    rigidbody.velocity = new();
                    idleTime = Random.Range(minimumIdleTime, maximumIdleTime);
                    carBehavior = Idle;
                }

                else
                {
                    move = Mathf.MoveTowards(move, turnDir, sensitivity * Time.deltaTime);
                    rigidbody.velocity = move * turnSpeed * transform.right;
                }
            }

            if(moveDirection == MoveDirection.Direction_B)
            {
                if (transform.position.x >= leftEdge.x)
                {
                    turnDir = -1;
                    rigidbody.velocity = new();
                    idleTime = Random.Range(minimumIdleTime, maximumIdleTime);
                    carBehavior = Idle;
                }

                else
                {
                    move = Mathf.MoveTowards(move, turnDir, sensitivity * Time.deltaTime);
                    rigidbody.velocity = move * turnSpeed * transform.right;
                }
            }
        }

        public int GetTurnDir() => turnDir;
        public string GetBehaviorName() => carBehavior.GetMethodInfo().Name;
        public Vector3 GetSpawnCoorLeft() => spawnCoorLeft;
        public Vector3 GetSpawnCoorRight() => spawnCoorRight;
        public MoveDirection GetMoveDirection() => moveDirection;

        public void SetCanTurn(bool canTurn)
        {
            this.canTurn = canTurn;
        }

        public void SetSpawnPos(SpawnPosition position)
        {
            spawnPosition = position;
        }

        public void SetCoorPos(float zPos)
        {
            spawnCoorLeft.z = zPos;
            spawnCoorRight.z = zPos;
        }

        public void SetMovementMultiplier(float multply)
        {
            speed *= multply;
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
            {
                Gizmos.DrawLine(transform.position, transform.position + leftEdge);
                Gizmos.DrawLine(transform.position, transform.position + rightEdge);
            }
        }
    }
}

public enum MoveDirection
{
    Direction_A, Direction_B, Middle
}

public enum SpawnPosition
{
    Left, Right
}
