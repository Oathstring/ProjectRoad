using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oathstring
{
    public class PlayerCamera : MonoBehaviour
    {
        private Vector3 velocity = Vector3.zero;

        [SerializeField] Transform player;
        [SerializeField] Vector3 offset;
        [SerializeField] float smoothSpeed = 0.1f;

        // Start is called before the first frame update
        void Start()
        {
            if (player == null) 
            {
                player = GameObject.FindGameObjectWithTag("Player").transform;
            }
        }

        // Update is called once per frame
        void Update()
        {
            /*Vector3 desiredPosition = player.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;*/

            // Define a target position above and behind the target transform
            Vector3 targetPosition = player.TransformPoint(offset);

            // Smoothly move the camera towards that target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed);
        }
    }
}
