using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
/// <summary>
/// 自己写的toggle组建，不会往外发送事件。
/// </summary>
public class MyUIToggle : MonoBehaviour {
    public UISprite backGroundSprite = null;//背景图
    public bool IsCheckChange = false;//是否让组建自动控制toggle的值
    
    private bool value;
    public bool Value
    {
        get { return value; }
        set
        {
            this.value = value;
            UpdataValue();
        }
    }

    private void UpdataValue()
    {
        //TweenAlpha.Begin(backGroundSprite.gameObject, 0.15f, IsCheckChange ? 1f : 0f);
        if(value)
            backGroundSprite.color = new Color(backGroundSprite.color.r, backGroundSprite.color.g, backGroundSprite.color.b, 1);
        else
            backGroundSprite.color = new Color(backGroundSprite.color.r, backGroundSprite.color.g, backGroundSprite.color.b, 0);
        //改变所有的toggle
        if (IsCheckChange && null != transform.parent && value)
        {
            MyUIToggleParent parent = transform.parent.GetComponent<MyUIToggleParent>();
            if (null == parent)
            {
                parent = transform.parent.gameObject.AddComponent<MyUIToggleParent>();
            }
            parent.InitChildList();

            //其余的设置成false
            Dictionary<GameObject, MyUIToggleItem> dic = parent.GetMyList();
            IEnumerator<GameObject> enumerator = dic.Keys.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (enumerator.Current != gameObject)
                {
                    dic[enumerator.Current].Toggle.Value = !value;
                }
            }
        }
    }
}

public class MyUIToggleParent : MonoBehaviour
{
    private Dictionary<GameObject, MyUIToggleItem> m_list;

    void OnDestroy()
    {
        m_list.Clear();
        m_list = null;
    }

    public void InitChildList()
    {
        if (null == gameObject)
            return;
        if(null == m_list)
            m_list = new Dictionary<GameObject, MyUIToggleItem>();

        m_list.Clear();
        for (int idx = 0; idx < transform.childCount; idx++)
        {
            AddMyList(transform.GetChild(idx).gameObject);
        }
    }
    public void AddMyList(GameObject go)
    {
        MyUIToggle ui = go.GetComponent<MyUIToggle>();
        if (null != ui)
        {
            if(!m_list.ContainsKey(go))
            {
                MyUIToggleItem item = new MyUIToggleItem();
                item.Item = go;
                item.Toggle = ui;
                m_list.Add(go, item);
            }
        }
    }

    public Dictionary<GameObject, MyUIToggleItem> GetMyList()
    {
        return m_list;
    }

    public MyUIToggle GetChildToggle(GameObject go)
    {
        if (m_list.ContainsKey(go))
            return m_list[go].Toggle;

        return null;
    }
}

public class MyUIToggleItem
{
    public GameObject Item { get; set; }
    public MyUIToggle Toggle { get; set; }
}
