// GENERATED AUTOMATICALLY FROM 'Assets/Input/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""eabfb66c-f610-454f-bd81-e241a23e995d"",
            ""actions"": [
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""3c59236a-caba-4e68-8e0d-77b74a63978d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""e4a08636-b27f-4b5b-a3c6-b6efa2473fe9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Move Up"",
                    ""type"": ""Button"",
                    ""id"": ""8f8a4842-0b79-4607-ae99-e7949983ce05"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold(duration=0.2)""
                },
                {
                    ""name"": ""Move Down"",
                    ""type"": ""Button"",
                    ""id"": ""82e4cedc-bbb7-4a05-80db-7060a5400bf6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold(duration=0.2)""
                },
                {
                    ""name"": ""Move Left"",
                    ""type"": ""Button"",
                    ""id"": ""134fdef0-3d32-490d-978c-383dca88654d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold(duration=0.2)""
                },
                {
                    ""name"": ""Move Right"",
                    ""type"": ""Button"",
                    ""id"": ""9beabdd8-5941-4a8a-aaae-0565059544d1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold(duration=0.2)""
                },
                {
                    ""name"": ""Start"",
                    ""type"": ""Button"",
                    ""id"": ""302d42b5-11ca-4add-93ee-3daf86b82c75"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""L"",
                    ""type"": ""Button"",
                    ""id"": ""9f52d560-36e0-4e3d-b201-5f4a05087fbb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""R"",
                    ""type"": ""Button"",
                    ""id"": ""aca3efb1-f930-40ba-bf4d-25bf787c229f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""12643ece-1c95-4c55-8e2d-95a2c15d8070"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""79fe2b91-e5ff-40c5-881f-d5b22a10a31b"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4bf87c34-edea-4f43-a23a-a94ddb5bd84f"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b4ddc754-8038-4b10-963b-c0e40655af48"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d72159dc-2ba5-4280-bfa6-2372207fb09c"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7fae0dce-a8a5-4772-a974-33da7787326f"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e6f14e9d-ea1c-4183-b630-bad81e7bd944"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Start"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7b94be74-8e5f-4dd9-b556-90f3be3760f6"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""L"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""de4ea841-a7be-49df-9061-80ac51f8ab1b"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""L"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5aad7639-8f65-42cf-bf54-bf1a1da45023"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""R"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""90f40cee-ffa7-48e7-963c-03bc379c4635"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""R"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Interact = m_Gameplay.FindAction("Interact", throwIfNotFound: true);
        m_Gameplay_Cancel = m_Gameplay.FindAction("Cancel", throwIfNotFound: true);
        m_Gameplay_MoveUp = m_Gameplay.FindAction("Move Up", throwIfNotFound: true);
        m_Gameplay_MoveDown = m_Gameplay.FindAction("Move Down", throwIfNotFound: true);
        m_Gameplay_MoveLeft = m_Gameplay.FindAction("Move Left", throwIfNotFound: true);
        m_Gameplay_MoveRight = m_Gameplay.FindAction("Move Right", throwIfNotFound: true);
        m_Gameplay_Start = m_Gameplay.FindAction("Start", throwIfNotFound: true);
        m_Gameplay_L = m_Gameplay.FindAction("L", throwIfNotFound: true);
        m_Gameplay_R = m_Gameplay.FindAction("R", throwIfNotFound: true);
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

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_Interact;
    private readonly InputAction m_Gameplay_Cancel;
    private readonly InputAction m_Gameplay_MoveUp;
    private readonly InputAction m_Gameplay_MoveDown;
    private readonly InputAction m_Gameplay_MoveLeft;
    private readonly InputAction m_Gameplay_MoveRight;
    private readonly InputAction m_Gameplay_Start;
    private readonly InputAction m_Gameplay_L;
    private readonly InputAction m_Gameplay_R;
    public struct GameplayActions
    {
        private @PlayerControls m_Wrapper;
        public GameplayActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Interact => m_Wrapper.m_Gameplay_Interact;
        public InputAction @Cancel => m_Wrapper.m_Gameplay_Cancel;
        public InputAction @MoveUp => m_Wrapper.m_Gameplay_MoveUp;
        public InputAction @MoveDown => m_Wrapper.m_Gameplay_MoveDown;
        public InputAction @MoveLeft => m_Wrapper.m_Gameplay_MoveLeft;
        public InputAction @MoveRight => m_Wrapper.m_Gameplay_MoveRight;
        public InputAction @Start => m_Wrapper.m_Gameplay_Start;
        public InputAction @L => m_Wrapper.m_Gameplay_L;
        public InputAction @R => m_Wrapper.m_Gameplay_R;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @Interact.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteract;
                @Cancel.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCancel;
                @Cancel.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCancel;
                @Cancel.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCancel;
                @MoveUp.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveUp;
                @MoveUp.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveUp;
                @MoveUp.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveUp;
                @MoveDown.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveDown;
                @MoveDown.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveDown;
                @MoveDown.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveDown;
                @MoveLeft.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveLeft;
                @MoveLeft.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveLeft;
                @MoveLeft.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveLeft;
                @MoveRight.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveRight;
                @MoveRight.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveRight;
                @MoveRight.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMoveRight;
                @Start.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnStart;
                @Start.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnStart;
                @Start.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnStart;
                @L.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnL;
                @L.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnL;
                @L.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnL;
                @R.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnR;
                @R.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnR;
                @R.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnR;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Cancel.started += instance.OnCancel;
                @Cancel.performed += instance.OnCancel;
                @Cancel.canceled += instance.OnCancel;
                @MoveUp.started += instance.OnMoveUp;
                @MoveUp.performed += instance.OnMoveUp;
                @MoveUp.canceled += instance.OnMoveUp;
                @MoveDown.started += instance.OnMoveDown;
                @MoveDown.performed += instance.OnMoveDown;
                @MoveDown.canceled += instance.OnMoveDown;
                @MoveLeft.started += instance.OnMoveLeft;
                @MoveLeft.performed += instance.OnMoveLeft;
                @MoveLeft.canceled += instance.OnMoveLeft;
                @MoveRight.started += instance.OnMoveRight;
                @MoveRight.performed += instance.OnMoveRight;
                @MoveRight.canceled += instance.OnMoveRight;
                @Start.started += instance.OnStart;
                @Start.performed += instance.OnStart;
                @Start.canceled += instance.OnStart;
                @L.started += instance.OnL;
                @L.performed += instance.OnL;
                @L.canceled += instance.OnL;
                @R.started += instance.OnR;
                @R.performed += instance.OnR;
                @R.canceled += instance.OnR;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    public interface IGameplayActions
    {
        void OnInteract(InputAction.CallbackContext context);
        void OnCancel(InputAction.CallbackContext context);
        void OnMoveUp(InputAction.CallbackContext context);
        void OnMoveDown(InputAction.CallbackContext context);
        void OnMoveLeft(InputAction.CallbackContext context);
        void OnMoveRight(InputAction.CallbackContext context);
        void OnStart(InputAction.CallbackContext context);
        void OnL(InputAction.CallbackContext context);
        void OnR(InputAction.CallbackContext context);
    }
}
