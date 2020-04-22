using System.Collections;
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

    void Spawn()
    {
        switch (nextSpawn)
        {
            case NextSpawn.CANI:
                nextSpawn = NextSpawn.HIPSTER;

                int crandom = lastCani;
                while (crandom == lastCani)
                    crandom = Random.Range(0, 5);

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
                    hrandom = Random.Range(0, 5);

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
}
