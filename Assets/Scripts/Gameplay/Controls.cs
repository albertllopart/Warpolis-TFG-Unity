using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Controls : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent keyboard_w;
    [HideInInspector]
    public UnityEvent keyboard_w_up;
    [HideInInspector]
    public UnityEvent keyboard_w_down;
    [HideInInspector]
    public UnityEvent keyboard_a;
    [HideInInspector]
    public UnityEvent keyboard_a_up;
    [HideInInspector]
    public UnityEvent keyboard_a_down;
    [HideInInspector]
    public UnityEvent keyboard_s;
    [HideInInspector]
    public UnityEvent keyboard_s_up;
    [HideInInspector]
    public UnityEvent keyboard_s_down;
    [HideInInspector]
    public UnityEvent keyboard_d;
    [HideInInspector]
    public UnityEvent keyboard_d_up;
    [HideInInspector]
    public UnityEvent keyboard_d_down;

    [HideInInspector]
    public UnityEvent keyboard_o;
    [HideInInspector]
    public UnityEvent keyboard_o_up;
    [HideInInspector]
    public UnityEvent keyboard_o_down;
    [HideInInspector]
    public UnityEvent keyboard_k;
    [HideInInspector]
    public UnityEvent keyboard_k_up;
    [HideInInspector]
    public UnityEvent keyboard_k_down;

    [HideInInspector]
    public UnityEvent keyboard_return_down;

    //gamepad
    PlayerControls controls;

    bool up;
    bool down;
    bool left;
    bool right;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Gameplay.Interact.performed += ctx => SendODown(); //ctx serveix per dir-li a unity que sé que hi ha un context a l'acció però jo simplement vull cridar aquest mètode independentment d'aquest
        controls.Gameplay.Cancel.performed += ctx => SendKDown();
        controls.Gameplay.Start.performed += ctx => SendReturnDown();

        //W
        controls.Gameplay.MoveUp.started += ctx => SendWDown();
        controls.Gameplay.MoveUp.performed += ctx => SendW(true);
        controls.Gameplay.MoveUp.canceled += ctx => SendW(false);
        //S
        controls.Gameplay.MoveDown.started += ctx => SendSDown();
        controls.Gameplay.MoveDown.performed += ctx => SendS(true);
        controls.Gameplay.MoveDown.canceled += ctx => SendS(false);
        //A
        controls.Gameplay.MoveLeft.started += ctx => SendADown();
        controls.Gameplay.MoveLeft.performed += ctx => SendA(true);
        controls.Gameplay.MoveLeft.canceled += ctx => SendA(false);
        //D
        controls.Gameplay.MoveRight.started += ctx => SendDDown();
        controls.Gameplay.MoveRight.performed += ctx => SendD(true);
        controls.Gameplay.MoveRight.canceled += ctx => SendD(false);

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        keyboard_w = new UnityEvent();
        keyboard_w_up = new UnityEvent();
        keyboard_w_down = new UnityEvent();
        keyboard_a = new UnityEvent();
        keyboard_a_up = new UnityEvent();
        keyboard_a_down = new UnityEvent();
        keyboard_s = new UnityEvent();
        keyboard_s_up = new UnityEvent();
        keyboard_s_down = new UnityEvent();
        keyboard_d = new UnityEvent();
        keyboard_d_up = new UnityEvent();
        keyboard_d_down = new UnityEvent();

        keyboard_o = new UnityEvent();
        keyboard_o_up = new UnityEvent();
        keyboard_o_down = new UnityEvent();
        keyboard_k = new UnityEvent();
        keyboard_k_up = new UnityEvent();
        keyboard_k_down = new UnityEvent();

        keyboard_return_down = new UnityEvent();
    }

    // Update is called once per frame
    void Update()
    {
        InvokeW();
        InvokeA();
        InvokeS();
        InvokeD();

        InvokeO();
        InvokeK();
        InvokeReturn();

        if (up)
            keyboard_w.Invoke();
        if (down)
            keyboard_s.Invoke();
        if (left)
            keyboard_a.Invoke();
        if (right)
            keyboard_d.Invoke();
    }

    void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    void InvokeW()
    {
        if (Input.GetKey("w"))
            keyboard_w.Invoke();
        if (Input.GetKeyUp("w"))
            keyboard_w_up.Invoke();
        if (Input.GetKeyDown("w"))
            keyboard_w_down.Invoke();
    }

    void SendWDown()
    {
        keyboard_w_down.Invoke();
    }

    void SendW(bool send)
    {
        up = send;
    }

    void InvokeA()
    {
        if (Input.GetKey("a"))
            keyboard_a.Invoke();
        if (Input.GetKeyUp("a"))
            keyboard_a_up.Invoke();
        if (Input.GetKeyDown("a"))
            keyboard_a_down.Invoke();
    }

    void SendADown()
    {
        keyboard_a_down.Invoke();
    }

    void SendA(bool send)
    {
        left = send;
    }

    void InvokeS()
    {
        if (Input.GetKey("s"))
            keyboard_s.Invoke();
        if (Input.GetKeyUp("s"))
            keyboard_s_up.Invoke();
        if (Input.GetKeyDown("s"))
            keyboard_s_down.Invoke();
    }

    void SendSDown()
    {
        keyboard_s_down.Invoke();
    }

    void SendS(bool send)
    {
        down = send;
    }

    void InvokeD()
    {
        if (Input.GetKey("d"))
            keyboard_d.Invoke();
        if (Input.GetKeyUp("d"))
            keyboard_d_up.Invoke();
        if (Input.GetKeyDown("d"))
            keyboard_d_down.Invoke();
    }

    void SendDDown()
    {
        keyboard_d_down.Invoke();
    }
    void SendD(bool send)
    {
        right = send;
    }

    void InvokeO()
    {
        if (Input.GetKey("o"))
            keyboard_o.Invoke();
        if (Input.GetKeyUp("o"))
            keyboard_o_up.Invoke();
        if (Input.GetKeyDown("o"))
            keyboard_o_down.Invoke();
    }

    void SendODown()
    {
        keyboard_o_down.Invoke();
    }

    void InvokeK()
    {
        if (Input.GetKey("k"))
            keyboard_k.Invoke();
        if (Input.GetKeyUp("k"))
            keyboard_k_up.Invoke();
        if (Input.GetKeyDown("k"))
            keyboard_k_down.Invoke();
    }

    void SendKDown()
    {
        keyboard_k_down.Invoke();
    }

    void InvokeReturn()
    {
        if (Input.GetKeyDown("return"))
            keyboard_return_down.Invoke();
    }

    void SendReturnDown()
    {
        keyboard_return_down.Invoke();
    }
}
