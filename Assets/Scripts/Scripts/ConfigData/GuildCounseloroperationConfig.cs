using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuildCounseloroperation
{
    /// <summary>
    /// 操作类型
    ///-------------------------------------------------------
    /// </summary>
    public int id;

    /// <summary>
    /// </summary>
    public string name;

    /// <summary>
    /// 成功率
    ///-------------------------------------------------------
    /// </summary>
    public int successrate;

    /// <summary>
    /// 需要道具
    ///-------------------------------------------------------
    /// </summary>
    public string consumeitem;

    /// <summary>
    /// 消耗钻石
    ///-------------------------------------------------------
    /// </summary>
    public int consumediamond;

    /// <summary>
    /// 消耗欧元
    ///-------------------------------------------------------
    /// </summary>
    public int consumeEuro;

    /// <summary>
    /// 消耗公会资金
    ///-------------------------------------------------------
    /// </summary>
    public int consumefund;

    /// <summary>
    /// 获得贡献
    ///-------------------------------------------------------
    /// </summary>
    public int contribute;

    /// <summary>
    /// 增加己方友好度
    ///-------------------------------------------------------
    /// </summary>
    public int addamity;

    /// <summary>
    /// 减少对方友好度
    ///-------------------------------------------------------
    /// </summary>
    public int subamity;

}


public class GuildCounseloroperationConfig : ConfigLoaderBase
{

    private static Dictionary<int, GuildCounseloroperation> m_data = new Dictionary<int, GuildCounseloroperation>();


    protected override void OnLoad()
    {
        if (!ReadConfig<GuildCounseloroperation>("GuildCounseloroperation.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(GuildCounseloroperation row)
    {
        m_data.Add(row.id, row);

    }
    public static GuildCounseloroperation GetGuildCounseloroperation(int id)
    {
        if (m_data.ContainsKey(id))
        {
            return m_data[id];
        }
        return null;
    }
}
