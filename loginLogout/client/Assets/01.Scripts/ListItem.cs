using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListItem : MonoBehaviour
{
    public Image image;
    public Text txtName;

    public Button btnLoad;

    private void Awake()
    {
        btnLoad = GetComponent<Button>();
    }

    public void SetData(string filename, Sprite sprite)
    {
        txtName.text = filename;
        image.sprite = sprite;
    }
}
