using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public string baseUrl = "http://localhost:54000";

    private string token = "";

    public Button logoutBtn;

    public void SetToken(string token)
    {
        this.token = token;
        PlayerPrefs.SetString("token", token); //토큰을 저장
        UIManager.instance.ShowBox1();
    }
    
    private void Awake()
    {

        if(instance != null)
        {
            Debug.LogError("다수의 NetworkManager가 돌아가고 있습니다.");
        }
        instance = this;
        token = PlayerPrefs.GetString("token", ""); //없으면 null나옴
        logoutBtn.onClick.AddListener(Logout);
    }

    private void Start()
    {
        if(!token.Equals(""))
        {
            UIManager.instance.ShowBox2();
        }
    }

    public void Logout()
    {
        //토큰을 null로 변경하고 PlayerPrefabs에서 token을 지워주고
        token = "";
        PlayerPrefs.DeleteKey("token");
        UIManager.instance.ShowBox1();
        // showBox1을 하면 된다.
    }

    public void SendGetRequest(string url, string queryString, Action<string> callBack)
    {
        StartCoroutine(SendGet($"{baseUrl}/{url}{queryString}", callBack));
    }

    public void SendPostRequest(string url, string payload, Action<string> callBack)
    {
        StartCoroutine( SendPost($"{baseUrl}/{url}", payload, callBack));
    }

    IEnumerator SendGet(string url, Action<string> callBack)
    {
        UnityWebRequest req = UnityWebRequest.Get(url);

        req.SetRequestHeader("authorization", "Bearer " + token);

        yield return req.SendWebRequest();

        if(req.result == UnityWebRequest.Result.Success)
        {
            string data = req.downloadHandler.text;
            callBack(data);
        }
        else
        {
            callBack("{\"result\":false, \"msg\": \"error in communicaion\"}");
        }
    }

    IEnumerator SendPost(string url, string payload, Action<string> callBack)
    {
        UnityWebRequest req = UnityWebRequest.Post(url, payload);
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("authorization", "Bearer " + token);

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(payload);
        req.uploadHandler = new UploadHandlerRaw(jsonToSend);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string data = req.downloadHandler.text;
            callBack(data);
        }
        else
        {
            callBack("{\"result\":false, \"msg\": \"error in communicaion\"}");
        }
    }
}
