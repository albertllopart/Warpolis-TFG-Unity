using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyText : MonoBehaviour
{
    public enum Anchor
    {
        LEFT, CENTERED
    };

    public string text;
    public Color color;
    public Anchor anchor;
    public int layer;

    void Awake()
    {

    }
}
