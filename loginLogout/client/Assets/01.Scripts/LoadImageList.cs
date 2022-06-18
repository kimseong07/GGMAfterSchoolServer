using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadImageList : MonoBehaviour
{
    public Button loadBtn;
    public Transform listContent; //리스트를 넣을 
    public GameObject listUIPrefab;

    public Image imageField;

    private readonly string url = "http://localhost:54000/fileList";


    void Start()
    {
        loadBtn.onClick.AddListener(() =>
        {
            StartCoroutine(LoadList());
        });  
    }

    IEnumerator LoadList()
    {
        UnityWebRequest req = UnityWebRequest.Get(url);

        yield return req.SendWebRequest();
        if(req.result == UnityWebRequest.Result.Success)
        {
            string json = req.downloadHandler.text;
            ImageListVO vo = JsonUtility.FromJson<ImageListVO>(json);

            Debug.Log(vo.msg);

            Transform[] childs = listContent.GetComponentsInChildren<Transform>();
            for(int i = 1; i < childs.Length; i++)
            {
                Destroy(childs[i].gameObject);
            }
            foreach(string file in vo.list)
            {
                GameObject prefab = Instantiate(listUIPrefab, listContent);
                ListItem li = prefab.GetComponent<ListItem>();


                LoadThumbnail(li, file);
                //li.SetData(file);
            }
            //vo.list.ForEach(x => Debug.Log(x));
        }
    }

       

    private void LoadThumbnail(ListItem li, string filename)
    {
        
        string url = $"http://localhost:54000/thumb?file=r_{filename}";

        Action<Sprite> a = s => li.SetData(filename, s);

        StartCoroutine( GetImageFromServer( url, a ) );

        li.btnLoad.onClick.AddListener(() =>
        {
            string origin = $"http://localhost:54000/image?file={filename}";
            StartCoroutine(GetImageFromServer(origin, s => imageField.sprite = s));
        });
    }

    

    IEnumerator GetImageFromServer(string url, Action<Sprite> handler)
    {
        UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Texture2D t =
                ((DownloadHandlerTexture)(req.downloadHandler)).texture;

            Rect rect = new Rect(0, 0, t.width, t.height);
            Sprite s = Sprite.Create(t, rect, new Vector2(0.5f, 0.5f));

            handler(s);
        }
    }
}
