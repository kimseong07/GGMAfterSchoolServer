using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankListRefresh : MonoBehaviour
{
    public Button refreshBtn;
    public Transform contentView;
    public RankItemScript rankItemPrefab; //랭크 아이템 프리팹

    private void Start()
    {
        refreshBtn.onClick.AddListener(() =>
        {
            NetworkManager.instance.SendGetRequest("list", "", ListGenerate);
        });
    }

    private void ListGenerate(string json)
    {
        //Debug.Log(json);
        //1. json을 받을 자료구조 (즉 VO형태를 만들어야해)
        //2. jsonutility를 이용해서 json을 vo로 파싱해
        ResponseVO res = JsonUtility.FromJson<ResponseVO>(json);

        Debug.Log(json);
        if (res.result)
        {
            
            RecordListVO vo = JsonUtility.FromJson<RecordListVO>(res.payload);
            for (int i = contentView.childCount - 1; i >= 0; i--)
            {
                Destroy(contentView.GetChild(i).gameObject);
            }
            
            for (int i = 0; i < vo.list.Count; i++)
            {
                RankItemScript ris = Instantiate(rankItemPrefab, contentView);
                ris.SetData(i + 1, vo.list[i]);
                ris.SetAnimation(i * 0.4f + 0.1f);
            }
        }
        else
        {
            Debug.Log(res.payload);
        }

        

        //3. 파싱한 vo에서 list를 for문을 돌면서 Instantiate 해서 contentView의 자식으로 넣어야 해
        //  단 이작업을 하기전에 contentView의 모든 자식을 삭제해야 한다. 
        //  Destroy 와 Instantiate를 써서 랭크가 표시되도록 한다.

        
    }
}
