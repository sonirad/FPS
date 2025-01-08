using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

public class Test99_HTML : TestBase
{
    readonly string url = "https://atentsexample.azurewebsites.net/monster";
    // readonly string url2 = "https://www.google.com";

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        StartCoroutine(GetData());
    }

    private IEnumerator GetData()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
        }
    }
}
