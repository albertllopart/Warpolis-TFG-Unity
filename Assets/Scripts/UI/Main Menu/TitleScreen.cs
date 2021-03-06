﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public GameObject cani_infantry;
    public GameObject cani_transport;
    public GameObject cani_tank;
    public GameObject cani_aerial;
    public GameObject cani_gunner;
    public GameObject cani_ranged;

    public GameObject hipster_infantry;
    public GameObject hipster_transport;
    public GameObject hipster_tank;
    public GameObject hipster_aerial;
    public GameObject hipster_gunner;
    public GameObject hipster_ranged;

    List<GameObject> cani;
    List<GameObject> hipster;

    public float spawnTime;
    float timer = 0.0f;
    enum NextSpawn { CANI, HIPSTER };
    NextSpawn nextSpawn = NextSpawn.CANI;
    int lastCani;
    int lastHipster;

    //version
    public GameObject unit;
    public GameObject decimal1;

    // Start is called before the first frame update
    void Start()
    {
        cani = new List<GameObject>();
        cani.Add(cani_infantry);
        cani.Add(cani_transport);
        cani.Add(cani_tank);
        cani.Add(cani_aerial);
        cani.Add(cani_gunner);
        cani.Add(cani_ranged);

        hipster = new List<GameObject>();
        hipster.Add(hipster_infantry);
        hipster.Add(hipster_transport);
        hipster.Add(hipster_tank);
        hipster.Add(hipster_aerial);
        hipster.Add(hipster_gunner);
        hipster.Add(hipster_ranged);

        FindObjectOfType<SoundController>().PlayTitle();
        FindObjectOfType<FadeTo>().finishedDecreasing.AddListener(SubscribeToEvents);

        SetVersion();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnTime)
        {
            timer = 0.0f;
            Spawn();
        }
    }

    void SetVersion()
    {
        unit.GetComponent<Number>().CreateNumber(1);
        decimal1.GetComponent<Number>().CreateNumber(0);
    }

    void Spawn()
    {
        switch (nextSpawn)
        {
            case NextSpawn.CANI:
                nextSpawn = NextSpawn.HIPSTER;

                int crandom = lastCani;
                while (crandom == lastCani)
                    crandom = Random.Range(0, 6);

                GameObject cpuppet = Instantiate(cani[crandom], transform.Find("Spawner").transform.position, Quaternion.identity);
                cpuppet.transform.SetParent(transform.Find("Spawner"));
                if (crandom == 3)
                    cpuppet.transform.position += new Vector3(0, 1, 0);

                lastCani = crandom;
                break;

            case NextSpawn.HIPSTER:
                nextSpawn = NextSpawn.CANI;

                int hrandom = lastHipster;
                while (hrandom == lastHipster)
                    hrandom = Random.Range(0, 6);

                GameObject hpuppet = Instantiate(hipster[hrandom], transform.Find("Spawner").transform.position, Quaternion.identity);
                hpuppet.transform.SetParent(transform.Find("Spawner"));
                if (hrandom == 3)
                    hpuppet.transform.position += new Vector3(0, 1, 0);

                lastHipster = hrandom;
                break;
        }
    }

    public void DestroyPuppets()
    {
        foreach (Transform puppet in transform.Find("Spawner").transform)
        {
            Destroy(puppet.gameObject);
        }
    }

    void TransitionToNextScene()
    {
        UnsubscribeFromEvents();
        FindObjectOfType<FadeTo>().FadeToSetup();
        FindObjectOfType<FadeTo>().finishedIncreasing.AddListener(LoadNextScene);

        FindObjectOfType<SoundController>().PlayButton();
    }

    void LoadNextScene()
    {
        FindObjectOfType<FadeTo>().finishedIncreasing.RemoveListener(LoadNextScene);
        Loader.Load(Loader.Scene.main_menu);
    }

    void SubscribeToEvents()
    {
        FindObjectOfType<Controls>().keyboard_o_down.AddListener(TransitionToNextScene);
        FindObjectOfType<Controls>().keyboard_return_down.AddListener(TransitionToNextScene);
    }

    void UnsubscribeFromEvents()
    {
        FindObjectOfType<Controls>().keyboard_o_down.RemoveListener(TransitionToNextScene);
        FindObjectOfType<Controls>().keyboard_return_down.RemoveListener(TransitionToNextScene);
    }
}
