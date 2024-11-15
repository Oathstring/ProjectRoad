using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Oathstring
{
    public class PlayerEffect : MonoBehaviour
    {
        private readonly Collider[] overLapBuffer = new Collider[10];

        private delegate void EventEffect();
        private EventEffect eventEffect;

        private float transparantCD;

        private MeshRenderer bodyMesh;
        private MeshRenderer spoilerMesh;
        private MeshRenderer[] wheelMeshes;

        private Collider[] colliders;
        private BoxCollider boxCollider;

        private readonly float transparantMinimum = 0.1f;
        private float transparantTransition = 1;

        private Material baseMat;
        private Color baseColor;
        //private LayerMask baseLayer;

        // hidden effect
        private float speedupEffectMultiply = 1;
        private float speedupTimeMultiply = 2.5f;

        private bool speedup;
        private bool transparant;

        [Header("Mesh Renderer Objects")]
        [SerializeField] GameObject carBody;
        [SerializeField] GameObject spoiler;
        [SerializeField] GameObject wheel;

        [Header("Transparant Effect")]
        [SerializeField] Material transparantMat;
        [SerializeField] float fadeOutSpeed = 5f;
        [SerializeField] LayerMask transparantLayer;

        [Header("Speedup Effect")]
        [SerializeField] float maxSpeedupMultiply = 2.5f;

        [Header("Visual Effect")]
        [SerializeField] ParticleSystem engineSmoke; 

        [Header("Event Effect")]
        [SerializeField] LayerMask obstacleLayer;

        private void Awake()
        {
            speedupTimeMultiply = maxSpeedupMultiply;

            bodyMesh = carBody.GetComponent<MeshRenderer>();
            spoilerMesh = spoiler.GetComponent<MeshRenderer>();
            wheelMeshes = wheel.GetComponentsInChildren<MeshRenderer>();

            colliders = GetComponentsInChildren<Collider>();
            boxCollider = GetComponentInChildren<BoxCollider>();

            baseMat = bodyMesh.material;
            baseColor = baseMat.color;
            //baseLayer = gameObject.layer;

            transparantMat.SetColor("_BaseColor", baseColor);
        }

        private void Update()
        {
            if (transparant)
            {
                if(transparantMat.color.a > transparantMinimum)
                {
                    transparantTransition -= Time.deltaTime * fadeOutSpeed;
                    transparantMat.SetColor("_BaseColor", new(1, 1, 1, transparantTransition));
                }

                bodyMesh.material = transparantMat;
                spoilerMesh.material = transparantMat;
                foreach(MeshRenderer wheelMesh in wheelMeshes)
                {
                    wheelMesh.material = transparantMat;
                }

                foreach(Collider collider in colliders)
                {
                    collider.gameObject.layer = LayerMask.NameToLayer("Invisible");
                }

                eventEffect();
            }

            else
            {
                transparantTransition = 1;
                transparantMat.SetColor("_BaseColor", baseColor);
                bodyMesh.material = baseMat;
                spoilerMesh.material = baseMat;
                foreach (MeshRenderer wheelMesh in wheelMeshes)
                {
                    wheelMesh.material = baseMat;
                }

                foreach (Collider collider in colliders)
                {
                    collider.gameObject.layer = LayerMask.NameToLayer("Player");
                }
            }
        }

        private void TakeDamageEvent()
        {
            if (transparant)
            {
                int overlap = Physics.OverlapBoxNonAlloc(transform.position + boxCollider.center, boxCollider.size / 2, overLapBuffer, Quaternion.identity, obstacleLayer);

                if (transparantCD <= 0 && overlap < 1)
                {
                    transparantCD = 0;
                    transparant = false;

                    eventEffect -= TakeDamageEvent;
                }

                else if (transparantCD >= 0)
                {
                    transparant = true;
                    transparantCD -= Time.deltaTime;
                }
            }
        }

        public void TakeDamageCDStart(float cd) // memulai cooldown take damage player akan transparant sementara
        {
            transparant = true;
            transparantCD = cd;

            eventEffect = TakeDamageEvent;
        }

        public void StartEngineSmoke()
        {
            if(!engineSmoke.isPlaying) engineSmoke.Play();
        }

        private void SpeedupEvent()
        {
            if (transparant)
            {
                if (speedup)
                {
                    if(speedupEffectMultiply >= maxSpeedupMultiply)
                    {
                        speedupEffectMultiply = maxSpeedupMultiply;
                    }

                    else
                    {
                        speedupEffectMultiply += Time.deltaTime * speedupTimeMultiply;
                    }
                }

                else
                {
                    if (speedupEffectMultiply <= 1)
                    {
                        speedupEffectMultiply = 1;
                    }

                    else
                    {
                        speedupEffectMultiply -= Time.deltaTime * speedupTimeMultiply;
                    }
                }

                int overlap = Physics.OverlapBoxNonAlloc(transform.position + boxCollider.center, boxCollider.size / 2, overLapBuffer, Quaternion.identity, obstacleLayer);

                if (transparantCD <= 0 && overlap < 1)
                {
                    transparantCD = 0;
                    transparant = false;

                    eventEffect -= TakeDamageEvent;
                }

                else if (transparantCD >= 0)
                {
                    transparant = true;
                    transparantCD -= Time.deltaTime;
                }
            }
        }

        private IEnumerator SpeedupCoroutineEvent(float cd)
        {
            yield return new WaitForSeconds(cd - 1);

            speedup = false;
        }

        public void SpeedupStart(float cd)
        {
            transparant = true;
            speedup = true;
            transparantCD = cd;

            eventEffect = SpeedupEvent;
            StartCoroutine(SpeedupCoroutineEvent(cd));
        }

        public bool HasSpeedup() => speedup;
        public bool HasTransparant() => transparant;

        public float GetSpeedupEffectMultiply() => speedupEffectMultiply;

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawCube(transform.position + boxCollider.center, boxCollider.size);
        }
    }
}
