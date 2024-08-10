using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IA
{
    #region

    public enum InputType
    {
        Null,
        Click,
        Hold,
        Value2,
        Value3
    }

    public class InputInfo
    {
        public InputInfo(InputAction inputAction, InputType type)
        {
            this.inputAction = inputAction;
            this.type = type;

            action = this.type switch
            {
                InputType.Null => null,
                InputType.Click => ((InputAction.CallbackContext context) => { val1 = true; }),
                InputType.Hold => ((InputAction.CallbackContext context) => { val1 = true; }),
                InputType.Value2 => ((InputAction.CallbackContext context) => { val2 = context.ReadValue<Vector2>(); }),
                InputType.Value3 => ((InputAction.CallbackContext context) => { val3 = context.ReadValue<Vector3>(); }),
                _ => null
            };
        }

        private InputAction inputAction = null;
        private InputType type = InputType.Null;
        private System.Action<InputAction.CallbackContext> action = null;

        private bool val1 = false;
        private Vector2 val2 = Vector2.zero;
        private Vector3 val3 = Vector3.zero;

        public T Get<T>()
        {
            object ret = null;

            ret = type switch
            {
                InputType.Null => null,
                InputType.Click => val1,
                InputType.Hold => val1,
                InputType.Value2 => val2,
                InputType.Value3 => val3,
                _ => null
            };

            return (T)ret;
        }

        public void Link(bool isLink)
        {
            if (isLink)
            {
                switch (type)
                {
                    case InputType.Null:
                        break;

                    case InputType.Click:
                        inputAction.performed += action;
                        break;

                    case InputType.Hold:
                        inputAction.performed += action;
                        break;

                    case InputType.Value2:
                        inputAction.started += action;
                        inputAction.performed += action;
                        inputAction.canceled += action;
                        break;

                    case InputType.Value3:
                        inputAction.started += action;
                        inputAction.performed += action;
                        inputAction.canceled += action;
                        break;

                    default:
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case InputType.Null:
                        break;

                    case InputType.Click:
                        inputAction.performed -= action;
                        break;

                    case InputType.Hold:
                        inputAction.performed -= action;
                        break;

                    case InputType.Value2:
                        inputAction.started -= action;
                        inputAction.performed -= action;
                        inputAction.canceled -= action;
                        break;

                    case InputType.Value3:
                        inputAction.started -= action;
                        inputAction.performed -= action;
                        inputAction.canceled -= action;
                        break;

                    default:
                        break;
                }
            }
        }

        public void OnLateUpdate()
        {
            if (type == InputType.Click && val1) val1 = false;
            if (type == InputType.Hold && val1) val1 = false;
        }
    }

    #endregion

    #region

    public static class Ex
    {
        public static InputInfo Add(this InputInfo inputInfo, List<InputInfo> list)
        {
            list.Add(inputInfo);

            return inputInfo;
        }
    }

    #endregion

    public class InputGetter : MonoBehaviour
    {
        #region

        IA _inputs = null;

        public static InputGetter Instance { get; set; } = null;

        private readonly List<InputInfo> inputInfoList = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            _inputs = new IA();

            Init();

            foreach (InputInfo e in inputInfoList)
            {
                e.Link(true);
            }
        }

        private void LateUpdate()
        {
            foreach (InputInfo e in inputInfoList)
            {
                e.OnLateUpdate();
            }
        }

        private void OnEnable()
        {
            _inputs.Enable();
        }

        private void OnDisable()
        {
            _inputs.Disable();
        }

        private void OnDestroy()
        {
            foreach (InputInfo e in inputInfoList)
            {
                e.Link(false);
            }

            _inputs.Dispose();
        }

        #endregion

        public InputInfo Main_Click { get; private set; } = null;
        public InputInfo Main_Hold { get; private set; } = null;
        public InputInfo Main_Value2 { get; private set; } = null;

        private void Init()
        {
            Main_Click = new InputInfo(_inputs.Main.Click, InputType.Click).Add(inputInfoList);
            Main_Hold = new InputInfo(_inputs.Main.Hold, InputType.Hold).Add(inputInfoList);
            Main_Value2 = new InputInfo(_inputs.Main.Value, InputType.Value2).Add(inputInfoList);
        }
    }
}
