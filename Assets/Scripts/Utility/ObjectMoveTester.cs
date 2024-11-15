using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oathstring
{
    public class ObjectMoveTester : MonoBehaviour
    {
        private void FixedUpdate()
        {
            float maju = Input.GetAxis("Vertical");

            transform.Translate(0, 0, maju * 50 * Time.fixedDeltaTime);       
        }
    }
}
