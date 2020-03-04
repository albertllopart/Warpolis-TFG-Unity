using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float keyDownCounterW; //per calcular el temps que el botó porta premut
    private float keyDownCounterA;
    private float keyDownCounterS;
    private float keyDownCounterD;
            
    private float nextMoveCounterW; //per calcular el temps que cal esperar a moure una casella mentre el botó està premut
    private float nextMoveCounterA;
    private float nextMoveCounterS;
    private float nextMoveCounterD;

    public float keyDownSpeed = 0.25f;
    public float nextMoveSpeed = 0.1f;

    private uint xCameraMovementOffset = 2;
    private uint yCameraMovementOffset = 1; // la x és 2 perquè necessita tenir en compte la oclusió dels laterals

    // Start is called before the first frame update
    void Start()
    {
        SubscribeToEvents();

        keyDownCounterW = 0.0f;
        keyDownCounterA = 0.0f;
        keyDownCounterS = 0.0f;
        keyDownCounterD = 0.0f;

        nextMoveCounterW = 0.0f;
        nextMoveCounterA = 0.0f;
        nextMoveCounterS = 0.0f;
        nextMoveCounterD = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Interact();
        LogPosition();
    }

    public void MyOnEnable()
    {
        SubscribeToEvents();
    }
    public void MyOnDisable()
    {
        UnsubscribeFromEvents();
    }

    // Moviment
    void Move()
    {
        //he traslladat la major part del moviment a events. El que hi he deixat no pot escoltar events perquè rep paràmetres

        if (Input.GetKeyUp("w"))
        {
            resetCounterParameters(0);
        }
        if (Input.GetKeyUp("a"))
        {
            resetCounterParameters(1);
        }
        if (Input.GetKeyUp("s"))
        {
            resetCounterParameters(2);
        }
        if (Input.GetKeyUp("d"))
        {
            resetCounterParameters(3);
        }
    }

    void resetCounterParameters(uint key)
    {
        if (key == 0) // W
        {
            keyDownCounterW = 0.0f;
            nextMoveCounterW = 0.0f;
        }
        if (key == 1) // A
        {
            keyDownCounterA = 0.0f;
            nextMoveCounterA = 0.0f;
        }
        if (key == 2) // S
        {
            keyDownCounterS = 0.0f;
            nextMoveCounterS = 0.0f;
        }
        if (key == 3) // D
        {
            keyDownCounterD = 0.0f;
            nextMoveCounterD = 0.0f;
        }
    }

    void checkEnhancedW()
    {
        if (Input.GetKey("w"))
        {
            if (keyDownCounterW < keyDownSpeed)
                keyDownCounterW += Time.deltaTime;

            nextMoveCounterW += Time.deltaTime;

            if (keyDownCounterW >= keyDownSpeed)
            {
                if (nextMoveCounterW >= nextMoveSpeed)
                {
                    MovePlayerUp();
                    nextMoveCounterW = 0.0f;
                }
            }
        }
    }

    void checkEnhancedA()
    {
        if (Input.GetKey("a"))
        {
            if (keyDownCounterA < keyDownSpeed)
                keyDownCounterA += Time.deltaTime;

            nextMoveCounterA += Time.deltaTime;

            if (keyDownCounterA >= keyDownSpeed)
            {
                if (nextMoveCounterA >= nextMoveSpeed)
                {
                    MovePlayerLeft();
                    nextMoveCounterA = 0.0f;
                }
            }
        }
    }

    void checkEnhancedS()
    {
        if (Input.GetKey("s"))
        {
            if (keyDownCounterS < keyDownSpeed)
                keyDownCounterS += Time.deltaTime;

            nextMoveCounterS += Time.deltaTime;

            if (keyDownCounterS >= keyDownSpeed)
            {
                if (nextMoveCounterS >= nextMoveSpeed)
                {
                    MovePlayerDown();
                    nextMoveCounterS = 0.0f;
                }
            }
        }
    }

    void checkEnhancedD()
    {
        if (Input.GetKey("d"))
        {
            if (keyDownCounterD < keyDownSpeed)
                keyDownCounterD += Time.deltaTime;

            nextMoveCounterD += Time.deltaTime;

            if (keyDownCounterD >= keyDownSpeed)
            {
                if (nextMoveCounterD >= nextMoveSpeed)
                {
                    MovePlayerRight();
                    nextMoveCounterD = 0.0f;
                }
            }
        }
    }

    public void MovePlayerUp()
    {
        if (Camera.main.GetComponent<CameraController>().GetTopLeftCorner().y > transform.position.y)
        {
            transform.position += new Vector3(0, 1, 0);

            if (CheckCameraBoundaries(0))
                Camera.main.GetComponent<CameraController>().MoveCameraUp();
        }
    }

    public void MovePlayerLeft()
    {
        if (Camera.main.GetComponent<CameraController>().GetTopLeftCorner().x < transform.position.x - 1) // -1 per tenir en compte la oclusió
        {
            transform.position += new Vector3(-1, 0, 0);

            if (CheckCameraBoundaries(1))
                Camera.main.GetComponent<CameraController>().MoveCameraLeft();
        }
    }

    public void MovePlayerDown()
    {
        if (Camera.main.GetComponent<CameraController>().GetBottomRightCorner().y < transform.position.y)
        {
            transform.position += new Vector3(0, -1, 0);

            if (CheckCameraBoundaries(2))
                Camera.main.GetComponent<CameraController>().MoveCameraDown();
        }
    }

    public void MovePlayerRight()
    {
        if (Camera.main.GetComponent<CameraController>().GetBottomRightCorner().x > transform.position.x + 1) // +1 per tenir en compte la oclusió
        {
            transform.position += new Vector3(1, 0, 0);

            if (CheckCameraBoundaries(3))
                Camera.main.GetComponent<CameraController>().MoveCameraRight();
        }
    }

    bool CheckCameraBoundaries(int direction)
    {
        Vector2 cameraTopLeft = Camera.main.GetComponent<CameraController>().GetTopLeftCorner();
        Vector2 cameraBottomRight = Camera.main.GetComponent<CameraController>().GetBottomRightCorner();
        Vector2 mapTopLeft = GameObject.Find("Map Controller").GetComponent<MapController>().GetTopLeftCorner();
        Vector2 mapBottomRight = GameObject.Find("Map Controller").GetComponent<MapController>().GetBottomRightCorner();

        switch (direction)
        {
            case 0: // W
                if (cameraTopLeft.y - yCameraMovementOffset < gameObject.transform.position.y
                    && cameraTopLeft.y < mapTopLeft.y)
                    return true;
                break;

            case 1: // A
                if (cameraTopLeft.x + xCameraMovementOffset > gameObject.transform.position.x
                    && cameraTopLeft.x > mapTopLeft.x)
                    return true;
                break;

            case 2: // S
                if (cameraBottomRight.y + yCameraMovementOffset > gameObject.transform.position.y
                    && cameraBottomRight.y > mapBottomRight.y)
                    return true;
                break;

            case 3: // D
                if (cameraBottomRight.x - xCameraMovementOffset < gameObject.transform.position.x
                    && cameraBottomRight.x < mapBottomRight.x)
                    return true;
                break;
        }

        return false;
    }

    // Interacció

    void Interact()
    {
        if (Input.GetKeyDown("o"))
        {
            Vector2 from = transform.position;
            Vector2 to = transform.position;

            RaycastHit2D result = Physics2D.Linecast(from, to);

            if (result.collider != null)
                Debug.Log(result.collider.gameObject.name);
        }
    }

    // Debug

    void LogPosition()
    {
        if (Input.GetKeyDown("f1"))
        {
            Debug.Log(gameObject.transform.position);
        }
    }

    void SubscribeToEvents()
    {
        GetComponentInParent<Controls>().keyboard_w.AddListener(checkEnhancedW);
        GetComponentInParent<Controls>().keyboard_w_down.AddListener(MovePlayerUp);

        GetComponentInParent<Controls>().keyboard_a.AddListener(checkEnhancedA);
        GetComponentInParent<Controls>().keyboard_a_down.AddListener(MovePlayerLeft);

        GetComponentInParent<Controls>().keyboard_s.AddListener(checkEnhancedS);
        GetComponentInParent<Controls>().keyboard_s_down.AddListener(MovePlayerDown);

        GetComponentInParent<Controls>().keyboard_d.AddListener(checkEnhancedD);
        GetComponentInParent<Controls>().keyboard_d_down.AddListener(MovePlayerRight);
    }

    void UnsubscribeFromEvents()
    {
        GetComponentInParent<Controls>().keyboard_w.RemoveListener(checkEnhancedW);
        GetComponentInParent<Controls>().keyboard_w_down.RemoveListener(MovePlayerUp);

        GetComponentInParent<Controls>().keyboard_a.RemoveListener(checkEnhancedA);
        GetComponentInParent<Controls>().keyboard_a_down.RemoveListener(MovePlayerLeft);

        GetComponentInParent<Controls>().keyboard_s.RemoveListener(checkEnhancedS);
        GetComponentInParent<Controls>().keyboard_s_down.RemoveListener(MovePlayerDown);

        GetComponentInParent<Controls>().keyboard_d.RemoveListener(checkEnhancedD);
        GetComponentInParent<Controls>().keyboard_d_down.RemoveListener(MovePlayerRight);
    }
}
