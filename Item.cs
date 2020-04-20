using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Jh_Lib;

// @TODO 코드 양 많아지면 클래스 분리할 것
// class Item 
// class Item_Niddle
// class Item_BombSize
// class Item_BombCount
// class Item_Speed

public class Item : PoolObject
{
    GameObject mSystemManager;

    float mTimer = 0;
    float mSpeed = 0.001f;
    float mOffset = 0.02f;
    protected string mImageRes = "";

    void Start()
    {
        mSystemManager = GameObject.Find("SystemManager");
        GetImageRes();
        LoadImage();
    }

    void Update()
    {
        // Shaking
        mTimer += mOffset;
        mTimer = mTimer % 100;
        float height = Mathf.Sin(mTimer) * mSpeed;
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + height, this.transform.position.z);
    }

    void LoadImage() 
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(mImageRes);
    }

    virtual public void ItemStart(Character charac) 
    { 
        ObjectSleep(); 
        this.gameObject.SetActive(false); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Character>() != null)
        {
            ItemStart(collision.gameObject.GetComponent<Character>());
            mSystemManager.GetComponent<AudioManager>().PlayItemAudio();
        }
    }

    public override void SleepCustome()
    {
        GameObject.Find("SystemManager").GetComponent<ItemFactory>().RemoveItem(TileMap.PosToIndex(transform.position));
    }

    virtual public void GetImageRes()
    {
    }
}

public class Item_Niddle : Item
{

    override public void GetImageRes() 
    {
        mImageRes = "niddle_item";
    }

    override public void ItemStart(Character charc)
    {
        charc.AddNiddle();
        base.ItemStart(charc);
    }
}

public class Item_BombSize : Item
{

    override public void GetImageRes()
    {
        mImageRes = "bomb_size";
    }

    override public void ItemStart(Character charc)
    {
        charc.BombSizeUp();
        base.ItemStart(charc);
    }
}

public class Item_BombLength : Item
{

    override public void GetImageRes()
    {
        mImageRes = "bomb_length";
    }

    override public void ItemStart(Character charc)
    {
        charc.BombLengthUp();
        base.ItemStart(charc);
    }
}

public class Item_Speed : Item
{
    override public void GetImageRes()
    {
        mImageRes = "speed_item";
    }

    override public void ItemStart(Character charc)
    {
        charc.SpeedUp();
        base.ItemStart(charc);
    }
}
