using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AvatarObj
{
    public string AvatarName = string.Empty; //객체 검색
    public GameObject prefab = null; //저장할 프리펩
    public int AvatarCount = 0; //초기화할때 생성한 객체 수

    //객체를 저장할 리스트
    [SerializeField]
    private List<GameObject> AvatarList = new List<GameObject>();
    
    public void Initialize(Transform parent = null)
    {
        for (int index = 0; index < AvatarCount; ++index)
        {
            AvatarList.Add(CreateItem(parent));
        }
    }

    public void PushToAvatar(GameObject item, Transform parent = null)
    {
        item.transform.SetParent(parent);
        item.SetActive(false);
        AvatarList.Add(item);
    }

    public GameObject PopFromAvatar(Transform parent = null)
    {
        if (AvatarList.Count == 0)
            AvatarList.Add(CreateItem(parent));

        GameObject item = AvatarList[0];
        AvatarList.RemoveAt(0);

        return item;
    }

    private GameObject CreateItem(Transform parent = null)
    {
        GameObject item = Object.Instantiate(prefab) as GameObject;
        item.name = AvatarName;
        item.transform.SetParent(parent);
        item.transform.Rotate(new Vector3(0, 1, 0), 180);
        item.transform.localPosition = Vector3.zero;
        item.SetActive(false);

        return item;
    }
}