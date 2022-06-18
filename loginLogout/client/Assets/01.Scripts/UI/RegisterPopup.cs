using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPopup : PopUp
{
    public Button registerBtn;
    public Button closeBtn;

    public InputField nameInput;
    public InputField idInput;
    public InputField passInput;
    public InputField passConfirmInput;

    protected override void Awake()
    {
        base.Awake();
        registerBtn.onClick.AddListener(() =>
        {
            Regex reg = new Regex(@"^[°¡-ÆRa-zA-Z]{2,3}$");
            
            //   @"^[0-9]{3}-[0-9]{3,4}-[0-9]{4}$";
            if (!reg.IsMatch(nameInput.text))
            {
                Debug.Log("ÀÌ¸§Àº ¹Ýµå½Ã ÇÑ±Û ¶Ç´Â ¿µ¹®À¸·Î 2~3±ÛÀÚ¿©¾ß ÇÕ´Ï´Ù.");
                PopUpManager.instance.OpenPopUp(
                    "alert", "ÀÌ¸§Àº ¹Ýµå½Ã ÇÑ±Û ¶Ç´Â ¿µ¹®À¸·Î 2~3±ÛÀÚ¿©¾ß ÇÕ´Ï´Ù.");
                return;
            }
            
            //ºóÄ­ÀÌ ÀÖ´Â°¡?
            
            //passInput°ú passConfirm ÀÌ ´Ù¸£¸é Debug.Error ¶ç¿ìµµ·Ï ÇÏ°í => popupÀ¸·Î ±³Ã¼

            RegisterVO vo = new RegisterVO(nameInput.text, idInput.text, passInput.text);
            string json = JsonUtility.ToJson(vo);
            NetworkManager.instance.SendPostRequest("register", json, result =>
            {
                //ResponseVO ÇüÅÂ·Î result¸¦ ÆÄ½ÌÇØ¼­
                // ±× °á°ú°¡ true¶ó¸é ¾ó·µÀ» Å¬¸¯½Ã È¸¿ø°¡ÀÔÃ¢µµ °°ÀÌ ´ÝÈ÷°í
                // false¶ó¸é ¾ó·µ Å¬¸¯½Ã ¾ó·µ¸¸ ´ÝÈ÷°Ô
                ResponseVO vo = JsonUtility.FromJson<ResponseVO>(result);
                if (vo.result)
                {
                    PopUpManager.instance.OpenPopUp("alert", vo.payload, 2);
                }
                else
                {
                    PopUpManager.instance.OpenPopUp("alert", vo.payload);
                }
            });
        });

        closeBtn.onClick.AddListener(() => PopUpManager.instance.ClosePopUp());
    }
}
