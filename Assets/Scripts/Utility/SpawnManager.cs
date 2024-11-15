using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oathstring.ProjectRoad.Utility
{
    public class SpawnManager : MonoBehaviour
    {

        private readonly int bufferCount = 0;
        private readonly Collider[] hitBuffers = new Collider[15];
        private GameObject[] objectPoolingCars;
        private GameObject[] objectPoolingPowerups;

        [Header("Cars")]
        [SerializeField] GameObject[] cars;
        [SerializeField] float[] checkSpawnArea = new float[3];
        [Header("Powerups")]
        [SerializeField] GameObject[] powerups;
        [SerializeField] float powerupCheckSpawnArea;
        [Header("Utilities")]
        [SerializeField] LayerMask hitLayers;
        [SerializeField] GameObject objectPoolingCar;
        [SerializeField] GameObject objectPoolingPowerup;

        private void Awake()
        {

        }

        private void Update()
        {
            objectPoolingCars = new GameObject[objectPoolingCar.transform.childCount];

            for (int oPIndex = 0; oPIndex < objectPoolingCars.Length; oPIndex++)
            {
                objectPoolingCars[oPIndex] = this.objectPoolingCar.transform.GetChild(oPIndex).gameObject;
                GameObject objectPoolingCar = objectPoolingCars[oPIndex];
                objectPoolingCar.transform.position = new();
            }

            objectPoolingPowerups = new GameObject[objectPoolingPowerup.transform.childCount];

            for (int oPIndex = 0; oPIndex < objectPoolingPowerups.Length; oPIndex++)
            {
                objectPoolingPowerups[oPIndex] = this.objectPoolingPowerup.transform.GetChild(oPIndex).gameObject;
                GameObject objectPoolingPowerup = objectPoolingPowerups[oPIndex];
                objectPoolingPowerup.transform.position = new();
            }
        }

        public void Spawn(float minZPos, float maxZPos, string spawnType) // spawn cars using terrain event
        {
            if(spawnType == "Car")
            {
                SpawnCar(minZPos, maxZPos);
            }

            else if(spawnType == "Powerup")
            {
                SpawnPowerup(minZPos, maxZPos);
            }
        }

        public void SpawnCar(float minZPos, float maxZPos)
        {
            int cSIndex = Random.Range(0, cars.Length); // car index
            int sAIndex = 0; //sa = spawn area
            CarMovement carMovement = cars[cSIndex].GetComponent<CarMovement>();

            if (cars[cSIndex].CompareTag("Small Car"))
            {
                sAIndex = 0;
            }

            else if (cars[cSIndex].CompareTag("Medium Car"))
            {
                sAIndex = 1;
            }

            else if (cars[cSIndex].CompareTag("Large Car"))
            {
                sAIndex = 2;
            }

            Vector3 spawnPoint = new(0, 0, Random.Range(minZPos, maxZPos));
            float spawnXPosRNG = Random.Range(0f, 1f); // left or right
            SpawnPosition spawnPos;
            if (spawnXPosRNG < 0.5f)
            {
                spawnPos = SpawnPosition.Left;
                spawnPoint = new(carMovement.GetSpawnCoorLeft().x, 0, spawnPoint.z);
            }

            else
            {
                spawnPos = SpawnPosition.Right;
                spawnPoint = new(carMovement.GetSpawnCoorRight().x, 0, spawnPoint.z);
            }

            int hits = Physics.OverlapSphereNonAlloc(spawnPoint, checkSpawnArea[sAIndex], hitBuffers, hitLayers);
            //print(hits);
            if (hits > bufferCount)
            {
                //Debug.LogError("Change Position! this pos already has object or you are in camera area");
                SpawnCar(minZPos, maxZPos);
            }

            else
            {
                for (int oPIndex = 0; oPIndex < objectPoolingCars.Length; oPIndex++)
                {
                    if (objectPoolingCars[oPIndex].name == cars[cSIndex].name)
                    {
                        //Debug.Log("Pooling Success! car spawned at:" + spawnPoint + " Spawn At:" + spawnPos);
                        GameObject pooledCar = objectPoolingCars[oPIndex];
                        pooledCar.SetActive(true);
                        pooledCar.transform.parent = null;

                        carMovement = pooledCar.GetComponent<CarMovement>();
                        carMovement.SetSpawnPos(spawnPos);
                        carMovement.SetCoorPos(spawnPoint.z);
                        break;
                    }

                    else if (oPIndex == objectPoolingCars.Length - 1)
                    {
                        //Debug.Log("Success! car spawned at:" + spawnPoint + " Spawn At:" + spawnPos);
                        GameObject spawnedCar = Instantiate(cars[cSIndex], spawnPoint, cars[cSIndex].transform.rotation);
                        spawnedCar.name = cars[cSIndex].name;

                        carMovement = spawnedCar.GetComponent<CarMovement>();
                        carMovement.SetSpawnPos(spawnPos);
                        carMovement.SetCoorPos(spawnPoint.z);
                    }
                }

                if (objectPoolingCars.Length < 1)
                {
                    //Debug.Log("Success! car spawned at:" + spawnPoint + " Spawn At:" + spawnPos);
                    GameObject spawnedCar = Instantiate(cars[cSIndex], spawnPoint, cars[cSIndex].transform.rotation);
                    spawnedCar.name = cars[cSIndex].name;

                    carMovement = spawnedCar.GetComponent<CarMovement>();
                    carMovement.SetSpawnPos(spawnPos);
                    carMovement.SetCoorPos(spawnPoint.z);
                }
            }
        }

        public void SpawnPowerup(float minZPos, float maxZPos)
        {
            int pSIndex = Random.Range(0, powerups.Length); // car index
            PowerupMovement powerupMovement = powerups[pSIndex].GetComponent<PowerupMovement>();

            Vector3 spawnPoint = new(0, 0, Random.Range(minZPos, maxZPos));
            float spawnXPosRNG = Random.Range(0f, 1f); // left or right
            SpawnPosition spawnPos;
            if (spawnXPosRNG < .5f)
            {
                spawnPos = SpawnPosition.Left;
                spawnPoint = new(powerupMovement.GetSpawnCoorLeft().x, 0, spawnPoint.z);
            }

            else
            {
                spawnPos = SpawnPosition.Right;
                spawnPoint = new(powerupMovement.GetSpawnCoorRight().x, 0, spawnPoint.z);
            }

            float spawnXPosDirRNG = Random.Range(0f, 1f);
            MoveDirection moveDirection;
            if (spawnXPosDirRNG < .5f)
            {
                moveDirection = MoveDirection.Direction_A;
            }

            else
            {
                moveDirection = MoveDirection.Direction_B;
            }

            int hits = Physics.OverlapSphereNonAlloc(spawnPoint, powerupCheckSpawnArea, hitBuffers, hitLayers);
            //print(hits);
            if (hits > bufferCount)
            {
                //Debug.LogError("Change Position! this pos already has object or you are in camera area");
                SpawnPowerup(minZPos, maxZPos);
            }

            else
            {
                for (int oPIndex = 0; oPIndex < objectPoolingPowerups.Length; oPIndex++)
                {
                    if (objectPoolingPowerups[oPIndex].name == powerups[pSIndex].name)
                    {
                        //Debug.Log("Pooling Success! powerup spawned at:" + spawnPoint + " Spawn At:" + spawnPos);
                        GameObject pooledPowerup = objectPoolingPowerups[oPIndex];
                        pooledPowerup.SetActive(true);
                        pooledPowerup.transform.parent = null;

                        powerupMovement = pooledPowerup.GetComponent<PowerupMovement>();
                        powerupMovement.SetSpawnPos(spawnPos, moveDirection);
                        powerupMovement.SetCoorPos(spawnPoint.z);
                        break;
                    }

                    else if (oPIndex == objectPoolingPowerups.Length - 1)
                    {
                        //Debug.Log("Success! powerup spawned at:" + spawnPoint + " Spawn At:" + spawnPos);
                        GameObject spawnedPowerup = Instantiate(powerups[pSIndex], spawnPoint, powerups[pSIndex].transform.rotation);
                        spawnedPowerup.name = powerups[pSIndex].name;

                        powerupMovement = spawnedPowerup.GetComponent<PowerupMovement>();
                        powerupMovement.SetSpawnPos(spawnPos, moveDirection);
                        powerupMovement.SetCoorPos(spawnPoint.z);
                    }
                }

                if (objectPoolingPowerups.Length < 1)
                {
                    //Debug.Log("Success! powerup spawned at:" + spawnPoint + " Spawn At:" + spawnPos);
                    GameObject spawnedPowerup = Instantiate(powerups[pSIndex], spawnPoint, powerups[pSIndex].transform.rotation);
                    spawnedPowerup.name = powerups[pSIndex].name;

                    powerupMovement = spawnedPowerup.GetComponent<PowerupMovement>();
                    powerupMovement.SetSpawnPos(spawnPos, moveDirection);
                    powerupMovement.SetCoorPos(spawnPoint.z);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, checkSpawnArea[0]);
            Gizmos.DrawWireSphere(transform.position, checkSpawnArea[1]);
            Gizmos.DrawWireSphere(transform.position, checkSpawnArea[2]);
        }
    }
}
