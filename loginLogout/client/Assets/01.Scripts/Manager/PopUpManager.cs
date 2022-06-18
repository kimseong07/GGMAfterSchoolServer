using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    public Button registerPopupBtn;
    public Button loginPopupBtn;
    public Transform popupParent;
    
    private CanvasGroup popupCanvasGroup;

    public RegisterPopup registerPopupPrefab;
    public AlertPopup alertPopupPrefab;
    public LoginPopup loginPopupPrefab;

    private Dictionary<string, PopUp> popupDic = new Dictionary<string, PopUp>();
    private Stack<PopUp> popupStack = new Stack<PopUp>();

    public static PopUpManager instance;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("다수의 팝업매니저가 실행중입니다.");
        }
        instance = this;
    }

    void Start()
    {
        popupCanvasGroup = popupParent.GetComponent<CanvasGroup>();
        if(popupCanvasGroup == null)
        {
            popupCanvasGroup = popupParent.gameObject.AddComponent<CanvasGroup>();
        }
        popupCanvasGroup.alpha = 0;
        popupCanvasGroup.interactable = false;
        popupCanvasGroup.blocksRaycasts = false;

        popupDic.Add("register", Instantiate(registerPopupPrefab, popupParent));
        popupDic.Add("login", Instantiate(loginPopupPrefab, popupParent));
        popupDic.Add("alert", Instantiate(alertPopupPrefab, popupParent));

        registerPopupBtn.onClick.AddListener(() => OpenPopUp("register"));
        loginPopupBtn.onClick.AddListener(() => OpenPopUp("login"));
    }

    public void OpenPopUp(string name, object data = null, int closeCount = 1)
    {
        if(popupStack.Count == 0) //최초로 열리는 팝업
        {
            popupCanvasGroup.interactable = true;
            DOTween.To(
                () =>popupCanvasGroup.alpha, 
                value => popupCanvasGroup.alpha = value,
                1, 
                0.8f
            ).OnComplete(() =>
            {
                popupCanvasGroup.blocksRaycasts = true;
            });
        }

        popupStack.Push(popupDic[name]);
        popupDic[name].Open(data, closeCount);
    }

    public void ClosePopUp()
    {
        popupStack.Pop().Close();
        if(popupStack.Count == 0)
        {
            DOTween.To(
                () => popupCanvasGroup.alpha,
                value => popupCanvasGroup.alpha = value,
                0,
                0.8f
            ).OnComplete(() =>
            {
                popupCanvasGroup.interactable = false;
                popupCanvasGroup.blocksRaycasts = false;
            });
        }
    }
}
