using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TeamFormationSystem
{
    /// <summary>
    /// 阵型系统id
    ///-------------------------------------------------------
    /// </summary>
    public int id;

    /// <summary>
    /// 名字
    ///-------------------------------------------------------
    /// </summary>
    public string name;

    /// <summary>
    /// 激活条件（阵型id:等级;）
    ///-------------------------------------------------------
    /// </summary>
    public string activeCondition;

    /// <summary>
    /// 属性类型
    ///-------------------------------------------------------
    /// </summary>
    public string valueType;

    /// <summary>
    /// 属性加成（属性名:数值;）
    ///-------------------------------------------------------
    /// </summary>
    public string addProp;

    /// <summary>
    /// 材料（id:num;）
    ///-------------------------------------------------------
    /// </summary>
    public string material;

    public int needLevel;

    public int maxStrongLevel;

}

//阵型球员位置
public class TeamFormationSystemConfig : ConfigLoaderBase
{
    
    public static Dictionary<int, TeamFormationSystem> m_data = new Dictionary<int, TeamFormationSystem>();

    public static Dictionary<int, FormationSysInfo> m_formation_sys = new Dictionary<int, FormationSysInfo>();

    protected override void OnLoad()
    {
        if (!ReadConfig<TeamFormationSystem>("TeamFormationSystem.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(TeamFormationSystem row)
    {
        m_data[row.id] = row;
    }

    public static TeamFormationSystem GetTeamFormationSystem(int id)
    {
        if (m_data.ContainsKey(id))
        {
            return m_data[id];
        }
        return null;

    }
    //获取阵型系统id list
    public static List<int> GetTeamFSId()
    {
        List<int> list = new List<int>();
        var enumerator = m_data.Keys.GetEnumerator();
        while(enumerator.MoveNext())
        {
            list.Add(enumerator.Current);
        }

        return list;
   
    }

    //获取阵型系列信息
    public static FormationSysInfo GetFormationSys(int id)
    {
        
        if (m_formation_sys.ContainsKey(id))
            return m_formation_sys[id];
        return null;
    }
}

//阵型系统数据
public  class FormationSysInfo
{
    public int id;
    public int strongLevel; //强化等级
    public int open;
    public int active;
}
