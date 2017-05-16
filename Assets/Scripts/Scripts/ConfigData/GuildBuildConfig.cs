using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//公会权限
public class GuildBuild
{
    public int id;

    /// <summary>
    /// 等级
    ///-------------------------------------------------------
    /// </summary>
    public int level;

    /// <summary>
    /// 最大等级
    ///-------------------------------------------------------
    /// </summary>
    public int maxLevel;

}
public class GuildBuildConfig : ConfigLoaderBase
{

    private static Dictionary<int, GuildBuild> m_data = new Dictionary<int, GuildBuild>();

    public static Dictionary<int, GuildBuildInfo> mGuildBuildDict = new Dictionary<int, GuildBuildInfo>();
    
    protected override void OnLoad()
    {
        if (!ReadConfig<GuildBuild>("GuildBuild.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(GuildBuild row)
    {
        m_data.Add(row.id, row) ;
        
    }
    public static GuildBuild GetGuildBuild(int id)
    {
        if (m_data.ContainsKey(id))
        {
            return m_data[id];
        }
        return null;
    }

    //获取公会建筑信息
    public static GuildBuildInfo GetGuildBuildInfo(int id)
    {
        if(mGuildBuildDict.ContainsKey(id))
        {
            return mGuildBuildDict[id];
        }
        GuildBuildInfo info = new GuildBuildInfo();
        mGuildBuildDict.Add(id, info);
        return info;
    }

    public static List<GuildBuildInfo> GetGuildBuildList()
    {
        List<GuildBuildInfo> list = new List<GuildBuildInfo>();

        var enumor = mGuildBuildDict.Values.GetEnumerator();
        while(enumor.MoveNext())
        {
            list.Add(enumor.Current);
        }
        list.Sort(CompareBuild);
        return list;
    }
    private static int CompareBuild(GuildBuildInfo info1, GuildBuildInfo info2)
    {
        if (info1.id < info2.id)
            return -1;
        else if (info1.id > info2.id)
            return 1;
        else
            return 0;
    }
}
