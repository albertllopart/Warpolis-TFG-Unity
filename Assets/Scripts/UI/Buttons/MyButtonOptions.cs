using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButtonOptions : MyButtonTemplate
{
    public override void OnClick()
    {
        base.OnClick();
        Debug.Log("MyButtonOptions::OnClick");
    }
}
