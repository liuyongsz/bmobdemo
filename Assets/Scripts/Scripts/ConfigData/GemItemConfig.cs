using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GemItem
{
    public int id;
    //增加属性
    public string propType;
    //属性增加值
    public float propValue;
    //等级
    public int level;

}
public class GemItemConfig : ConfigLoaderBase
{
    
    private static Dictionary<int, GemItem> m_data = new Dictionary<int, GemItem>();

    protected override void OnLoad()
    {
        if (!ReadConfig<GemItem>("GemItem.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(GemItem row)
    {
        m_data[row.id] = row;

    }

    public static GemItem GetGemItem(int id)
    {
        if (m_data.ContainsKey(id))
        {
            return m_data[id];
        }
        return null;

    }
}
