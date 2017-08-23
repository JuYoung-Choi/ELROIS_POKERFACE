using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMgr : Singleton<CharacterMgr>
{ 
    public List<AvatarObj> characterMgr = new List<AvatarObj>();

    private void Awake()
    {
        for (int index = 0; index < characterMgr.Count; ++index)
        {
            characterMgr[index].Initialize(transform);
        }
    }

    public bool PushToPool(string itemName, GameObject item, Transform parent = null)
    {
        AvatarObj pool = GetPoolItem(itemName);
        if (pool == null)
            return false;

        pool.PushToAvatar(item, parent == null ? transform : parent);
        return true;
    }

    public GameObject PopFromPool(string itemName, Transform parent = null)
    {
        AvatarObj pool = GetPoolItem(itemName);
        if (pool == null)
            return null;

        return pool.PopFromAvatar(parent);
    }

    AvatarObj GetPoolItem(string itemName)
    {
        for (int index = 0; index < characterMgr.Count; ++index)
        {
            if (characterMgr[index].AvatarName.Equals(itemName))
                return characterMgr[index];
        }

        return null;
    }

}
