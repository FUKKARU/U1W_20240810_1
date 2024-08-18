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

        public int KinokoCreatedNum { get; private set; } = 0;
        SpawnObj kinokoInstance;

        private readonly List<GameObject> appleTrees = new List<GameObject>();
        public int AppleCreatedNum { get; private set; } = 0;
        private static readonly byte maxApplePerTree = 3;
        SpawnObj appleInstance;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            spawnerSO = SO_Spawner.Entity;
            tagSO = SO_Tag.Entity;
            foreach (Transform child in appleParentTf)
            {
                if (child.CompareTag(tagSO.TreeTag))
                    appleTrees.Add(child.gameObject);
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
            while (true)
            {
                CreateKinokoBhv();
                yield return new WaitForSeconds(kinokoInstance.createOffsetTime);
            }
        }

        private void CreateKinokoBhv()
        {
            // キノコの生成数が上限に達しているか？
            if (KinokoCreatedNum >= spawnerSO.ＭaxKinokoNum) return;

            // 「地面」が下に存在するか？
            if (!Physics.Raycast(mushroomBorder.GetRandomPosition(), Vector3.down, out RaycastHit hit)) return;

            // その「地面」は真にマップの地面であるか？
            if (!hit.collider.CompareTag(tagSO.TerrainTag)) return;

            // 地面に垂直になるように生成する
            Quaternion rot = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
            GameObject obj = Instantiate(kinokoInstance.objdata, hit.point, rot, instantiatedMushroomParent);
            obj.transform.up = hit.normal;

            // キノコの個数をインクリメント
            KinokoCreatedNum++;
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
            KinokoCreatedNum--;
        }
        #endregion

        #region apple
        //リンゴ生成
        IEnumerator CreateApple()
        {
            while (true)
            {
                CreateAppleBhv();
                yield return new WaitForSeconds(appleInstance.createOffsetTime);
            }
        }

        private void CreateAppleBhv()
        {
            // リンゴの生成数が上限に達しているか？
            if (AppleCreatedNum >= appleTrees.Count * maxApplePerTree) return;

            // ランダムなリンゴの木を取得する(ただし、生成上限に達していないもの)
            GameObject tree;
            Apple.AppleTreeMov treeCp;
            int cnt = 0;
            while (true)
            {
                tree = appleTrees[Random.Range(0, appleTrees.Count)];
                treeCp = tree.GetComponent<Apple.AppleTreeMov>();

                if (treeCp.appleNum < maxApplePerTree) break;

                if (++cnt > byte.MaxValue) throw new Exception("リンゴの木の取得に時間がかかりすぎています");
            }

            // その木のランダムな場所にリンゴを生成する
            Func<Transform> func = Random.Range(1, maxApplePerTree + 1) switch
            {
                1 => (() => { treeCp.apple1Created = true; return tree.transform.Find(spawnerSO.ApplePos1); }),
                2 => (() => { treeCp.apple2Created = true; return tree.transform.Find(spawnerSO.ApplePos2); }),
                3 => (() => { treeCp.apple3Created = true; return tree.transform.Find(spawnerSO.ApplePos3); }),
                _ => throw new Exception("リンゴの生成インデックスが有効ではありません")
            };
            GameObject obj = Instantiate(appleInstance.objdata, func());
            obj.transform.localEulerAngles = new(0, Random.Range(0.0f, 360.0f), 0);

            // リンゴの個数をインクリメント
            AppleCreatedNum++;
            treeCp.appleNum++;
        }

        //プレイヤーがリンゴを取得した際
        public void AppleNumDec(int num)
        {
            AppleCreatedNum -= num;
        }
        #endregion
    }
}

