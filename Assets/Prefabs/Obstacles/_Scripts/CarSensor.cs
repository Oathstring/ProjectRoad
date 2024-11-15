using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace Oathstring
{
    public class CarSensor : MonoBehaviour
    {
        private Vector3 sensorPosition;
        private CarMovement carMovement;

        [SerializeField] float length; // length of boxcast
        [SerializeField] Vector3 size; // add by debug box collider
        [SerializeField] Vector3 offset;
        [SerializeField] LayerMask layerMask;

        private readonly int bufferCount = 1;
        private readonly RaycastHit[] sensorBuffers = new RaycastHit[8];
        private int playerDetectedRightSide = 0;
        private int playerDetectedLeftSide = 0;

        private void Awake()
        {
            carMovement = GetComponent<CarMovement>();
        }

        private void Update()
        {
            sensorPosition = transform.position + offset;

            if (RightSensor() > bufferCount || LeftSensor() > bufferCount) carMovement.SetCanTurn(false);
            else carMovement.SetCanTurn(true);
            //print(RightSensor() + " " + LeftSensor() + " " + sensorBuffers.Length);
        }

        private int RightSensor()
        {
            if (carMovement.GetBehaviorName() == "Idle")
            {
                if (Physics.BoxCastAll(sensorPosition, size / 2, transform.TransformDirection(Vector3.right), Quaternion.identity, length, layerMask).Length > 1)
                {
                    RaycastHit[] hits = Physics.BoxCastAll(sensorPosition, size / 2, transform.TransformDirection(Vector3.right), Quaternion.identity, length, layerMask);

                    foreach(RaycastHit hit in hits)
                    {
                        GameObject playerObj = hit.collider.transform.parent.gameObject;

                        if (playerObj.CompareTag("Player"))
                        {
                            PlayerCarMovement playerCarMovement = playerObj.GetComponent<PlayerCarMovement>();
                            if (playerCarMovement.GetMoveDirection() == carMovement.GetMoveDirection())
                            {
                                playerDetectedRightSide = 1;
                                //print("Player Detected!: " + playerObj.name);
                            }

                            else playerDetectedRightSide = 0;
                        }

                        else playerDetectedRightSide = 0;
                    }
                }

                else playerDetectedRightSide = 0;

                return Physics.BoxCastNonAlloc(sensorPosition, size / 2, transform.TransformDirection(Vector3.right), sensorBuffers, Quaternion.identity, length, layerMask) + playerDetectedRightSide;
            }

            else return 0;
        }

        private int LeftSensor()
        {
            if (carMovement.GetBehaviorName() == "Idle")
            {
                if (Physics.BoxCastAll(sensorPosition, size / 2, transform.TransformDirection(Vector3.left), Quaternion.identity, length, layerMask).Length > 1)
                {
                    RaycastHit[] hits = Physics.BoxCastAll(sensorPosition, size / 2, transform.TransformDirection(Vector3.left), Quaternion.identity, length, layerMask);

                    foreach (RaycastHit hit in hits)
                    {
                        GameObject playerObj = hit.collider.transform.parent.gameObject;

                        if (playerObj.CompareTag("Player"))
                        {
                            PlayerCarMovement playerCarMovement = playerObj.GetComponent<PlayerCarMovement>();
                            if (playerCarMovement.GetMoveDirection() == carMovement.GetMoveDirection())
                            {
                                playerDetectedLeftSide = 1;
                                //print("Player Detected!: " + playerObj.name);
                            }

                            else playerDetectedLeftSide = 0;
                        }

                        else playerDetectedLeftSide = 0;
                    }
                }

                else playerDetectedLeftSide = 0;

                return Physics.BoxCastNonAlloc(sensorPosition, size / 2, transform.TransformDirection(Vector3.left), sensorBuffers, Quaternion.identity, length, layerMask) + playerDetectedLeftSide;
            }

            else return 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Car Edge"))
            {
                GameObject objectPooling = GameObject.FindGameObjectWithTag("Object Pooling");
                gameObject.transform.parent = objectPooling.transform.GetChild(0);
                gameObject.SetActive(false);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
            {
                Gizmos.DrawWireCube(transform.position + offset, size);
            }
        }
    }
}
