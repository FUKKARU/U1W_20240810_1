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
        Value0,
        Value1,
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
                InputType.Null
                => null,
                InputType.Click
                => new() { (InputAction.CallbackContext context) => { val0 = true; } },
                InputType.Hold
                => new() { (InputAction.CallbackContext context) => { val0 = true; } },
                InputType.Value0
                => new() { (InputAction.CallbackContext context) => { val0 = true; },
                    (InputAction.CallbackContext context) => { val0 = false; } },
                InputType.Value1
                => new() { (InputAction.CallbackContext context) => { val1 = context.ReadValue<float>(); } },
                InputType.Value2
                => new() { (InputAction.CallbackContext context) => { val2 = context.ReadValue<Vector2>(); } },
                InputType.Value3
                => new() { (InputAction.CallbackContext context) => { val3 = context.ReadValue<Vector3>(); } },
                _ => null
            };
        }

        private readonly InputAction inputAction = null;
        private readonly InputType type = InputType.Null;
        private readonly List<System.Action<InputAction.CallbackContext>> action = null;

        private bool val0 = false;
        private float val1 = 0;
        private Vector2 val2 = Vector2.zero;
        private Vector3 val3 = Vector3.zero;

        public T Get<T>()
        {
            object ret = type switch
            {
                InputType.Null => null,
                InputType.Click => val0,
                InputType.Hold => val0,
                InputType.Value0 => val0,
                InputType.Value1 => val1,
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
                        inputAction.performed += action[0];
                        break;

                    case InputType.Hold:
                        inputAction.performed += action[0];
                        break;

                    case InputType.Value0:
                        inputAction.performed += action[0];
                        inputAction.canceled += action[1];
                        break;

                    case InputType.Value1:
                        inputAction.started += action[0];
                        inputAction.performed += action[0];
                        inputAction.canceled += action[0];
                        break;

                    case InputType.Value2:
                        inputAction.started += action[0];
                        inputAction.performed += action[0];
                        inputAction.canceled += action[0];
                        break;

                    case InputType.Value3:
                        inputAction.started += action[0];
                        inputAction.performed += action[0];
                        inputAction.canceled += action[0];
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
                        inputAction.performed -= action[0];
                        break;

                    case InputType.Hold:
                        inputAction.performed -= action[0];
                        break;

                    case InputType.Value0:
                        inputAction.performed -= action[0];
                        inputAction.canceled -= action[1];
                        break;

                    case InputType.Value1:
                        inputAction.started -= action[0];
                        inputAction.performed -= action[0];
                        inputAction.canceled -= action[0];
                        break;

                    case InputType.Value2:
                        inputAction.started -= action[0];
                        inputAction.performed -= action[0];
                        inputAction.canceled -= action[0];
                        break;

                    case InputType.Value3:
                        inputAction.started -= action[0];
                        inputAction.performed -= action[0];
                        inputAction.canceled -= action[0];
                        break;

                    default:
                        break;
                }
            }
        }

        public void OnLateUpdate()
        {
            if (type == InputType.Click && val0) val0 = false;
            if (type == InputType.Hold && val0) val0 = false;
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

        public InputInfo Main_ChargeValue0 { get; private set; } = null;
        public InputInfo Main_TransformClick { get; private set; } = null;
        public InputInfo Main_MoveValue2 { get; private set; } = null;

        private void Init()
        {
            Main_ChargeValue0 = new InputInfo(_inputs.Main.Charge, InputType.Value0).Add(inputInfoList);
            Main_TransformClick = new InputInfo(_inputs.Main.Transform, InputType.Click).Add(inputInfoList);
            Main_MoveValue2 = new InputInfo(_inputs.Main.Move, InputType.Value2).Add(inputInfoList);
        }
    }
}
