using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
    public static void Shuffle<T>(T[] source)
    {
        for (int i = source.Length - 1; i > -1; i--)
        {
            int index = Random.Range(0, i);

            (source[index], source[i]) = (source[i], source[index]);
        }
    }
}
