using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TeamFormation
{
    /// <summary>
    /// id
    ///-------------------------------------------------------
    /// </summary>
    public int id;

    /// <summary>
    /// 名字
    ///-------------------------------------------------------
    /// </summary>
    public string name;

    /// <summary>
    /// 阵型系统类型
    ///-------------------------------------------------------
    /// </summary>
    public int type;

    /// <summary>
    /// 球员位置
    ///-------------------------------------------------------
    /// </summary>
    public string playerPos;

    /// <summary>
    /// 上阶阵型id
    ///-------------------------------------------------------
    /// </summary>
    public int lastId;

    /// <summary>
    /// 下阶阵型id
    ///-------------------------------------------------------
    /// </summary>
    public int nextId;

    /// <summary>
    /// 解锁等级
    ///-------------------------------------------------------
    /// </summary>
    public int unlockLevel;

    /// <summary>
    /// </summary>
    public string position;

}

//阵型球员位置
public class TeamFormationConfig : ConfigLoaderBase
{
    
    private static Dictionary<int, TeamFormation> m_data = new Dictionary<int, TeamFormation>();
    public static List<int> m_activeId_list = new List<int>();//解锁阵型ID List

    //阵型属性加成
    public static List<AddProp> mForamtionProp = new List<AddProp>();
    //球员羁绊属性加成
    public static Dictionary<int, List<AddProp>> mRelatProp = new Dictionary<int, List<AddProp>>();


    protected override void OnLoad()
    {
        if (!ReadConfig<TeamFormation>("TeamFormation.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(TeamFormation row)
    {
        m_data[row.id] = row;
    }

    public static TeamFormation GetTeamFormation(int id)
    {
        if (m_data.ContainsKey(id))
        {
            return m_data[id];
        }
        return null;

    }

    //通过过类型获取 阵型list
    public static List<TeamFormation>GetFormationByType(int type)
    {

        List<TeamFormation> list = new List<TeamFormation>();
        var enumerator = m_data.Values.GetEnumerator();
        while(enumerator.MoveNext())
        {
            if(enumerator.Current.type == type)
            {
                list.Add(enumerator.Current);
            }
        }
        return list;
    }

    //通过属性名获取加成属性
    public static AddProp GetFormationAddProp(string name)
    {

        for(int i=0; i<mForamtionProp.Count; i++)
        {
            AddProp info = mForamtionProp[i];
            if (info.propName == name)
                return info;
        }

        return new AddProp();
    }
    //获取上阵属性增加
    public static int GetFormationPropValue(string name)
    {
        for (int i = 0; i < mForamtionProp.Count; i++)
        {
            AddProp info = mForamtionProp[i];
            if (info.propName == name)
                return info.value;
        }

        return 0;
    }
    //获取球员羁绊属性
    public static int GetRelatePropValue(int cardID, string name)
    {
        int value = 0;
        if (mRelatProp.ContainsKey(cardID))
        {
            List<AddProp> list = mRelatProp[cardID];
            for (int i = 0; i < list.Count; i++)
            {
                AddProp info = list[i];
                if (info.propName == name)
                    value += info.value;
            }
        }
        return value;
    }
    ///获取羁绊球员的属性
    public static AddProp GetRelateAddProp(int cardID,string name)
    {
        if (mRelatProp.ContainsKey(cardID))
        {
            List<AddProp> list = mRelatProp[cardID];
            for (int i = 0; i < list.Count; i++)
            {
                AddProp info = list[i];
                if (info.propName == name)
                    return info;
            }
        }

        return new AddProp();
        
    }

    //球员羁绊属性清理
    public static void RelateAdddPropClear()
    {
        mRelatProp.Clear();
    }
}
