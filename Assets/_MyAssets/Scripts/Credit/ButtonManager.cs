using General;
using SO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Credit.Button
{
    public sealed class ButtonManager : MonoBehaviour
    {
        #region

        [SerializeField, Header("�߂�{�^���̃X�v���C�g")] private ButtonSprites _backButtonSprites;
        private ButtonSprites backButtonSprites => _backButtonSprites;

        [SerializeField, Header("�߂�{�^���̃R���|�[�l���g")] private ButtonInfo _backButtonInfo;
        private ButtonInfo backButtonInfo => _backButtonInfo;

        [SerializeField, Header("AudioSource")] private AudioSource _audioSource;
        private AudioSource audioSource => _audioSource;

        private bool isBackButtonHandlable = false;

        private enum ContactType
        {
            /// <summary>
            /// �|�C���^�[���Ώۂɏ����
            /// </summary>
            PointerEnter,

            /// <summary>
            /// �|�C���^�[���Ώۂ��痣�ꂽ
            /// </summary>
            PointerExit,

            /// <summary>
            /// �|�C���^�[���Ώۂ���������
            /// </summary>
            PointerUp,

            /// <summary>
            /// �|�C���^�[��������Ԃ��痣����
            /// </summary>
            PointerDown,

            /// <summary>
            /// �Ώۂ̏�Ń|�C���^�[���������A����̑Ώۂ̏�ŗ�����
            /// </summary>
            PointerClick,
        }

        private void OnEnable()
        {
            backButtonInfo.Image.sprite = backButtonSprites.Normal;

            backButtonInfo.EventTrigger.
                AddListener(EventTriggerType.PointerEnter, () => OnBackButtonHandled(ContactType.PointerEnter));

            backButtonInfo.EventTrigger.
                AddListener(EventTriggerType.PointerExit, () => OnBackButtonHandled(ContactType.PointerExit));

            backButtonInfo.EventTrigger.
                AddListener(EventTriggerType.PointerUp, () => OnBackButtonHandled(ContactType.PointerUp));

            backButtonInfo.EventTrigger.
                AddListener(EventTriggerType.PointerDown, () => OnBackButtonHandled(ContactType.PointerDown));

            backButtonInfo.EventTrigger.
                AddListener(EventTriggerType.PointerClick, () => OnBackButtonHandled(ContactType.PointerClick));

            StartCoroutine(Ex.Wait(() => isBackButtonHandlable = true, SO_System.Entity.BeforeButtonDur));
        }

        private void OnDisable()
        {
            _backButtonSprites.Dispose();
            _backButtonSprites = null;

            _backButtonInfo.Dispose();
            _backButtonInfo = null;

            _audioSource = null;
        }

        #endregion

        private void OnBackButtonHandled(ContactType type)
        {
            if (!isBackButtonHandlable) return;

            switch (type)
            {
                case ContactType.PointerEnter:
                    {
                        backButtonInfo.Image.sprite = backButtonSprites.Hover;
                    }
                    break;

                case ContactType.PointerExit:
                    {
                        backButtonInfo.Image.sprite = backButtonSprites.Normal;
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
                        isBackButtonHandlable = false;

                        audioSource.Raise(SO_Sound.Entity.ButtonClickSE, SType.SE);

                        backButtonInfo.Image.sprite = backButtonSprites.Click;

                        StartCoroutine(Ex.Wait(
                                () => Flow.SceneChange(SO_SceneName.Entity.Title, false),
                                SO_System.Entity.AfterButtonDur
                                ));
                    }
                    break;

                default:
                    throw new System.Exception("�s���Ȏ�ނł�");
            }
        }
    }

    #region

    internal static class ButtonExMethods
    {
        /// <summary>
        /// EventTrigger�ɃC�x���g��o�^����
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
        [SerializeField, Header("�ʏ�")] private Sprite _normal;
        public Sprite Normal => _normal;

        [SerializeField, Header("�z�o�[")] private Sprite _hover;
        public Sprite Hover => _hover;

        [SerializeField, Header("����")] private Sprite _click;
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
        [SerializeField, Header("Image�R���|�[�l���g")] private Image _image;
        public Image Image => _image;

        [SerializeField, Header("EventTrigger�R���|�[�l���g")] private EventTrigger _eventTrigger;
        public EventTrigger EventTrigger => _eventTrigger;

        public void Dispose()
        {
            _image = null;
            _eventTrigger = null;
        }
    }

    #endregion
}