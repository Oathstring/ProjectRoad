using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oathstring
{
    [CreateAssetMenu(fileName = "HeartIncreasement", menuName = "Powerup Abilities/Heart Increasement")]
    public class HeartIncreasement : PowerupType
    {
        [SerializeField] int heartCount = 1;

        public int GetHeart() => heartCount;
    }
}
