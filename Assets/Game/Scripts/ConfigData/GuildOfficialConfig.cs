using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//公会职位
public class GuildOfficial
{
    /// <summary>
    /// id(职位ID)
    ///-------------------------------------------------------
    /// </summary>
    public int id;

    public string name;
    /// <summary>
    /// 拥有的权限
    ///-------------------------------------------------------
    /// </summary>
    public string powerOpen;

    /// <summary>
    /// 人数限制
    ///-------------------------------------------------------
    /// </summary>
    public int num;

}
public class GuildOfficialConfig : ConfigLoaderBase
{


    private static Dictionary<int, GuildOfficial> m_data = new Dictionary<int, GuildOfficial>();

    //同阵营公会列表
    public static List<GuildInfo> mGuildList = new List<GuildInfo>();
    //已经申请过的公会ID
    public static List<int> mApplyGuildIdList = new List<int>();

    protected override void OnLoad()
    {
        if (!ReadConfig<GuildOfficial>("GuildOfficial.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(GuildOfficial row)
    {
        m_data.Add(row.id, row) ;
        
    }
    public static GuildOfficial GetGuildOfficial(int id)
    {
        if (m_data.ContainsKey(id))
        {
            return m_data[id];
        }
        return null;
    }

    //获取公会职位列表
    public static List<GuildOfficial> GetGuildOfficalList()
    {
        List<GuildOfficial> list = new List<GuildOfficial>();

        var enumor = m_data.Values.GetEnumerator();
        while(enumor.MoveNext())
        {
            list.Add(enumor.Current);
        }

        return list;
    }

}
