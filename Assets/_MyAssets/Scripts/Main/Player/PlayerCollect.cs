using General;
using Main.GameHandler;
using SO;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Player
{
    public class PlayerCollect : MonoBehaviour
    {
        [SerializeField] private SoundPlayer soundPlayer;
        [SerializeField] private PlayerMove playerMove;
        [SerializeField] private Judger judger;
        [SerializeField] private Transform hyojuTf;

        public int kinokoNum { get; private set; } = 0;
        public int appleNum { get; private set; } = 0;
        public int aburaageNum { get; private set; } = 0;
        float interactDistance = 3;
        [SerializeField] Transform stackStartPoint;
        List<GameObject> collectedObjects = new List<GameObject>();
        GameObject closestHuman;
        GameObject tradeOrNotUI;
        Button initiateTradeButton;
        bool canInteract = false;
        bool onInteract = false;

        SO_Spawner spawnerSO;
        SO_Tag tagSO;
        SO_HierarchyPath hierarchyPathSO;

        [Space(25)]
        [SerializeField] private Image appleUIImage;
        [SerializeField] private TextMeshProUGUI appleUIText;
        [SerializeField] private Image mushroomUIImage;
        [SerializeField] private TextMeshProUGUI mushroomUIText;
        [SerializeField] private Image aburaageUIImage;
        [SerializeField] private TextMeshProUGUI aburaageUIText;
        ItemUI itemUI;

        private enum Item
        {
            Apple,
            Mushroom,
            Aburaage
        }

        void Awake()
        {
            tagSO = SO_Tag.Entity;
            spawnerSO = SO_Spawner.Entity;
            hierarchyPathSO = SO_HierarchyPath.Entity;

            GameObject Canvas = GameObject.FindGameObjectWithTag(tagSO.CanvasTag);
            tradeOrNotUI = Canvas.transform.Find(hierarchyPathSO.TradeOrNot).gameObject;
            initiateTradeButton = tradeOrNotUI.transform.Find(hierarchyPathSO.InitiateTradeButon).GetComponent<Button>();
        }

        private void OnEnable()
        {
            itemUI = new(
                new(appleUIImage, appleUIText),
                new(mushroomUIImage, mushroomUIText),
                new(aburaageUIImage, aburaageUIText)
                );
        }

        private void OnDisable()
        {
            itemUI.Dispose();
            itemUI = null;
        }

        void Update()
        {
            FindHuman();
            UpdateUI();
            CheckClear();
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
            if (playerMove.IsFoxFigure()) return;

            GameObject[] humans = GameObject.FindGameObjectsWithTag(tagSO.HumanTag);
            float closestDistance = Mathf.Infinity;
            foreach (GameObject human in humans)
            {
                float distanceToHuman = playerMove.CalcSqrMagnitude(human.transform.position);
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
                initiateTradeButton.onClick.AddListener(() => soundPlayer.Play(SoundType.General_ClickSE));
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
            soundPlayer.Play(SoundType.General_ClickSE);

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

        public void OnTriggerEnterBhv(Collider other)
        {
            if (!playerMove.IsFoxFigure()) return;

            if (other.gameObject.tag == tagSO.KinokoTag)
            {
                soundPlayer.Play(SoundType.Main_ItemGetSE);
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

        public void OnCollisionEnterBhv(Collision collision)
        {
            if (!playerMove.IsFoxFigure()) return;

            if (collision.gameObject.tag == tagSO.AppleTag)
            {
                soundPlayer.Play(SoundType.Main_ItemGetSE);
                appleNum++;
                collision.gameObject.GetComponent<SphereCollider>().enabled = false;
                collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                collectedObjects.Add(collision.gameObject);
                StackHead();
            }
        }

        private void UpdateUI()
        {
            (int appleNum, int mushroomNum, int aburaageNum) = CountItems();

            itemUI.Apple.SetText(appleNum.ToString());
            itemUI.Mushroom.SetText(mushroomNum.ToString());
            itemUI.Aburaage.SetText(aburaageNum.ToString());
        }

        private Item Which(GameObject obj)
        {
            if (obj.tag == tagSO.AppleTag) return Item.Apple;
            else if (obj.tag == tagSO.KinokoTag) return Item.Mushroom;
            else return Item.Aburaage;
        }

        private void CheckClear()
        {
            // 兵十との距離が十分近いか？
            float d2 = SO_Judge.Entity.GoalDistance * SO_Judge.Entity.GoalDistance;
            if (playerMove.CalcSqrMagnitude(hyojuTf.position) > d2) return;

            // 油揚げは必要個数分集まっているか？
            (int _, int _, int aburaageNum) = CountItems();
            if (aburaageNum < SO_Judge.Entity.AburaageGoalNum) return;

            // それならば、ゲームクリア！
            judger.GameClear();
        }

        private (int, int, int) CountItems()
        {
            int appleNum = 0;
            int mushroomNum = 0;
            int aburaageNum = 0;

            foreach (var e in collectedObjects)
            {
                Item item = Which(e);

                if (item == Item.Apple) appleNum++;
                else if (item == Item.Mushroom) mushroomNum++;
                else if (item == Item.Aburaage) aburaageNum++;
                else throw new System.Exception("無効な種類です");
            }

            return (appleNum, mushroomNum, aburaageNum);
        }
    }

    internal sealed class ItemUI : System.IDisposable
    {
        private ItemUIUnit _apple;
        internal ItemUIUnit Apple => _apple;

        private ItemUIUnit _mushroom;
        internal ItemUIUnit Mushroom => _mushroom;

        private ItemUIUnit _aburaage;
        internal ItemUIUnit Aburaage => _aburaage;

        internal ItemUI(ItemUIUnit apple, ItemUIUnit mushroom, ItemUIUnit aburaage)
        {
            this._apple = apple;
            this._mushroom = mushroom;
            this._aburaage = aburaage;
        }

        public void Dispose()
        {
            _apple.Dispose();
            _mushroom.Dispose();
            _aburaage.Dispose();
            _apple = null;
            _mushroom = null;
            _aburaage = null;
        }

        internal bool IsNullExist()
        {
            if (_apple == null) return true;
            if (_mushroom == null) return true;
            if (_aburaage == null) return true;
            if (_apple.IsNullExist()) return true;
            if (_mushroom.IsNullExist()) return true;
            if (_aburaage.IsNullExist()) return true;
            return false;
        }
    }

    internal sealed class ItemUIUnit : System.IDisposable
    {
        private Image image;
        private TextMeshProUGUI text;

        internal ItemUIUnit(Image image, TextMeshProUGUI text)
        {
            this.image = image;
            this.text = text;
        }

        public void Dispose()
        {
            image = null;
            text = null;
        }

        internal bool IsNullExist()
        {
            if (!image) return true;
            if (!text) return true;
            return false;
        }

        internal void SetText(string text)
        {
            this.text.text = text;
        }
    }
}