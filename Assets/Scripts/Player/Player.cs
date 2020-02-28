using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private uint keyDownCounter;
    private uint nextMoveCounter;

    public uint keyDownSpeed = 150;
    public uint nextMoveSpeed = 100;

    // Start is called before the first frame update
    void Start()
    {
        keyDownCounter = 0;
        nextMoveCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        //TODO: SEPARAR EL MOVIMENT EN 1 FUNCIÓ PER CADA DIRECCIÓ (DIAGONALS)

        // moviment simple

        if (Input.GetKeyDown("w"))
        {
            gameObject.GetComponent<Transform>().position += new Vector3(0, 1, 0);
        }
        else if (Input.GetKeyDown("a"))
        {
            gameObject.GetComponent<Transform>().position += new Vector3(-1, 0, 0);
        }
        else if (Input.GetKeyDown("s"))
        {
            gameObject.GetComponent<Transform>().position += new Vector3(0, -1, 0);
        }
        else if (Input.GetKeyDown("d"))
        {
            gameObject.GetComponent<Transform>().position += new Vector3(1, 0, 0);
        }

        // moviment millorat

        keyDownCounter += 1;

        if (keyDownCounter >= keyDownSpeed)
        {
            if (Input.GetKey("w"))
            {
                nextMoveCounter += 1;

                if (nextMoveCounter >= nextMoveSpeed)
                {
                    gameObject.GetComponent<Transform>().position += new Vector3(0, 1, 0);
                    nextMoveCounter = 0;
                }
            }
            else if (Input.GetKey("a"))
            {
                nextMoveCounter += 1;

                if (nextMoveCounter >= nextMoveSpeed)
                {
                    gameObject.GetComponent<Transform>().position += new Vector3(-1, 0, 0);
                    nextMoveCounter = 0;
                }
            }
            else if (Input.GetKey("s"))
            {
                nextMoveCounter += 1;

                if (nextMoveCounter >= nextMoveSpeed)
                {
                    gameObject.GetComponent<Transform>().position += new Vector3(0, -1, 0);
                    nextMoveCounter = 0;
                }
            }
            else if (Input.GetKey("d"))
            {
                nextMoveCounter += 1;

                if (nextMoveCounter >= nextMoveSpeed)
                {
                    gameObject.GetComponent<Transform>().position += new Vector3(1, 0, 0);
                    nextMoveCounter = 0;
                }
            }
        }

        if (Input.GetKeyUp("w"))
        {
            resetCounterParameters();
        }
        else if (Input.GetKeyUp("a"))
        {
            resetCounterParameters();
        }
        else if (Input.GetKeyUp("s"))
        {
            resetCounterParameters();
        }
        else if (Input.GetKeyUp("d"))
        {
            resetCounterParameters();
        }
    }

    void resetCounterParameters()
    {
        keyDownCounter = 0;
        nextMoveCounter = 0;
    }
}
