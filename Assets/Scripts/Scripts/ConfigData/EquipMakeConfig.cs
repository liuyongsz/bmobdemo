
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EquipMakeInfo
{
    public int ID;
    public int Position;
    public string Cost;
    public int money;

}

public class EquipMakeConfig : ConfigLoaderBase
{
    private static  Dictionary<int, EquipMakeInfo> m_data = new Dictionary<int, EquipMakeInfo>();

    protected override void OnLoad()
    {
        if (!ReadConfig<EquipMakeInfo>("EquipMake.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(EquipMakeInfo row)
    {
        m_data[row.ID] = row;
    }

    public static EquipMakeInfo GetCardInfo(int id)
    {
        if (m_data.ContainsKey(id))
        {
            return m_data[id];
        }
        return null;
    }

    /// <summary>
    /// 通过位置获取制作装备
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static List<object> GetEquipListByPos(int pos)
    {
        List<object> list = new List<object>();
        var enumerator = m_data.Values.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (pos == enumerator.Current.Position)
            {
                list.Add(enumerator.Current);
            }
        }

        return list;
    }

}
