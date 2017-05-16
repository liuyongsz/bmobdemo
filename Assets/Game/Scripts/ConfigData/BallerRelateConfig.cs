using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BallerRelate
{
    /// <summary>
    /// 球员ID
    ///-------------------------------------------------------
    /// </summary>
    public int id;

    /// <summary>
    /// </summary>
    public string name;

    /// <summary>
    /// 羁绊球员
    ///-------------------------------------------------------
    /// </summary>
    public string relate;

    /// <summary>
    /// 属性
    ///-------------------------------------------------------
    /// </summary>
    public string prop1;

    /// <summary>
    /// 属性
    ///-------------------------------------------------------
    /// </summary>
    public string prop2;

    /// <summary>
    /// 属性
    ///-------------------------------------------------------
    /// </summary>
    public string prop3;

    /// <summary>
    /// 属性
    ///-------------------------------------------------------
    /// </summary>
    public string prop4;

    public string relationName;


}

//阵型球员位置
public class BallerRelateConfig : ConfigLoaderBase
{
    
    private static Dictionary<int, BallerRelate> m_data = new Dictionary<int, BallerRelate>();

    protected override void OnLoad()
    {
        if (!ReadConfig<BallerRelate>("BallerRelate.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(BallerRelate row)
    {
        m_data[row.id] = row;
    }

    public static BallerRelate GetBallerRelate(int id)
    {
        if (m_data.ContainsKey(id))
        {
            return m_data[id];
        }
        return null;

    }

    /// <summary>
    /// 获取羁绊信息
    /// </summary>
    /// <param name="configId"></param>
    /// <returns></returns>
    public static List<RelationBallerInfo> GetBallerRelateInfo(string configId)
    {
        List<RelationBallerInfo> rela_list = new List<RelationBallerInfo>();
       BallerRelate info = GetBallerRelate(int.Parse(configId));

        RelationBallerInfo data = null;
        string[] arr = info.relate.Split(';');

        string[] nameArr = info.relationName.Split(',');


        for (int i = 0; i < arr.Length; i++)
        {
            data = new RelationBallerInfo();
            data.playerId = int.Parse(configId);
            data.relationName = nameArr[i];

            string[] content = arr[i].Split(',');
            bool isActive = true;
            for (int j = 0; j < content.Length; j++)
            {
                int id = int.Parse(content[j]);
                if (!data.ballerList.Contains(id) && id != 0)
                    data.ballerList.Add(id);

                if (isActive)
                    isActive = IsBallerInteamOrBench(id.ToString());

                string propStr = "prop" + (j + 1);
                string propContent = GameConvert.StringConvert(info.GetType().GetField(propStr).GetValue(info));


                data.propName = propContent.Split(':')[0];

                data.propValue =GameConvert.IntConvert( propContent.Split(':')[1]);

            }

            data.isActive = isActive;
            rela_list.Add(data);
        }


        return rela_list;

    }


    //是否上阵和替补
    public static  bool IsBallerInteamOrBench(string configId)
    {
        var card_enumerator = TeamMediator.teamList.Values.GetEnumerator();
        while (card_enumerator.MoveNext())
        {
            if (card_enumerator.Current.configId == configId)
            {
                if (card_enumerator.Current.inTeam == 1 || card_enumerator.Current.bench == 1)
                    return true;
            }

        }
        return false;

    }


}

//球员羁绊信息
public class RelationBallerInfo
{
    public int playerId;//configID

    public List<int> ballerList = new List<int>();

    public string relationName; // 羁绊名称

    public bool isActive;//是否激活

    public string propName;

    public int propValue;

}
