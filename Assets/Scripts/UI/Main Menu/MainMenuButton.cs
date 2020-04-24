using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
    //traslation
    public Vector2 goal;

    //info
    public GameObject info;

    void OnSelected()
    {
        EnableInfo(true);
    }

    void OnDeselected()
    {
        EnableInfo(false);
    }

    public void EnableInfo(bool enable)
    {
        if (info != null)
        {
            info.SetActive(enable);

            if (enable)
            {
                info.transform.SetParent(FindObjectOfType<MainMenuController>().transform.Find("Mode").transform.Find("Info").transform.Find("TextPosition").transform);
                info.transform.localPosition = new Vector3(0, 0, 0);
            }
        }
    }
}
