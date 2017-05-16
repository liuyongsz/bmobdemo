using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Bench
{
    /// <summary>
    /// id
    ///-------------------------------------------------------
    /// </summary>
    public int id;

    /// <summary>
    /// 最大数量
    ///-------------------------------------------------------
    /// </summary>
    public int maxNum;

    /// <summary>
    /// 默认开启数量
    ///-------------------------------------------------------
    /// </summary>
    public int defaultOpen;

}

//阵型球员位置
public class BenchConfig : ConfigLoaderBase
{
    
    private static Dictionary<int, Bench> m_data = new Dictionary<int, Bench>();

    protected override void OnLoad()
    {
        if (!ReadConfig<Bench>("Bench.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(Bench row)
    {
        m_data[row.id] = row;
    }

    public static Bench GetBench(int id)
    {
        if (m_data.ContainsKey(id))
        {
            return m_data[id];
        }
        return null;

    }
}
