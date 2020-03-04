using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButtonQuit : MyButtonTemplate
{
    public override void OnClick()
    {
        base.OnClick();

        Application.Quit();
    }
}
