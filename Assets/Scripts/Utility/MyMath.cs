using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class MyMath
{
   public static Vector2Int GetCorrespondingFraction(float myDecimal)
    {
        //retorna la fracció generatriu d'un decimal (numerador = Vector2Int.x, denominador = Vector2Int.y)
        Vector2Int ret = new Vector2Int(-1, -1);

        float absDecimal = Mathf.Abs(myDecimal);

        int myInteger = (int)absDecimal;
        float myFloat = absDecimal - myInteger;

        int numberDecimals = GetNumberOfDecimals(myFloat);

        //int numerator = 0;
        //int denominador = 10 ^ numberDecimals;

        return ret;
    }

    public static int GetNumberOfDecimals(float myDecimal)
    {
        //decimal badDecimal = (decimal)myDecimal;

        double doubleVal = (double)myDecimal;
        decimal goodDecimal = (decimal)doubleVal;

        Debug.Log("MyMath::GetNumberOfDecimals - goodDecimal = " + goodDecimal);
        Debug.Log("MyMath::GetNumberOfDecimals - Number of Decimals = " + BitConverter.GetBytes(decimal.GetBits(goodDecimal)[3])[2]);
        return BitConverter.GetBytes(decimal.GetBits(goodDecimal)[3])[2];
    }
}
