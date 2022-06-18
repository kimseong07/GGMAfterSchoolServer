using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SimpleImageGet : MonoBehaviour
{
    public Button btnLoad;
    public Image loadedImage;

    private readonly string url = "http://localhost:54000/image";

    private void Start()
    {
        btnLoad.onClick.AddListener(
            ()=>StartCoroutine(ImageRequest()) );
    }

    IEnumerator ImageRequest()
    {
        string path = Directory.GetCurrentDirectory()
                        + "/Assets/gondr.jpg";
        
        if(File.Exists(path))
        {
            Texture2D t = new Texture2D(1,1);
            byte[] dataBytes = File.ReadAllBytes(path);
            t.LoadImage(dataBytes);
            Rect rect = new Rect(0, 0, t.width, t.height);
            loadedImage.sprite 
                = Sprite.Create(t, rect, new Vector2(0.5f, 0.5f));
            yield break;
        }

        UnityWebRequest req = 
            UnityWebRequestTexture.GetTexture(url);
        
        yield return req.SendWebRequest();

        if(req.result == UnityWebRequest.Result.Success)
        {
            Texture2D t =
                ((DownloadHandlerTexture)(req.downloadHandler)).texture;

            Rect rect = new Rect(0, 0, t.width, t.height);
            Sprite s = Sprite.Create(t, rect, new Vector2(0.5f, 0.5f));
            loadedImage.sprite = s;

            byte[] byteData = t.EncodeToJPG();
            File.WriteAllBytes(path, byteData);
        }
        else
        {
            Debug.LogError("데이터통신중 오류 발생");
        }
    }
}
