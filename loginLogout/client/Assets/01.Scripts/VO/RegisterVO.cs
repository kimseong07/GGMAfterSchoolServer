using System;

[Serializable]
public class RegisterVO
{
    public string name;
    public string id;
    public string password;

    public RegisterVO(string name, string id, string password)
    {
        this.name = name;
        this.id = id;
        this.password = password;
    }
}
