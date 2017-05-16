using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GemCompound
{

    public int id;

    public int compoundId;

    public float amount;

}
public class GemCompoundConfig : ConfigLoaderBase
{
    
    private static Dictionary<int, GemCompound> m_data = new Dictionary<int, GemCompound>();

    protected override void OnLoad()
    {
        if (!ReadConfig<GemCompound>("GemCompound.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(GemCompound row)
    {
        m_data[row.id] = row;

    }

    public static GemCompound GetGemCompound(int id)
    {
        if (m_data.ContainsKey(id))
        {
            return m_data[id];
        }
        return null;

    }
}
