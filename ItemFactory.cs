using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jh_Lib;

public class ItemFactory : ObjectPool<ItemFactory, Item>
{
    public GameObject Item;

    float mPossiblity = Const.ItemDropPossiblity;
    Dictionary<int, Item> mItemMap = new Dictionary<int, Item>();
    
    int mItemIndex = 0; 

    public void SetItem(Vector3 pos)
    {
        // 아이템 드랍 여부
        bool drop_item = CheckSettingItemPossiblity();
        if (!drop_item) return;

        // @PoolObject Push
        GameObject item = Push(Item);
        item.transform.position = pos;
        item.SetActive(true);

        int index = TileMap.PosToIndex(pos);
        mItemMap[index] = item.GetComponent<Item>();

        // 아이템 타입을 잠시 고정
        mItemIndex += 1;

        // 어떤 아이템 드랍하는지
        int item_num = (mItemIndex) % 4; // niddle, speed, bombsize, bombcount

        switch (item_num)
        {
            case 0:
                item.gameObject.AddComponent<Item_Niddle>();
                break;
            case 1:
                item.gameObject.AddComponent<Item_BombSize>();
                break;
            case 2:
                item.gameObject.AddComponent<Item_BombLength>();
                break;
            case 3:
                item.gameObject.AddComponent<Item_Speed>();
                break;
        }
        
    }

    // 아이템 고정을 위해 random 잠시 제거
    bool CheckSettingItemPossiblity()
    {
        //float rate = Random.Range(0.0f, 1.0f); //0.0f ~ 0.9~f
        return true; // rate < mPossiblity;
    }

    public void RemoveItem(int index)
    {
        if (!mItemMap.ContainsKey(index)) return;

        mItemMap[index] = null;
    }

    public Item GetItembMap(int index)
    {
        if (!mItemMap.ContainsKey(index)) return null;

        return mItemMap[index];
    }
}
