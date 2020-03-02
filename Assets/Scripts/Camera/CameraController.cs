using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector2 topLeftCorner;
    private Vector2 topRightCorner;
    private Vector2 bottomLeftCorner;
    private Vector2 bottomRightCorner;

    private bool cameraMoved = false;
    
    // Start is called before the first frame update
    void Start()
    {
        CalculateCameraCorners();
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraMoved)
        {
            CalculateCameraCorners();
            cameraMoved = false;
        }
    }
    public void CalculateCameraCorners()
    {
        topLeftCorner = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, Camera.main.nearClipPlane));
        topRightCorner = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));
        bottomLeftCorner = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        bottomRightCorner = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, Camera.main.nearClipPlane));

        RoundCameraCornerValues();
    }

    void RoundCameraCornerValues()
    {
        topLeftCorner.x = Mathf.Round(topLeftCorner.x);
        topLeftCorner.y = Mathf.Round(topLeftCorner.y);

        topRightCorner.x = Mathf.Round(topRightCorner.x) - 1;
        topRightCorner.y = Mathf.Round(topRightCorner.y);

        bottomLeftCorner.x = Mathf.Round(bottomLeftCorner.x);
        bottomLeftCorner.y = Mathf.Round(bottomLeftCorner.y) + 1;

        bottomRightCorner.x = Mathf.Round(bottomRightCorner.x) - 1;
        bottomRightCorner.y = Mathf.Round(bottomRightCorner.y) + 1; // aquests valors són per aconseguir exactament la cantonada, ja que això retorna valors de mapa aka top left de la casella

        Debug.Log("CameraController::RoundCameraCornerValues - Top Left = " + topLeftCorner +
                                                            " , Top Right = " + topRightCorner +
                                                            " , Bottom Left = " + bottomLeftCorner +
                                                            " , Bottom Right = " + bottomRightCorner);
    }

    public void SetCameraMoved(bool set)
    {
        cameraMoved = set;
    }

    public void MoveCameraUp()
    {
        gameObject.transform.position += new Vector3(0, 1, 0);
        cameraMoved = true;
    }

    public void MoveCameraLeft()
    {
        gameObject.transform.position += new Vector3(-1, 0, 0);
        cameraMoved = true;
    }

    public void MoveCameraDown()
    {
        gameObject.transform.position += new Vector3(0, -1, 0);
        cameraMoved = true;
    }

    public void MoveCameraRight()
    {
        gameObject.transform.position += new Vector3(1, 0, 0);
        cameraMoved = true;
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
