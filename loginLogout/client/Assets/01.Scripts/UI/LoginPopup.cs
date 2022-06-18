using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPopup : PopUp
{
    public Button loginButton;
    public Button closeButton;

    public InputField idInput;
    public InputField passInput;

    public RectTransform menuPanel;

    protected override void Awake()
    {
        base.Awake();

        menuPanel = GameObject.Find("OuterPanel").GetComponent<RectTransform>();

        closeButton.onClick.AddListener(() =>
        {
            PopUpManager.instance.ClosePopUp();
        });

        loginButton.onClick.AddListener(() =>
        {
            //여기서 id와 pass에 올바르지 않은문자나 공백이 있는 지 검사해야해
            LoginVO vo = new LoginVO(idInput.text, passInput.text);
            string payload = JsonUtility.ToJson(vo);
            NetworkManager.instance.SendPostRequest("login", payload, res =>
            {
                Debug.Log(res);
                ResponseVO vo = JsonUtility.FromJson<ResponseVO>(res);
                if(vo.result)
                {
                    PopUpManager.instance.OpenPopUp("alert", "로그인 성공", 2);
                    NetworkManager.instance.SetToken(vo.payload); //토큰 저장
                }
                else
                {
                    PopUpManager.instance.OpenPopUp("alert", vo.payload, 1);
                }
            });
        });
    }
}
