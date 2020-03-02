using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour
{
    private uint width;
    private uint height;
    private uint xOffset;
    private uint yOffset;

    private Vector2 topLeftCorner;
    private Vector2 bottomRightCorner;

    // Start is called before the first frame update
    void Start()
    {
        //Accedeixo dos fills cap dins del map controller per trobar el Tilemap corresponent a la base per obtenir-ne les dimensions del mapa

        if (transform.GetChild(0).transform.GetChild(0).GetComponent<Tilemap>() != null)
        {
            width = (uint)transform.GetChild(0).transform.GetChild(0).GetComponent<Tilemap>().size.x;
            height = (uint)transform.GetChild(0).transform.GetChild(0).GetComponent<Tilemap>().size.y;

            Debug.Log("MapController::Start - Map size is " + width + " x " + height);
        }
        else
        {
            Debug.LogError("MapController::Start - No Tilemap found");
        }

        HelloWorld();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public uint GetWidth()
    {
        return width;
    }

    public uint GetHeight()
    {
        return height;
    }

    public uint GetxOffset()
    {
        return xOffset;
    }

    public uint GetyOffset()
    {
        return yOffset;
    }

    // Posicionament inicial

    void HelloWorld()
    {
        if (width % 2 == 0)
            xOffset = width / 2;
        else
            xOffset = (width - 1) / 2;

        if (height % 2 == 0)
            yOffset = height / 2;
        else
            yOffset = (height - 1) / 2;

        Debug.Log("MapController::HelloWorld - xOffset = " + xOffset + ", yOffset = " + yOffset);

        Vector3 offset = new Vector3(xOffset, -yOffset, 0);
        gameObject.transform.GetChild(0).transform.position += offset;

        // Aplico el mateix offset a la càmera
        Camera.main.GetComponent<Transform>().position += offset;
        //Camera.main.GetComponent<CameraController>().SetCameraMoved(true); <-------- No entenc per què l'Start de la càmera es fa després que el de mapa, aquesta línia és per si en algun moment deixa de passar

        topLeftCorner = transform.position;
        bottomRightCorner = (Vector2)transform.position + new Vector2(width - 1, -height + 1);

        Debug.Log("MapController::HelloWorld - TopLeft = " + topLeftCorner + " , BottomRight = " + bottomRightCorner);
    }

    public Vector2 GetTopLeftCorner()
    {
        return topLeftCorner;
    }

    public Vector2 GetBottomRightCorner()
    {
        return bottomRightCorner;
    }
}
