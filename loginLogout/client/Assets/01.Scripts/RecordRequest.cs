using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RecordRequest : MonoBehaviour
{
    public InputField txtName;
    public InputField txtMsg;
    public InputField txtScore;

    public Button btnSend;

    private readonly string url = "http://localhost:54000/record";

    private void Start()
    {
        btnSend.onClick.AddListener(() =>
        {
            StartCoroutine(GetSend());
        });
    }

    IEnumerator GetSend()
    {

        RecordVO vo = new RecordVO(txtName.text, txtMsg.text, txtScore.text);
        UnityWebRequest req = UnityWebRequest.Get(url + vo);
        yield return req.SendWebRequest();

        if(req.result == UnityWebRequest.Result.Success)
        {
            string data = req.downloadHandler.text;
            Debug.Log(data);
        }
        else
        {
            Debug.LogError("데이터 통신중 오류 발생");
        }
    }
}
