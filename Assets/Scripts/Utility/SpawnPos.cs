using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oathstring.ProjectRoad.Utility
{
    public class SpawnPos : MonoBehaviour
    {
        [Header("Direction A Position")]
        [SerializeField] Transform dirAPos;

        [Header("Direction B Position")]
        [SerializeField] Transform dirBPos;

        public Transform GetDirAPos() => dirAPos;
        public Transform GetDirBPos() => dirBPos;
    }
}
