using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BenchBox
{
    /// <summary>
    /// 格子id
    ///-------------------------------------------------------
    /// </summary>
    public int id;

    /// <summary>
    /// 开启消耗
    ///-------------------------------------------------------
    /// </summary>
    public int needmoney;
    
}

//阵型球员位置
public class BenchBoxConfig : ConfigLoaderBase
{
    
    private static Dictionary<int, BenchBox> m_data = new Dictionary<int, BenchBox>();

    protected override void OnLoad()
    {
        if (!ReadConfig<BenchBox>("BenchBox.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(BenchBox row)
    {
        m_data[row.id] = row;
    }

    public static BenchBox GetBenchBox(int id)
    {
        if (m_data.ContainsKey(id))
        {
            return m_data[id];
        }
        return null;

    }
}
