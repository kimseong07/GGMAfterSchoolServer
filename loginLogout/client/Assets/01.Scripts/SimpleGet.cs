using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SimpleGet : MonoBehaviour
{
    public Button btnGet;

    private readonly string url = "http://localhost:54000/hello";


    private void Start()
    {    
        btnGet.onClick.AddListener(
            ( ) => StartCoroutine(GetRequest()) ); 
    }

    IEnumerator GetRequest()
    {
        UnityWebRequest req = UnityWebRequest.Get(url);
        
        yield return req.SendWebRequest();
        // 200 , 404, 
        if(req.result == UnityWebRequest.Result.Success)
        {
            string data = req.downloadHandler.text;
            Debug.Log(data);
            //HelloVO vo = JsonUtility.FromJson<HelloVO>(data);

            //Debug.Log($"Name : {vo.name} , msg : {vo.msg}");
            //foreach(HobbyVO h in vo.hobbies)
            //{
            //    Debug.Log(h.name + ", " + h.id);
            //}
        }   
        else
        {
            Debug.LogError("데이터 통신중 오류 발생");
        }
    }


}
