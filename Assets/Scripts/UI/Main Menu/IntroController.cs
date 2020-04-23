using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroController : MonoBehaviour
{
    //intro
    enum IntroState { BLACK1, TEXT, BLACK2 };
    public GameObject introText;
    IntroState introState = IntroState.BLACK1;
    bool intro = true;
    float timer = 0.0f;
    float introTime1 = 1.0f;
    float introTime2 = 4.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (intro)
            Intro();
    }

    void Intro()
    {
        timer += Time.deltaTime;

        switch (introState)
        {
            case IntroState.BLACK1:
                if (timer > introTime1)
                {
                    Instantiate(introText, new Vector3(0, 0, 0), Quaternion.identity);
                    FindObjectOfType<SoundController>().PlayButton();
                    introState = IntroState.TEXT;
                    timer = 0.0f;
                }
                break;

            case IntroState.TEXT:
                if (timer > introTime2)
                {
                    Destroy(GameObject.Find("Intro Text(Clone)"));
                    introState = IntroState.BLACK2;
                    timer = 0.0f;
                }
                break;

            case IntroState.BLACK2:
                if (timer > introTime1)
                {
                    Loader.Load(Loader.Scene.title);
                }
                break;
        }
    }
}
