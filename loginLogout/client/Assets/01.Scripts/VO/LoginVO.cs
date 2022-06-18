using System;

[Serializable]
public class LoginVO
{
    public string id;
    public string password;

    public LoginVO(string id, string password)
    {
        this.id = id;
        this.password = password;
    }
}
