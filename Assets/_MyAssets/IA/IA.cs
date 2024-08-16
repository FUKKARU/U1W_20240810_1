//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/_MyAssets/IA/IA.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace IA
{
    public partial class @IA: IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @IA()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""IA"",
    ""maps"": [
        {
            ""name"": ""Main"",
            ""id"": ""7d3322de-7b19-4c54-af3d-b6c802e81f0e"",
            ""actions"": [
                {
                    ""name"": ""Charge"",
                    ""type"": ""Button"",
                    ""id"": ""44ae81c5-a6b6-41df-9b20-dda074acd7d2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Transform"",
                    ""type"": ""Button"",
                    ""id"": ""ba6c9735-a6e2-451b-bbee-092913087f74"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""59db8909-9ec2-4977-96ab-108b00b7b6f4"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""d00da5c4-cdf9-4784-a84c-ad5e8d73ea64"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""c1903215-fad9-4d39-afd6-b083c32296c8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""5200b73f-42cb-4083-ada3-29244e1037bd"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Charge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b8d4fd8f-a9e6-443e-9e1f-e03dc1a85006"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Transform"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""4c6d7016-ad6f-435f-9627-fe988e388c64"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""8e6fe26a-497a-4b77-9b06-b4f794b7be8f"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""bfc0324c-2e7c-4d68-8a89-16042573cf32"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""817cc790-cb28-4169-9c7b-482f8923ab2a"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""50954348-19c5-444a-afc0-c7cba7609f4d"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""511a97f1-0826-4687-999d-0eff664f38e1"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e2a0036a-b2a4-40f4-bed7-6f670133033e"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Main
            m_Main = asset.FindActionMap("Main", throwIfNotFound: true);
            m_Main_Charge = m_Main.FindAction("Charge", throwIfNotFound: true);
            m_Main_Transform = m_Main.FindAction("Transform", throwIfNotFound: true);
            m_Main_Move = m_Main.FindAction("Move", throwIfNotFound: true);
            m_Main_Interact = m_Main.FindAction("Interact", throwIfNotFound: true);
            m_Main_Cancel = m_Main.FindAction("Cancel", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }

        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // Main
        private readonly InputActionMap m_Main;
        private List<IMainActions> m_MainActionsCallbackInterfaces = new List<IMainActions>();
        private readonly InputAction m_Main_Charge;
        private readonly InputAction m_Main_Transform;
        private readonly InputAction m_Main_Move;
        private readonly InputAction m_Main_Interact;
        private readonly InputAction m_Main_Cancel;
        public struct MainActions
        {
            private @IA m_Wrapper;
            public MainActions(@IA wrapper) { m_Wrapper = wrapper; }
            public InputAction @Charge => m_Wrapper.m_Main_Charge;
            public InputAction @Transform => m_Wrapper.m_Main_Transform;
            public InputAction @Move => m_Wrapper.m_Main_Move;
            public InputAction @Interact => m_Wrapper.m_Main_Interact;
            public InputAction @Cancel => m_Wrapper.m_Main_Cancel;
            public InputActionMap Get() { return m_Wrapper.m_Main; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(MainActions set) { return set.Get(); }
            public void AddCallbacks(IMainActions instance)
            {
                if (instance == null || m_Wrapper.m_MainActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_MainActionsCallbackInterfaces.Add(instance);
                @Charge.started += instance.OnCharge;
                @Charge.performed += instance.OnCharge;
                @Charge.canceled += instance.OnCharge;
                @Transform.started += instance.OnTransform;
                @Transform.performed += instance.OnTransform;
                @Transform.canceled += instance.OnTransform;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Cancel.started += instance.OnCancel;
                @Cancel.performed += instance.OnCancel;
                @Cancel.canceled += instance.OnCancel;
            }

            private void UnregisterCallbacks(IMainActions instance)
            {
                @Charge.started -= instance.OnCharge;
                @Charge.performed -= instance.OnCharge;
                @Charge.canceled -= instance.OnCharge;
                @Transform.started -= instance.OnTransform;
                @Transform.performed -= instance.OnTransform;
                @Transform.canceled -= instance.OnTransform;
                @Move.started -= instance.OnMove;
                @Move.performed -= instance.OnMove;
                @Move.canceled -= instance.OnMove;
                @Interact.started -= instance.OnInteract;
                @Interact.performed -= instance.OnInteract;
                @Interact.canceled -= instance.OnInteract;
                @Cancel.started -= instance.OnCancel;
                @Cancel.performed -= instance.OnCancel;
                @Cancel.canceled -= instance.OnCancel;
            }

            public void RemoveCallbacks(IMainActions instance)
            {
                if (m_Wrapper.m_MainActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IMainActions instance)
            {
                foreach (var item in m_Wrapper.m_MainActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_MainActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public MainActions @Main => new MainActions(this);
        public interface IMainActions
        {
            void OnCharge(InputAction.CallbackContext context);
            void OnTransform(InputAction.CallbackContext context);
            void OnMove(InputAction.CallbackContext context);
            void OnInteract(InputAction.CallbackContext context);
            void OnCancel(InputAction.CallbackContext context);
        }
    }
}
