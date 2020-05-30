using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTextManager : MonoBehaviour
{
    //prefab
    public GameObject sprite;
    public enum VerticalAnchor { COLLAPSE, CENTER };

    [Header("Parameters")]
    public VerticalAnchor verticalAnchor;
    public float spacing;
    public int length;
    
    [Header("Characters")]
    public Sprite A;
    public Sprite B;
    public Sprite C;
    public Sprite D;
    public Sprite E;
    public Sprite F;
    public Sprite G;
    public Sprite H;
    public Sprite I;
    public Sprite J;
    public Sprite K;
    public Sprite L;
    public Sprite M;
    public Sprite N;
    public Sprite O;
    public Sprite P;
    public Sprite Q;
    public Sprite R;
    public Sprite S;
    public Sprite T;
    public Sprite U;
    public Sprite V;
    public Sprite W;
    public Sprite X;
    public Sprite Y;
    public Sprite Z;
    public Sprite a;
    public Sprite b;
    public Sprite c;
    public Sprite d;
    public Sprite e;
    public Sprite f;
    public Sprite g;
    public Sprite h;
    public Sprite i;
    public Sprite j;
    public Sprite k;
    public Sprite l;
    public Sprite m;
    public Sprite n;
    public Sprite o;
    public Sprite p;
    public Sprite q;
    public Sprite r;
    public Sprite s;
    public Sprite t;
    public Sprite u;
    public Sprite v;
    public Sprite w;
    public Sprite x;
    public Sprite y;
    public Sprite z;
    public Sprite question;
    public Sprite exclamation;
    public Sprite dot;
    public Sprite coma;
    public Sprite dots;
    public Sprite at;
    public Sprite slash;
    public Sprite score;

    //català i castellà
    public Sprite a_acc_obert;
    public Sprite a_acc_tancat;
    public Sprite e_acc_obert;
    public Sprite e_acc_tancat;
    public Sprite i_acc;
    public Sprite o_acc_obert;
    public Sprite o_acc_tancat;
    public Sprite u_acc;
    public Sprite geminada;
    public Sprite c_trencada;
    public Sprite enya;
    public Sprite apostrof;

    bool needToUpdate;

    // Start is called before the first frame update
    void Start()
    {
        int textCount = 0;
        int extraLines = 0;

        foreach (Transform text in transform)
        {
            if (length == 0)
            {
                CreateText(text.gameObject);
                text.transform.position -= new Vector3(0, spacing * textCount++, 0);
            }
            else
            {
                extraLines += CreateRestrictedText(text.gameObject);
                text.transform.position -= new Vector3(0, spacing * textCount++, 0);

                textCount += extraLines; extraLines = 0; //espai extra per les linies fetes amb salt de linia corresponents a un sol objecte myText
            }
        }

        if (verticalAnchor == VerticalAnchor.CENTER)
        {
            float textsCenter = GetTextVerticalCenter(textCount);
            float offset = Mathf.Abs(textsCenter - transform.position.y);

            foreach (Transform text in transform)
            {
                text.position += new Vector3(0, offset, 0);
            }
        }
    }

    void Update()
    {
        if (needToUpdate)
            UpdatePositionForLeftText();
    }

    string GetText(GameObject myText)
    {
        if (myText.GetComponent<MyText>().text.Contains("#"))
        {
            return FindObjectOfType<JSONHandler>().RetrieveText(myText.GetComponent<MyText>().text);
        }
        else
        {
            return myText.GetComponent<MyText>().text;
        }
    }

    void CreateText(GameObject myText)
    {
        string text = GetText(myText);
        string[] characters = new string[text.Length];

        for (int i = 0; i < text.Length; i++)
        {
            characters[i] = text[i].ToString();
        }

        float accumulatedOffset = 0f;

        foreach (string character in characters)
        {
            Vector3 nextPos = myText.transform.position + new Vector3(accumulatedOffset, 0, 0);

            GameObject nextChar = Instantiate(sprite, nextPos, Quaternion.identity);
            nextChar.transform.parent = myText.transform;
            nextChar.name = character;
            nextChar.GetComponent<SpriteRenderer>().sprite = GetSprite(character);
            nextChar.GetComponent<SpriteRenderer>().color = myText.GetComponent<MyText>().color;
            nextChar.GetComponent<SpriteRenderer>().sortingOrder = myText.GetComponent<MyText>().layer;

            //preparar offset pel seguent character
            if (nextChar.GetComponent<SpriteRenderer>().sprite != null)
                accumulatedOffset += nextChar.GetComponent<SpriteRenderer>().sprite.rect.width / 16f;

            accumulatedOffset += GetOffset(character);
        }

        switch (myText.GetComponent<MyText>().anchor)
        {
            case MyText.Anchor.CENTERED:
                Vector3 positionDifference = GetTextCenter(myText) - myText.transform.position;
                myText.transform.position -= positionDifference;
                break;
        }
    }

    int CreateRestrictedText(GameObject myText) //retorna el nombre de línies que ha ocupat el text
    {
        int ret = 0;

        string text = GetText(myText);
        Debug.Log("MyTextManager::CreateRestrictedText - Successfully retrieved text = " + text);

        string[] characters = new string[text.Length];

        for (int i = 0; i < text.Length; i++)
        {
            characters[i] = text[i].ToString();
        }

        float accumulatedOffset = 0f;
        List<GameObject> currentWord = new List<GameObject>(); //saber quina paraula estem escrivint per fer el salt de línia incloent-la

        foreach (string character in characters)
        {
            if (character != " ")
            {
                if (accumulatedOffset >= length)
                {
                    accumulatedOffset = 0.0f;
                    ret++;

                    foreach (GameObject letter in currentWord)
                    {
                        letter.transform.position = myText.transform.position + new Vector3(accumulatedOffset, -(spacing * ret), 0);

                        if (letter.GetComponent<SpriteRenderer>().sprite != null)
                            accumulatedOffset += letter.GetComponent<SpriteRenderer>().sprite.rect.width / 16f;

                        accumulatedOffset += GetOffset(letter.name);
                    }
                }
            }
            else if (character == " ")
            {
                currentWord = new List<GameObject>();
            }

            Vector3 nextPos = myText.transform.position + new Vector3(accumulatedOffset, (-spacing * ret), 0);

            GameObject nextChar = Instantiate(sprite, nextPos, Quaternion.identity);
            nextChar.transform.parent = myText.transform;
            nextChar.name = character;
            nextChar.GetComponent<SpriteRenderer>().sprite = GetSprite(character);
            nextChar.GetComponent<SpriteRenderer>().color = myText.GetComponent<MyText>().color;
            nextChar.GetComponent<SpriteRenderer>().sortingOrder = myText.GetComponent<MyText>().layer;

            if (character != " ")
                currentWord.Add(nextChar);

            //preparar offset pel seguent character
            if (nextChar.GetComponent<SpriteRenderer>().sprite != null)
                accumulatedOffset += nextChar.GetComponent<SpriteRenderer>().sprite.rect.width / 16f;

            accumulatedOffset += GetOffset(character);
        }

        return ret;
    }

    Sprite GetSprite(string character)
    {
        Sprite ret = null;

        switch (character)
        {
            case "A":
                ret = A;
                break;

            case "B":
                ret = B;
                break;

            case "C":
                ret = C;
                break;

            case "D":
                ret = D;
                break;

            case "E":
                ret = E;
                break;

            case "F":
                ret = F;
                break;

            case "G":
                ret = G;
                break;

            case "H":
                ret = H;
                break;

            case "I":
                ret = I;
                break;

            case "J":
                ret = J;
                break;

            case "K":
                ret = K;
                break;

            case "L":
                ret = L;
                break;

            case "M":
                ret = M;
                break;

            case "N":
                ret = N;
                break;

            case "O":
                ret = O;
                break;

            case "P":
                ret = P;
                break;

            case "Q":
                ret = Q;
                break;

            case "R":
                ret = R;
                break;

            case "S":
                ret = S;
                break;

            case "T":
                ret = T;
                break;

            case "U":
                ret = U;
                break;

            case "V":
                ret = V;
                break;

            case "W":
                ret = W;
                break;

            case "X":
                ret = X;
                break;

            case "Y":
                ret = Y;
                break;

            case "Z":
                ret = Z;
                break;

            case "a":
                ret = a;
                break;

            case "b":
                ret = b;
                break;

            case "c":
                ret = c;
                break;

            case "d":
                ret = d;
                break;

            case "e":
                ret = e;
                break;

            case "f":
                ret = f;
                break;

            case "g":
                ret = g;
                break;

            case "h":
                ret = h;
                break;

            case "i":
                ret = i;
                break;

            case "j":
                ret = j;
                break;

            case "k":
                ret = k;
                break;

            case "l":
                ret = l;
                break;

            case "m":
                ret = m;
                break;

            case "n":
                ret = n;
                break;

            case "o":
                ret = o;
                break;

            case "p":
                ret = p;
                break;

            case "q":
                ret = q;
                break;

            case "r":
                ret = r;
                break;

            case "s":
                ret = s;
                break;

            case "t":
                ret = t;
                break;

            case "u":
                ret = u;
                break;

            case "v":
                ret = v;
                break;

            case "w":
                ret = w;
                break;

            case "x":
                ret = x;
                break;

            case "y":
                ret = y;
                break;

            case "z":
                ret = z;
                break;

            case "?":
                ret = question;
                break;

            case "!":
                ret = exclamation;
                break;

            case ".":
                ret = dot;
                break;

            case ",":
                ret = coma;
                break;

            case ":":
                ret = dots;
                break;

            case "@":
                ret = at;
                break;

            case "/":
                ret = slash;
                break;

            case "-":
                ret = score;
                break;

            case "à":
                ret = a_acc_obert;
                break;

            case "á":
                ret = a_acc_tancat;
                break;

            case "è":
                ret = e_acc_obert;
                break;

            case "é":
                ret = e_acc_tancat;
                break;

            case "í":
                ret = i_acc;
                break;

            case "ò":
                ret = o_acc_obert;
                break;

            case "ó":
                ret = o_acc_tancat;
                break;

            case "ú":
                ret = u_acc;
                break;

            case "·":
                ret = geminada;
                break;

            case "ç":
                ret = c_trencada;
                break;

            case "ñ":
                ret = enya;
                break;

            case "'":
                ret = apostrof;
                break;
        }

        return ret;
    }

    float GetOffset(string character)
    {
        float ret = 1 / 16f;

        switch (character)
        {
            case "?":
                ret = 3 / 16f;
                break;

            case "!":
                ret = 3 / 16f;
                break;

            case ".":
                ret = 3 / 16f;
                break;

            case ",":
                ret = 3 / 16f;
                break;

            case ":":
                ret = 3 / 16f;
                break;

            case " ":
                ret = 3 / 16f;
                break;
        }

        return ret;
    }

    Vector3 GetTextCenter(GameObject myText)
    {
        int charCount = myText.transform.childCount;
        if (charCount > 1)
        {
            Vector3 lastPosition = myText.transform.GetChild(charCount - 1).transform.position;
            lastPosition += new Vector3(myText.transform.GetChild(charCount - 1).GetComponent<SpriteRenderer>().sprite.rect.width / 16f, 0, 0);

            Vector3 center = myText.transform.position + (lastPosition - myText.transform.position) / 2f;

            return center;
        }

        return myText.transform.position;
    }

    float GetTextVerticalCenter(int count)
    {
        if (count % 2 == 0) //count és parell
        {
            int middleTextIndex = count / 2 - 1;

            GameObject targetText = null;

            int iterator = 0;
            foreach (Transform text in transform)
            {
                if (iterator++ == middleTextIndex)
                    targetText = text.gameObject;
            }

            float jump = (spacing * 16f - 9f) / 16f;
            return targetText.transform.position.y - 9 / 16f - jump / 2; // 9 és l'alçada de les lletres
        }
        else // count és imparell
        {
            int middleTextIndex = count / 2;

            GameObject targetText = null;

            int iterator = 0;
            foreach (Transform text in transform)
            {
                if (iterator++ == middleTextIndex)
                    targetText = text.gameObject;
            }

            return targetText.transform.position.y - (4.5f / 16f); // 9 és l'alçada de les lletres
        }
    }

    public void SetNewText(string text, Color color, MyText.Anchor anchor, int layer)
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        GameObject newText = new GameObject();
        newText.AddComponent<MyText>();
        newText.GetComponent<MyText>().text = text;
        newText.GetComponent<MyText>().color = color;
        newText.GetComponent<MyText>().anchor = anchor;
        newText.GetComponent<MyText>().layer = layer;

        newText.transform.SetParent(transform);

        CreateText(newText);

        needToUpdate = true;
    }

    public void UpdatePositionForLeftText()
    {
        int counter = 0;
        foreach(Transform text in transform)
        {
            text.transform.position = new Vector3(transform.position.x, transform.position.y - spacing * counter++, 0);
        }
    }
}
