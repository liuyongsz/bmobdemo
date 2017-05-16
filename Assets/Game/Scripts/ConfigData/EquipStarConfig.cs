using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EquipStar
{
    public int id;
    public int star;
    public int shoot;
    public int pass;
    public int reel;
    public int defend;
    public int trick;
    public int steal;
    public int control;
    public int keep;
    public string cost;
}
public class EquipStarConfig : ConfigLoaderBase
{


    private static Dictionary<int, List<EquipStar>> m_data = new Dictionary<int, List<EquipStar>>();

    protected override void OnLoad()
    {
        if (!ReadConfig<EquipStar>("EquipStar.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(EquipStar row)
    {
        if (!m_data.ContainsKey(row.id))
        {
            m_data[row.id] = new List<EquipStar>();
        }

        var entry = m_data[row.id];
        entry.Add(row);
    }

    public static EquipStar GetEquipStarInfo(int id,int star)
    {
        if (!m_data.ContainsKey(id))
            return null;

        var enumerator = m_data[id].GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (star == enumerator.Current.star)
                return enumerator.Current;
        }

        return null;
    }

}
