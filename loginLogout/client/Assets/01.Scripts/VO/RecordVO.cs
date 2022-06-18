using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using UnityEngine;
using System;

[Serializable]
public class RecordVO
{
    public string name;
    public string msg;  //32비트  CPU기준으로 4바이트를 64비트로 8바이트르
    public int score;
    public string user;

    public RecordVO(string name, string msg, string score)
    {
        this.name = name;
        this.msg = msg;
        this.score = int.Parse(score);
    }

    public override string ToString()
    {
        return $"?name={EncodeStr(name)}&msg={EncodeStr(msg)}&score={score}";
    }

    private string EncodeStr(string str)
    {
        return HttpUtility.UrlEncode(str, Encoding.UTF8);
    }
}
