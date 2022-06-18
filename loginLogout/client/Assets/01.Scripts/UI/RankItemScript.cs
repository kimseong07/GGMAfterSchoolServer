using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class RankItemScript : MonoBehaviour
{
    public Text rankText;
    public Text nameText;
    public Text scoreText;
    public Text msgText;

    public void SetData(int rank, RecordVO vo)
    {
        rankText.text = rank.ToString();
        nameText.text = vo.name;
        scoreText.text = vo.score.ToString();
        msgText.text = vo.msg;
    }

    public void SetAnimation(float delay)
    {
        transform.localScale = new Vector3(1, 0, 1);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(delay);
        seq.Append(transform.DOScaleY(1, 0.4f));
    }
}
