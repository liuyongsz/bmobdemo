using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EquipStrong
{
    public int id;
    public int strongLv ;
    public int shoot ;
    public int pass ;
    public int reel ;
    public int defend ;
    public int trick;
    public int steal ;
    public int control ;
    public int keep ;
    public int cost ;
}
public class EquipStrongConfig : ConfigLoaderBase
{


    private static Dictionary<int, List<EquipStrong>> m_data = new Dictionary<int, List<EquipStrong>>();

    protected override void OnLoad()
    {
        if (!ReadConfig<EquipStrong>("EquipStrong.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(EquipStrong row)
    {
        if (!m_data.ContainsKey(row.id))
        {
            m_data[row.id] = new List<EquipStrong>();
        }

        var entry = m_data[row.id];
        entry.Add(row);
    }

    public static EquipStrong GetEquipStrongInfo(int id,int level)
    {
        if (!m_data.ContainsKey(id))
            return null;

        var enumerator = m_data[id].GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (level == enumerator.Current.strongLv)
                return enumerator.Current;
        }

        return null;

    }
}
