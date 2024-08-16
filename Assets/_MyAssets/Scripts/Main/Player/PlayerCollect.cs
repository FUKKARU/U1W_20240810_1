using SO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Main.Player
{
    public class PlayerCollect : MonoBehaviour
    {
        public int kinokoNum { get; private set; } = 0;
        public int appleNum { get; private set; } = 0;
        public int aburaageNum { get; private set; } = 0;
        float interactDistance = 3;
        [SerializeField] Transform stackStartPoint;
        List<GameObject> collectedObjects = new List<GameObject>();
        public float moveSpeed = 5f;
        GameObject closestHuman;
        GameObject tradeOrNotUI;
        Button initiateTradeButton;
        bool canInteract = false;
        bool onInteract = false;

        SO_Spawner spawnerSO;
        SO_Tag tagSO;
        SO_HierarchyPath hierarchyPathSO;

        void Awake()
        {
            tagSO = SO_Tag.Entity;
            spawnerSO = SO_Spawner.Entity;
            hierarchyPathSO = SO_HierarchyPath.Entity;

            GameObject Canvas = GameObject.FindGameObjectWithTag(tagSO.CanvasTag);
            tradeOrNotUI = Canvas.transform.Find(hierarchyPathSO.TradeOrNot).gameObject;
            initiateTradeButton = tradeOrNotUI.transform.Find(hierarchyPathSO.InitiateTradeButon).GetComponent<Button>();
            
        }

        void Update()
        {

            // 入力を取得
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");

            // 移動方向を計算
            Vector3 moveDirection = new Vector3(moveX, 0, moveY).normalized;

            // キャラクターを移動
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            FindHuman();
        }

        void StackHead()
        {
            float height = 0;
            for (int i = 0; i < collectedObjects.Count; i++)
            {
                GameObject obj = collectedObjects[i];
                obj.transform.position = stackStartPoint.position + new Vector3(0, height, 0);
                if (obj.tag == tagSO.AppleTag) height += 0.25f;
                else if (obj.tag == tagSO.KinokoTag) height += 0.45f;
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.parent = stackStartPoint;
            }
        }

        void FindHuman()
        {
            GameObject[] humans = GameObject.FindGameObjectsWithTag(tagSO.HumanTag);
            float closestDistance = Mathf.Infinity;
            foreach (GameObject human in humans)
            {
                float distanceToHuman = (human.transform.position - transform.position).sqrMagnitude;
                if (distanceToHuman < closestDistance)
                {
                    closestDistance = distanceToHuman;
                    closestHuman = human;
                }
                if (closestDistance <= Mathf.Pow(interactDistance, 2))
                {
                    canInteract = true;
                }
                else
                {
                    canInteract = false;
                }
            }
            if (!tradeOrNotUI.activeSelf && canInteract && !onInteract)
            {
                onInteract = true;
                tradeOrNotUI.SetActive(true);
                initiateTradeButton.onClick.RemoveAllListeners();
                initiateTradeButton.onClick.AddListener(closestHuman.GetComponent<Trade.Human>().OpenTrade);
                initiateTradeButton.onClick.AddListener(HideTradeOrNotUI);
            }
            else if (!canInteract)
            {
                tradeOrNotUI.SetActive(false);
                onInteract = false;
            }

        }

        public void HideTradeOrNotUI()
        {
            tradeOrNotUI.SetActive(false);
        }

        public void OnIneractFalse()
        {
            onInteract = false;
        }

        public void GetAburaage(int usedAppleNum, int usedKinokoNum)//油揚げ取得の際の処理
        {
            
            kinokoNum -= usedKinokoNum;
            appleNum -= usedAppleNum;
            List<GameObject> destroyObjects = new List<GameObject>();
            for (int i = 0; i < collectedObjects.Count; i++)
            {
                GameObject obj = collectedObjects[i];
                if (obj.tag == tagSO.AppleTag && usedAppleNum > 0)
                {
                    usedAppleNum--;
                    destroyObjects.Add(obj);
                }
                else if (obj.tag == tagSO.KinokoTag && usedKinokoNum > 0)
                {
                    usedKinokoNum--;
                    destroyObjects.Add(obj);

                }
            }
            foreach (GameObject obj in destroyObjects) { collectedObjects.Remove(obj); }
            foreach (GameObject obj in destroyObjects) { Destroy(obj); }
            collectedObjects.Add(Instantiate(spawnerSO.GetInstanceByName("Aburaage").objdata));
            StackHead();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == tagSO.KinokoTag)
            {
                kinokoNum++;
                Spawn.SpawnItem.Instance.KinokoNumDec();
                collectedObjects.Add(other.gameObject);
                StackHead();
            }

            else if (other.gameObject.tag == tagSO.TreeTag)
            {
                GameObject appleTree = other.gameObject;
                Apple.AppleTreeMov appleTreeComponent = appleTree.GetComponent<Apple.AppleTreeMov>();
                appleTreeComponent.FreeApple();

            }

        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == tagSO.AppleTag)
            {
                appleNum++;
                collision.gameObject.GetComponent<SphereCollider>().enabled = false;
                collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                collectedObjects.Add(collision.gameObject);
                StackHead();
            }
        }


    }
}