using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertPopup : PopUp
{
    public Button confirmButton;
    public Text msgText;

    public int closeCount = 1;

    protected override void Awake()
    {
        base.Awake();
        confirmButton.onClick.AddListener(() =>
        {
            PopUpManager.instance.ClosePopUp();
        });
    }

    public override void Open(object data, int closeCount)
    {
        base.Open(closeCount);
        this.closeCount = closeCount;
        msgText.text = (string)data;
    }

    public override void Close()
    {
        base.Close();
        this.closeCount--;
        if(this.closeCount > 0)
        {
            PopUpManager.instance.ClosePopUp();
        }
    }
}
