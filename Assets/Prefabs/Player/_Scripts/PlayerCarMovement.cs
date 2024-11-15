using Oathstring.ProjectRoad.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Oathstring
{
    public class PlayerCarMovement : MonoBehaviour
    {
        const int MOVEDIR = 1;

        private float yRot = 0;

        private new Rigidbody rigidbody;
        private PlayerStats playerStats;
        private PlayerEffect playerEffect;

        private float inputValue;

        private Transform sfxTransform;
        private AudioSource playerEngineSFX;

        private Menu menu;

        [Header("Movement")]
        [SerializeField] float speed = 10f;
        [SerializeField] float turnSpeed = 8f;
        [SerializeField] string inputAxis = "Horizontal";

        [Header("Turn Rotation")]
        [SerializeField] float turnRotateSpeed = 1;
        [SerializeField] float turnClamp = 6;
        [SerializeField] float turnSmooth = 0.2f;

        [SerializeField] Vector3 rightEdge;
        [SerializeField] Vector3 leftEdge;

        [SerializeField] MoveDirection moveDirection;

        // Waktu Game Di Start
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            playerStats = GetComponent<PlayerStats>();
            playerEffect = GetComponent<PlayerEffect>();

            sfxTransform = transform.GetChild(3);

            playerEngineSFX = sfxTransform.GetChild(0).GetComponent<AudioSource>();

            menu = FindObjectOfType<Menu>();
        }


        private void Update()
        {
            if (menu.Paused() && playerEngineSFX.isPlaying)
            {
                playerEngineSFX.Pause();
            }

            else if (!menu.Paused() && !playerEngineSFX.isPlaying)
            {
                playerEngineSFX.Play();
            }
        }

        // Memanggil Function Setiap Frame (Physics)
        private void FixedUpdate()
        {
#if UNITY_STANDALONE
            inputValue = Input.GetAxis(inputAxis);
#endif

            if(!playerStats.Crashed())
            {
                //transform.Translate(inputValue * speed * Time.fixedDeltaTime, 0, 0);
                //rigidbody.AddForce(transform.right * inputValue * speed, ForceMode.Impulse);
                rigidbody.velocity = new(inputValue * turnSpeed, 0, MOVEDIR * speed);

                if (inputValue <= 1 && inputValue >= -1)
                {
                    yRot = Mathf.Lerp(yRot, 0, turnSmooth);
                    Vector3 turnRot = new(rigidbody.rotation.x, yRot, rigidbody.rotation.z);
                    transform.eulerAngles = turnRot;
                }

                if (inputValue != 0)
                {
                    yRot += Time.fixedDeltaTime * inputValue * turnRotateSpeed;
                    yRot = Mathf.Clamp(yRot, -turnClamp, turnClamp);
                    Vector3 turnRot = new(rigidbody.rotation.x, yRot, rigidbody.rotation.z);
                    transform.eulerAngles = turnRot;
                }

                if (transform.position.x > rightEdge.x) moveDirection = MoveDirection.Direction_B;
                else if (transform.position.x < leftEdge.x) moveDirection = MoveDirection.Direction_A;
                else moveDirection = MoveDirection.Middle;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            bool collidedSmallCar = collision.collider.CompareTag("Small Car");
            bool collidedMediumCar = collision.collider.CompareTag("Medium Car");
            bool collidedLargeCar = collision.collider.CompareTag("Large Car");
            bool collidedObstacle = collision.collider.CompareTag("Obstacle");

            if (collidedSmallCar || collidedMediumCar || collidedLargeCar || collidedObstacle)
            {
                AudioSource playerHitSFX = sfxTransform.GetChild(1).GetComponent<AudioSource>();
                playerHitSFX.Play();

                if (playerStats.GetHeart() <= 1)
                {
                    AudioSource playerCrashSFX = sfxTransform.GetChild(2).GetComponent<AudioSource>();
                    playerCrashSFX.Play();
                    playerEngineSFX.Stop();

                    playerStats.Damage();
                    playerStats.SetCrashStat(true);
                    //Debug.LogError("Crashed!");
                    rigidbody.velocity = new(0, 0, 0);
                }

                else
                {
                    playerStats.Damage();
                    playerEffect.TakeDamageCDStart(3);
                    //Debug.LogError("Damaged!");
                }
            }
        }

        public MoveDirection GetMoveDirection() => moveDirection;

        public void TouchInput(float value)
        {
            inputValue = value;
        }
    }
}
