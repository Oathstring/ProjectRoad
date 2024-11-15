using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oathstring
{
    [CreateAssetMenu(fileName = "Speedup", menuName = "Powerup Abilities/Speedup")]
    public class Speedup : PowerupType
    {
        [SerializeField] int speedup = 1;

        public int GetSpeedup() => speedup;
    }
}
