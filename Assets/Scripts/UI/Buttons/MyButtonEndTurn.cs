using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButtonEndTurn : MyButtonTemplate
{
    public override void OnClick()
    {
        base.OnClick();
        Debug.Log("MyButtonEndTurn::OnClick");
    }
}