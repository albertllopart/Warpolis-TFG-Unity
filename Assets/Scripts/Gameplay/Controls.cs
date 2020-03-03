using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    void InvokeA()
    {
        if (Input.GetKey("a"))
            keyboard_a.Invoke();
        if (Input.GetKeyUp("a"))
            keyboard_a_up.Invoke();
        if (Input.GetKeyDown("a"))
            keyboard_a_down.Invoke();
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
    void InvokeD()
    {
        if (Input.GetKey("d"))
            keyboard_d.Invoke();
        if (Input.GetKeyUp("d"))
            keyboard_d_up.Invoke();
        if (Input.GetKeyDown("d"))
            keyboard_d_down.Invoke();
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
    void InvokeK()
    {
        if (Input.GetKey("k"))
            keyboard_k.Invoke();
        if (Input.GetKeyUp("k"))
            keyboard_k_up.Invoke();
        if (Input.GetKeyDown("k"))
            keyboard_k_down.Invoke();
    }
}
