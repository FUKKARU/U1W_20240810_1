using General;
using SO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Title.Button
{
    public sealed class ButtonManager : MonoBehaviour
    {
        #region

        [SerializeField, Header("ボタンのスプライト\n1.終わる\n2.始める\n3.クレジット")] private List<ButtonSprites> _buttonSpritesList;
        private ButtonSprites exitButtonSprites => _buttonSpritesList[0];
        private ButtonSprites startButtonSprites => _buttonSpritesList[1];
        private ButtonSprites creditButtonSprites => _buttonSpritesList[2];

        [SerializeField, Header("ボタンのコンポーネント\n1.終わる\n2.始める\n3.クレジット")] private List<ButtonInfo> _buttonInfoList;
        private ButtonInfo exitButtonInfo => _buttonInfoList[0];
        private ButtonInfo startButtonInfo => _buttonInfoList[1];
        private ButtonInfo creditButtonInfo => _buttonInfoList[2];

        [SerializeField, Header("AudioSource")] private AudioSource _audioSource;
        private AudioSource audioSource => _audioSource;

        private bool isExitButtonHandlable = false;
        private bool isStartButtonHandlable = false;
        private bool isCreditButtonHandlable = false;

        private enum ContactType
        {
            /// <summary>
            /// ポインターが対象に乗った
            /// </summary>
            PointerEnter,

            /// <summary>
            /// ポインターが対象から離れた
            /// </summary>
            PointerExit,

            /// <summary>
            /// ポインターが対象を押下した
            /// </summary>
            PointerUp,

            /// <summary>
            /// ポインターが押下状態から離した
            /// </summary>
            PointerDown,

            /// <summary>
            /// 対象の上でポインターを押下し、同一の対象の上で離した
            /// </summary>
            PointerClick,
        }

        private void OnEnable()
        {
            if (_buttonSpritesList.Count != 3) throw new System.Exception("リストの長さが想定外な値です");
            if (_buttonInfoList.Count != 3) throw new System.Exception("リストの長さが想定外な値です");

            exitButtonInfo.Image.sprite = exitButtonSprites.Normal;
            startButtonInfo.Image.sprite = startButtonSprites.Normal;
            creditButtonInfo.Image.sprite = creditButtonSprites.Normal;

            exitButtonInfo.EventTrigger.
                AddListener(EventTriggerType.PointerEnter, () => OnExitButtonHandled(ContactType.PointerEnter));
            startButtonInfo.EventTrigger.
                AddListener(EventTriggerType.PointerEnter, () => OnStartButtonHandled(ContactType.PointerEnter));
            creditButtonInfo.EventTrigger.
                AddListener(EventTriggerType.PointerEnter, () => OnCreditButtonHandled(ContactType.PointerEnter));

            exitButtonInfo.EventTrigger.
                AddListener(EventTriggerType.PointerExit, () => OnExitButtonHandled(ContactType.PointerExit));
            startButtonInfo.EventTrigger.
                AddListener(EventTriggerType.PointerExit, () => OnStartButtonHandled(ContactType.PointerExit));
            creditButtonInfo.EventTrigger.
                AddListener(EventTriggerType.PointerExit, () => OnCreditButtonHandled(ContactType.PointerExit));

            exitButtonInfo.EventTrigger.
                AddListener(EventTriggerType.PointerUp, () => OnExitButtonHandled(ContactType.PointerUp));
            startButtonInfo.EventTrigger.
                AddListener(EventTriggerType.PointerUp, () => OnStartButtonHandled(ContactType.PointerUp));
            creditButtonInfo.EventTrigger.
                AddListener(EventTriggerType.PointerUp, () => OnCreditButtonHandled(ContactType.PointerUp));

            exitButtonInfo.EventTrigger.
                AddListener(EventTriggerType.PointerDown, () => OnExitButtonHandled(ContactType.PointerDown));
            startButtonInfo.EventTrigger.
                AddListener(EventTriggerType.PointerDown, () => OnStartButtonHandled(ContactType.PointerDown));
            creditButtonInfo.EventTrigger.
                AddListener(EventTriggerType.PointerDown, () => OnCreditButtonHandled(ContactType.PointerDown));

            exitButtonInfo.EventTrigger.
                AddListener(EventTriggerType.PointerClick, () => OnExitButtonHandled(ContactType.PointerClick));
            startButtonInfo.EventTrigger.
                AddListener(EventTriggerType.PointerClick, () => OnStartButtonHandled(ContactType.PointerClick));
            creditButtonInfo.EventTrigger.
                AddListener(EventTriggerType.PointerClick, () => OnCreditButtonHandled(ContactType.PointerClick));

            StartCoroutine(Ex.Wait(() => isExitButtonHandlable = true, SO_System.Entity.BeforeButtonDur));
            StartCoroutine(Ex.Wait(() => isStartButtonHandlable = true, SO_System.Entity.BeforeButtonDur));
            StartCoroutine(Ex.Wait(() => isCreditButtonHandlable = true, SO_System.Entity.BeforeButtonDur));
        }

        private void OnDisable()
        {
            _buttonSpritesList[2].Dispose();
            _buttonSpritesList[2] = null;
            _buttonSpritesList[1].Dispose();
            _buttonSpritesList[1] = null;
            _buttonSpritesList[0].Dispose();
            _buttonSpritesList[0] = null;
            _buttonSpritesList = null;

            _buttonInfoList[2].Dispose();
            _buttonInfoList[2] = null;
            _buttonInfoList[1].Dispose();
            _buttonInfoList[1] = null;
            _buttonInfoList[0].Dispose();
            _buttonInfoList[0] = null;
            _buttonInfoList = null;

            _audioSource = null;
        }

        #endregion

        private void OnExitButtonHandled(ContactType type)
        {
            if (!isExitButtonHandlable) return;

            switch (type)
            {
                case ContactType.PointerEnter:
                    {
                        exitButtonInfo.Image.sprite = exitButtonSprites.Hover;
                    }
                    break;

                case ContactType.PointerExit:
                    {
                        exitButtonInfo.Image.sprite = exitButtonSprites.Normal;
                    }
                    break;

                case ContactType.PointerUp:
                    {

                    }
                    break;

                case ContactType.PointerDown:
                    {

                    }
                    break;

                case ContactType.PointerClick:
                    {
                        isExitButtonHandlable = false;
                        isStartButtonHandlable = false;
                        isCreditButtonHandlable = false;

                        audioSource.Raise(SO_Sound.Entity.ButtonClickSE, SType.SE);

                        exitButtonInfo.Image.sprite = exitButtonSprites.Click;

                        StartCoroutine(Ex.Wait(
                                () => Flow.QuitGame(),
                                SO_System.Entity.AfterButtonDur
                                ));
                    }
                    break;

                default:
                    throw new System.Exception("不正な種類です");
            }
        }

        private void OnStartButtonHandled(ContactType type)
        {
            if (!isStartButtonHandlable) return;

            switch (type)
            {
                case ContactType.PointerEnter:
                    {
                        startButtonInfo.Image.sprite = startButtonSprites.Hover;
                    }
                    break;

                case ContactType.PointerExit:
                    {
                        startButtonInfo.Image.sprite = startButtonSprites.Normal;
                    }
                    break;

                case ContactType.PointerUp:
                    {

                    }
                    break;

                case ContactType.PointerDown:
                    {

                    }
                    break;

                case ContactType.PointerClick:
                    {
                        isExitButtonHandlable = false;
                        isStartButtonHandlable = false;
                        isCreditButtonHandlable = false;

                        audioSource.Raise(SO_Sound.Entity.ButtonClickSE, SType.SE);

                        startButtonInfo.Image.sprite = startButtonSprites.Click;

                        StartCoroutine(Ex.Wait(
                                () => Flow.SceneChange(SO_SceneName.Entity.Main, false),
                                SO_System.Entity.AfterButtonDur
                                ));
                    }
                    break;

                default:
                    throw new System.Exception("不正な種類です");
            }
        }

        private void OnCreditButtonHandled(ContactType type)
        {
            if (!isCreditButtonHandlable) return;

            switch (type)
            {
                case ContactType.PointerEnter:
                    {
                        creditButtonInfo.Image.sprite = creditButtonSprites.Hover;
                    }
                    break;

                case ContactType.PointerExit:
                    {
                        creditButtonInfo.Image.sprite = creditButtonSprites.Normal;
                    }
                    break;

                case ContactType.PointerUp:
                    {

                    }
                    break;

                case ContactType.PointerDown:
                    {

                    }
                    break;

                case ContactType.PointerClick:
                    {
                        isExitButtonHandlable = false;
                        isStartButtonHandlable = false;
                        isCreditButtonHandlable = false;

                        audioSource.Raise(SO_Sound.Entity.ButtonClickSE, SType.SE);

                        creditButtonInfo.Image.sprite = creditButtonSprites.Click;

                        StartCoroutine(Ex.Wait(
                                () => Flow.SceneChange(SO_SceneName.Entity.Credit, false),
                                SO_System.Entity.AfterButtonDur
                                ));
                    }
                    break;

                default:
                    throw new System.Exception("不正な種類です");
            }
        }
    }

    #region

    internal static class ButtonExMethods
    {
        /// <summary>
        /// EventTriggerにイベントを登録する
        /// </summary>
        internal static void AddListener(this EventTrigger eventTrigger, EventTriggerType type, System.Action action)
        {
            EventTrigger.Entry entry = new();
            entry.eventID = type;
            entry.callback.AddListener((eventData) => { action(); });
            eventTrigger.triggers.Add(entry);
        }
    }

    [System.Serializable]
    internal sealed class ButtonSprites : System.IDisposable
    {
        [SerializeField, Header("通常")] private Sprite _normal;
        public Sprite Normal => _normal;

        [SerializeField, Header("ホバー")] private Sprite _hover;
        public Sprite Hover => _hover;

        [SerializeField, Header("押下")] private Sprite _click;
        public Sprite Click => _click;

        public void Dispose()
        {
            _normal = null;
            _hover = null;
            _click = null;
        }
    }

    [System.Serializable]
    internal sealed class ButtonInfo : System.IDisposable
    {
        [SerializeField, Header("Imageコンポーネント")] private Image _image;
        public Image Image => _image;

        [SerializeField, Header("EventTriggerコンポーネント")] private EventTrigger _eventTrigger;
        public EventTrigger EventTrigger => _eventTrigger;

        public void Dispose()
        {
            _image = null;
            _eventTrigger = null;
        }
    }

    #endregion
}