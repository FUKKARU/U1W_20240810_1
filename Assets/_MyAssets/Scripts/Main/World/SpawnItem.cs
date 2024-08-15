using SO;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Main.Spawn
{
    public class SpawnItem : MonoBehaviour
    {
        SO_Spawner spawnerSO;

        [SerializeField, Header("リンゴの木")]
        List<GameObject> appleTrees = new List<GameObject>();
        public int appleCreatedNum { get; private set; } = 0;
        const byte maxApplePerTree = 3;
        [SerializeField, Header("スポーンして良い範囲")] List<GameObject> SpawnRange;
        public int kinokoCreatedNum { get; private set; }
        List<Vector2> spawnRangeVector;
        SpawnObj kinokoInstance;
        SpawnObj appleInstance;
        public static SpawnItem Instance { get; set; } = null;
        void Awake()
        {

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            spawnerSO = SO_Spawner.Entity;
            spawnRangeVector = new List<Vector2>();
            foreach (GameObject obj in SpawnRange)
            {
                Vector2 position = new Vector2(obj.transform.position.x, obj.transform.position.z);
                spawnRangeVector.Add(position);
            }
        }
        void Start()
        {
            kinokoInstance = spawnerSO.GetInstanceByName("Kinoko");
            appleInstance = spawnerSO.GetInstanceByName("Apple");
            //StartCoroutine(CreateKinoko());
            StartCoroutine(CreateApple());
        }
        IEnumerator CreateKinoko()
        {
            Vector2 spawnPos;
            do
            {
                spawnPos = new Vector2(
                Random.Range(spawnerSO.X0, spawnerSO.X1),
                Random.Range(spawnerSO.Y0, spawnerSO.Y1));
            } while (CheckSpawnPoint(spawnPos) == false);
            Quaternion rot = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
            Instantiate(kinokoInstance.spawnObj, new Vector3(spawnPos.x, 0, spawnPos.y), rot);
            yield return new WaitForSeconds(kinokoInstance.createOffsetTime);
            StartCoroutine(CreateKinoko());
        }
        bool CheckSpawnPoint(Vector2 spawnPoint)
        {
            int boundaryPointsCount = spawnRangeVector.Count;
            float windingNum = 0.0f;
            for (int bp = 0; bp < boundaryPointsCount; bp++)
            {
                Vector2 firstPoint = spawnRangeVector[bp];
                Vector2 secondPoint = spawnRangeVector[(bp + 1) % boundaryPointsCount];

                Vector2 firstVector = spawnPoint - firstPoint;
                Vector2 secondVector = spawnPoint - secondPoint;

                if (firstVector.y * secondVector.y < 0.0f)
                {
                    float dist = firstVector.x + (firstVector.y * (secondVector.x - firstVector.x)) / (firstVector.y - secondVector.y);
                    if (dist < 0)
                    {
                        if (firstVector.y < 0.0f) windingNum += 1.0f;
                        else windingNum -= 1.0f;
                    }
                }
                else if (firstVector.y == 0.0f && firstVector.x > 0.0f)
                {
                    if (secondVector.y > 0.0f) windingNum += 0.5f;
                    else windingNum -= 0.5f;
                }
                else if (secondVector.y == 0.0f && secondVector.x > 0.0f)
                {
                    if (firstVector.y < 0.0f) windingNum += 0.5f;
                    else windingNum -= 0.5f;
                }
            }
            return ((int)windingNum % 2) != 0;
        }
        IEnumerator CreateApple()
        {
            int loop = 0;
            bool created = false;
            if (appleCreatedNum >= appleTrees.Count * maxApplePerTree) goto END;
            while (!created && loop < 100)
            {
                loop++;
                if (loop == 99) print("loop");
                GameObject appleTree = appleTrees[Random.Range(0, appleTrees.Count)];

                Apple.AppleTreeMov appleTreeComponent = appleTree.GetComponent<Apple.AppleTreeMov>();
                if (appleTreeComponent.appleNum <= maxApplePerTree)
                {
                    int createPosIndex = Random.Range(1, maxApplePerTree + 1);
                    string appleName;

                    switch (createPosIndex)
                    {
                        case 1:
                            appleName = "applePos1";
                            if (!appleTreeComponent.apple1Created)
                            {
                                appleTreeComponent.apple1Created = true;
                                created = true;
                            }
                                break;
                        case 2:
                            appleName = "applePos2";
                            if (!appleTreeComponent.apple2Created)
                            {
                                appleTreeComponent.apple2Created = true;
                                created = true;
                            }
                            break;
                        case 3:
                            appleName = "applePos3";
                            if (!appleTreeComponent.apple3Created)
                            {
                                appleTreeComponent.apple3Created = true;
                                created = true;
                            }
                            break;
                        default:
                            appleName = "error";
                            break;
                    }
                    if (appleName == "error") throw new Exception("リンゴの作成時にエラーが発生しました");
                    if (created)
                    {
                        Vector3 eulerRot = new Vector3(0, Random.Range(0.0f, 360.0f), 0);
                        Instantiate(appleInstance.spawnObj, appleTree.transform.Find(appleName)).transform.localEulerAngles = eulerRot;
                        appleCreatedNum++;
                        appleTreeComponent.appleNum++;
                    }

                }
            }
        END:
            yield return new WaitForSeconds(appleInstance.createOffsetTime);
            StartCoroutine(CreateApple());
        }
        public void KinokoNumDec()
        {
            kinokoCreatedNum--;
        }
        public void AppleNumDec(int num)
        {
            appleCreatedNum-=num;
        }

    }
}

