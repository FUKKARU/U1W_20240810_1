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

        [SerializeField, Header("�����S�̐����e")] private Transform appleParentTf;
        [SerializeField, Header("�L�m�R�̐����͈̓{�[�_�[")] private Border.Border mushroomBorder;

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
            if (!appleParentTf) throw new Exception($"{nameof(appleParentTf)}���ݒ肳��Ă��܂���");
            if (!mushroomBorder) throw new Exception($"{nameof(mushroomBorder)}���ݒ肳��Ă��܂���");

            kinokoInstance = spawnerSO.GetInstanceByName("Kinoko");
            appleInstance = spawnerSO.GetInstanceByName("Apple");
            StartCoroutine(CreateKinoko());
            StartCoroutine(CreateApple());
        }

        #region kinoko
        //�L�m�R����
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
            // �L�m�R�̐�����������ɒB���Ă��邩�H
            if (KinokoCreatedNum >= spawnerSO.�laxKinokoNum) return;

            // �u�n�ʁv�����ɑ��݂��邩�H
            if (!Physics.Raycast(mushroomBorder.GetRandomPosition(), Vector3.down, out RaycastHit hit)) return;

            // ���́u�n�ʁv�͐^�Ƀ}�b�v�̒n�ʂł��邩�H
            if (!hit.collider.CompareTag(tagSO.TerrainTag)) return;

            // �n�ʂɐ����ɂȂ�悤�ɐ�������
            Quaternion rot = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
            GameObject obj = Instantiate(kinokoInstance.objdata, hit.point, rot, instantiatedMushroomParent);
            obj.transform.up = hit.normal;

            // �L�m�R�̌����C���N�������g
            KinokoCreatedNum++;
        }

#if false
        //�L�m�R�̐����\�͈͐���
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
        //�L�m�R�̐����\�͈͂ɓ����Ă��邩�`�F�b�N
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

        //�����\�͈͂�`��
        void DrawSpawnRange()
        {
            for (int i = 0; i < spawnRange.Count; i++)
            {
                lineRenderer.SetPosition(i, spawnRange[i].transform.position);
            }
            lineRenderer.SetPosition(spawnRange.Count, spawnRange[0].transform.position);
        }
#endif

        //�v���C���[���L�m�R���擾������
        public void KinokoNumDec()
        {
            KinokoCreatedNum--;
        }
        #endregion

        #region apple
        //�����S����
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
            // �����S�̐�����������ɒB���Ă��邩�H
            if (AppleCreatedNum >= appleTrees.Count * maxApplePerTree) return;

            // �����_���ȃ����S�̖؂��擾����(�������A��������ɒB���Ă��Ȃ�����)
            GameObject tree;
            Apple.AppleTreeMov treeCp;
            int cnt = 0;
            while (true)
            {
                tree = appleTrees[Random.Range(0, appleTrees.Count)];
                treeCp = tree.GetComponent<Apple.AppleTreeMov>();

                if (treeCp.appleNum < maxApplePerTree) break;

                if (++cnt > byte.MaxValue) throw new Exception("�����S�̖؂̎擾�Ɏ��Ԃ������肷���Ă��܂�");
            }

            // ���̖؂̃����_���ȏꏊ�Ƀ����S�𐶐�����
            Func<Transform> func = Random.Range(1, maxApplePerTree + 1) switch
            {
                1 => (() => { treeCp.apple1Created = true; return tree.transform.Find(spawnerSO.ApplePos1); }),
                2 => (() => { treeCp.apple2Created = true; return tree.transform.Find(spawnerSO.ApplePos2); }),
                3 => (() => { treeCp.apple3Created = true; return tree.transform.Find(spawnerSO.ApplePos3); }),
                _ => throw new Exception("�����S�̐����C���f�b�N�X���L���ł͂���܂���")
            };
            GameObject obj = Instantiate(appleInstance.objdata, func());
            obj.transform.localEulerAngles = new(0, Random.Range(0.0f, 360.0f), 0);

            // �����S�̌����C���N�������g
            AppleCreatedNum++;
            treeCp.appleNum++;
        }

        //�v���C���[�������S���擾������
        public void AppleNumDec(int num)
        {
            AppleCreatedNum -= num;
        }
        #endregion
    }
}

