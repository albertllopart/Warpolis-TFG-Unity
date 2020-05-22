using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraController : MonoBehaviour
{
    private Vector2 topLeftCorner;
    private Vector2 topRightCorner;
    private Vector2 bottomLeftCorner;
    private Vector2 bottomRightCorner;

    private uint cameraWidth;

    float timer = 0.0f;

    //transitions
    bool fadeToWhite = false;
    float ftwTime = 0.025f;
    float ftwAlpha = 0.0f;
    bool ftwReturn = false;
    bool ftwToRest = false;
    float restingTime = 0.0f;
    float restingTimer = 0.0f;

    //events
    public UnityEvent cameraMoved;
    public UnityEvent fadeToWhiteEnd;
    public UnityEvent fadeToWhiteRest;

    // Start is called before the first frame update
    void Start()
    {
        fadeToWhiteEnd = new UnityEvent();
        fadeToWhiteRest = new UnityEvent();
        cameraMoved = new UnityEvent();

        CalculateCameraCorners();
        CalculateCameraWidth();
    }

    public void AfterStart()
    {
        SubscribeToEvents();
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeToWhite && !ftwToRest)
            FadeToWhite();

        if (ftwToRest)
            FtwRest();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 mouse = Input.mousePosition;
            mouse = Camera.main.ScreenToWorldPoint(mouse);
            mouse.x = (int)mouse.x;
            mouse.y = (int)mouse.y;
            mouse.z = 0;
            FindObjectOfType<MapController>().LogTileInfo(mouse);
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

        /*Debug.Log("CameraController::RoundCameraCornerValues - Top Left = " + topLeftCorner +
                                                            " , Top Right = " + topRightCorner +
                                                            " , Bottom Left = " + bottomLeftCorner +
                                                            " , Bottom Right = " + bottomRightCorner);*/
    }

    void CalculateCameraWidth()
    {
        if (topLeftCorner.x >= 0)
        {
            cameraWidth = (uint)topRightCorner.x - (uint)topLeftCorner.x + 1; // +1 perquè el top right corner no arriba a la casella següent
            Debug.Log("CameraController::CalculateCameraWidth - Camera Width = " + cameraWidth);
        }
        else
        {
            //Debug.LogError("CameraController::CalculateCameraWidth - TopLeftCorner.x < 0");
        }
    }

    public void MoveCameraUp()
    {
        gameObject.transform.position += new Vector3(0, 1, 0);
        cameraMoved.Invoke();
    }

    public void MoveCameraLeft()
    {
        gameObject.transform.position += new Vector3(-1, 0, 0);
        cameraMoved.Invoke();
    }

    public void MoveCameraDown()
    {
        gameObject.transform.position += new Vector3(0, -1, 0);
        cameraMoved.Invoke();
    }

    public void MoveCameraRight()
    {
        gameObject.transform.position += new Vector3(1, 0, 0);
        cameraMoved.Invoke();
    }

    public void CameraTraslation(Vector3 goal)
    {
        if (goal.z != -10)
        {
            goal.z = -10;
        }

        transform.position = goal;

        cameraMoved.Invoke();
    }

    public Vector2 GetTopLeftCorner()
    {
        return topLeftCorner;
    }

    public Vector2 GetBottomRightCorner()
    {
        return bottomRightCorner;
    }

    public Vector2 GetTopRightCorner()
    {
        return topRightCorner;
    }

    public uint GetCameraWidth()
    {
        return cameraWidth;
    }

    void OnEndGame()
    {
        FadeToWhiteSetup(1.0f);

        fadeToWhiteRest.AddListener(GameObject.Find("Map Controller").GetComponent<MapController>().UnloadMap);
        fadeToWhiteRest.AddListener(GameObject.Find("Menu Controller").GetComponent<MenuController>().OnGameEnded);
        //fadeToWhiteEnd.AddListener(GameObject.Find("Menu Controller").GetComponent<MenuController>().CompleteGameLoop);
    }

    public void FadeToWhiteSetup(float restingTime)
    {
        fadeToWhite = true;
        timer = 0.0f;
        ftwReturn = false;
        this.restingTime = restingTime;
        restingTimer = 0.0f;

        transform.Find("Fade To White").gameObject.SetActive(true);
    }

    void FadeToWhite()
    {
        timer += Time.deltaTime;

        if (timer >= ftwTime)
        {
            switch (ftwReturn)
            {
                case false:
                    FtwIncreaseAlpha();
                    break;

                case true:
                    FtwDecreaseAlpha();
                    break;
            }
        }

        if (ftwReturn && ftwAlpha <= 0.0f)
        {
            //finish
            transform.Find("Fade To White").gameObject.SetActive(false);
            fadeToWhite = false;
            fadeToWhiteEnd.Invoke();
        }
    }

    void FtwIncreaseAlpha()
    {
        if (ftwAlpha >= 1.0f)
        {
            Debug.Log("CameraController::FtwIncreaseAlpha - Starting to Rest");
            ftwToRest = true;
            fadeToWhiteRest.Invoke();
            return;
        }

        timer = 0.0f;

        Color color = new Color(1, 1, 1, ftwAlpha);

        transform.Find("Fade To White").GetComponent<SpriteRenderer>().color = color;

        ftwAlpha += 0.025f;
    }

    void FtwDecreaseAlpha()
    {
        timer = 0.0f;

        Color color = new Color(1, 1, 1, ftwAlpha);

        transform.Find("Fade To White").GetComponent<SpriteRenderer>().color = color;

        ftwAlpha -= 0.025f;
    }

    void FtwRest()
    {
        restingTimer += Time.deltaTime;

        if (restingTimer >= restingTime)
        {
            Debug.Log("CameraController::FtwRest - Finished Resting");
            ftwToRest = false;
            ftwReturn = true;
            restingTimer = 0.0f;
        }
    }

    void SubscribeToEvents()
    {
        cameraMoved.AddListener(CalculateCameraCorners);

        GameObject.Find("Menu Controller").GetComponent<MenuController>().endGame.AddListener(OnEndGame);
    }
}
