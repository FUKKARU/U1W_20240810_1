using SO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Main.Spawn
{
    public class SpawnItem : MonoBehaviour
    {
        [SerializeField] Transform instantiatedMushroomParent;

        [SerializeField, Header("リンゴの生成親")] private Transform appleParentTf;
        [SerializeField, Header("キノコの生成範囲ボーダー")] private Border.Border mushroomBorder;

        SO_Spawner spawnerSO;
        SO_Tag tagSO;
        public static SpawnItem Instance { get; set; } = null;

        LineRenderer lineRenderer;
        SO_Spawner spawner;

        public int kinokoCreatedNum { get; private set; } = 0;
        SpawnObj kinokoInstance;


        List<GameObject> appleTrees = new List<GameObject>();
        public int appleCreatedNum { get; private set; } = 0;
        const byte maxApplePerTree = 3;
        SpawnObj appleInstance;


        (float minX, float maxX, float minY, float maxY) minMaxRange;
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
            tagSO = SO_Tag.Entity;
            foreach (Transform child in appleParentTf)
            {
                if (child.CompareTag(tagSO.TreeTag))
                {
                    appleTrees.Add(child.gameObject);
                }
            }
        }
        void Start()
        {
            if (!appleParentTf) throw new Exception($"{nameof(appleParentTf)}が設定されていません");
            if (!mushroomBorder) throw new Exception($"{nameof(mushroomBorder)}が設定されていません");

            kinokoInstance = spawnerSO.GetInstanceByName("Kinoko");
            appleInstance = spawnerSO.GetInstanceByName("Apple");
            StartCoroutine(CreateKinoko());
            StartCoroutine(CreateApple());
        }

        #region kinoko
        //キノコ生成
        IEnumerator CreateKinoko()
        {
            if (kinokoCreatedNum < spawnerSO.ＭaxKinokoNum)
            {
                kinokoCreatedNum++;
                Vector3 checkPos = mushroomBorder.GetRandomPosition();
                Quaternion rot = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);  //ランダムな回転を与える
                if (Physics.Raycast(checkPos, Vector3.down, out RaycastHit hit) && hit.collider.CompareTag(tagSO.TerrainTag))
                {
                    Instantiate(kinokoInstance.objdata, hit.point, rot, instantiatedMushroomParent).transform.up = hit.normal;//地形に合わせた配置
                }
            }

            yield return new WaitForSeconds(kinokoInstance.createOffsetTime);
            StartCoroutine(CreateKinoko());
        }

#if false
        //キノコの生成可能範囲生成
        void CreatePoints()
        {
            spawnRangeVector.Clear();
            foreach (GameObject obj in spawnRange)
            {
                Vector2 position = new Vector2(obj.transform.position.x, obj.transform.position.z);
                spawnRangeVector.Add(position);
            }
        }
        (float minX, float maxX, float minY, float mixY) FindRangeMinMax(List<Vector2> checkList)
        {

            float maxX = checkList[0].x;
            float minX = checkList[0].x;
            float maxY = checkList[0].x;
            float minY = checkList[0].x;


            foreach (Vector2 vec in checkList)
            {
                if (vec.x > maxX) maxX = vec.y;
                if (vec.x < minX) minX = vec.y;
                if (vec.y > maxY) maxY = vec.y;
                if (vec.y < minY) minY = vec.y;
            }
            return (minX, maxX, minY, maxY);
        }
        //キノコの生成可能範囲に入っているかチェック
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

        //生成可能範囲を描画
        void DrawSpawnRange()
        {
            for (int i = 0; i < spawnRange.Count; i++)
            {
                lineRenderer.SetPosition(i, spawnRange[i].transform.position);
            }
            lineRenderer.SetPosition(spawnRange.Count, spawnRange[0].transform.position);
        }
#endif

        //プレイヤーがキノコを取得した際
        public void KinokoNumDec()
        {
            kinokoCreatedNum--;
        }
        #endregion

        #region apple
        //リンゴ生成
        IEnumerator CreateApple()
        {
            bool created = false;
            if (appleCreatedNum < appleTrees.Count * maxApplePerTree)
            {
                while (!created)
                {
                    GameObject appleTree = appleTrees[Random.Range(0, appleTrees.Count)];

                    Apple.AppleTreeMov appleTreeComponent = appleTree.GetComponent<Apple.AppleTreeMov>();
                    if (appleTreeComponent.appleNum <= maxApplePerTree)
                    {
                        int createPosIndex = Random.Range(1, maxApplePerTree + 1);
                        string appleName;

                        switch (createPosIndex)
                        {
                            case 1:
                                appleName = spawnerSO.ApplePos1;
                                if (!appleTreeComponent.apple1Created)
                                {
                                    appleTreeComponent.apple1Created = true;
                                    created = true;
                                }
                                break;
                            case 2:
                                appleName = spawnerSO.ApplePos2;
                                if (!appleTreeComponent.apple2Created)
                                {
                                    appleTreeComponent.apple2Created = true;
                                    created = true;
                                }
                                break;
                            case 3:
                                appleName = spawnerSO.ApplePos3;
                                if (!appleTreeComponent.apple3Created)
                                {
                                    appleTreeComponent.apple3Created = true;
                                    created = true;
                                }
                                break;
                            default:
                                throw new Exception("リンゴの作成時にエラーが発生しました");
                        }

                        if (created)
                        {
                            Vector3 eulerRot = new(0, Random.Range(0.0f, 360.0f), 0);
                            Instantiate(appleInstance.objdata, appleTree.transform.Find(appleName)).transform.localEulerAngles = eulerRot;
                            appleCreatedNum++;
                            appleTreeComponent.appleNum++;
                        }
                    }
                }
            }

            yield return new WaitForSeconds(appleInstance.createOffsetTime);
            StartCoroutine(CreateApple());
        }
        //プレイヤーがリンゴを取得した際
        public void AppleNumDec(int num)
        {
            appleCreatedNum -= num;
        }
        #endregion
    }
}

