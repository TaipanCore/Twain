using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebLoader : MonoBehaviour
{
    public void LoadSaveUsingWebRequest(String fullSaveFilePath, Action<String> DeserializeMethod)
    {
        StartCoroutine(WebRequest(fullSaveFilePath, DeserializeMethod));
    }
    private IEnumerator WebRequest(String fullSaveFilePath, Action<String> DeserializeMethod)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(fullSaveFilePath))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
                DeserializeMethod.Invoke(request.downloadHandler.text);
            else
                Debug.LogError("Failed to load default save: " + request.error);
        }
    }
    
}
