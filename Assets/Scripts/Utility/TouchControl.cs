using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Oathstring
{
#if !UNITY_STANDALONE
    public class TouchControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private PlayerCarMovement player;
        private bool touched;
        private readonly float sensitivity = 3;

        [SerializeField] TouchArea touchArea = TouchArea.None;

        private float inputValue;

        private void Awake()
        {
            player = FindObjectOfType<PlayerCarMovement>();
        }

        private void FixedUpdate()
        {
            if (touched)
            {
                float target = 0;

                if (touchArea == TouchArea.Left)
                {
                    target = -1;
                }

                if (touchArea == TouchArea.Right)
                {
                    target = 1;
                }

                inputValue = Mathf.MoveTowards(inputValue, target, sensitivity * Time.deltaTime);
            }

            else
            {
                inputValue = Mathf.MoveTowards(inputValue, 0, sensitivity * Time.deltaTime);
            }
            
            player.TouchInput(inputValue);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            touched = true;

            if (eventData.position.x > Screen.width / 2)
                touchArea = TouchArea.Right;
            else
                touchArea = TouchArea.Left;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            touched = false;
            touchArea = TouchArea.None;
        }
    }
#endif

    public enum TouchArea
    {
        Left, Right, None
    }
}
