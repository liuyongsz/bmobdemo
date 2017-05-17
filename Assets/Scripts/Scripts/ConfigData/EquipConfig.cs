using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class EquipInfo
{
    public int id;
    public int type;
    public int star;
    public int maxStar;
    public int maxStrongLevel;
    public int position;
    public int shoot;
    public int pass;
    public int reel;
    public int defend;
    public int trick;
    public int steal;
    public int control ;
    public int keep;
    public int suit;
    public string needOpenHold;
}
public class EquipConfig : ConfigLoaderBase
{
    //球员装备信息
    public static Dictionary<int, List<EquipItemInfo>> m_player_eqiup = new Dictionary<int, List<EquipItemInfo>>();

    private static  Dictionary<int, EquipInfo> EquipList = new Dictionary<int, EquipInfo>();

    protected override void OnLoad()
    {
        if (!ReadConfig<EquipInfo>("Equip.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        EquipList.Clear();
    }

    private void OnReadRow(EquipInfo row)
    {
        EquipList[row.id] = row;
    }

    public static EquipInfo GetEquipInfo(int id)
    {
        if (EquipList.ContainsKey(id))
        {
            return EquipList[id];
        }
        return null;
    }
    /// <summary>
    /// 判断是否拥有装备
    /// </summary>
    /// <returns></returns>
    public static bool IsHasEquip()
    {
        var enumerator = m_player_eqiup.Values.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Count > 0)
                return true;
        }
        return false;
    }
    /// <summary>
    /// 获取有装备列表的玩家id
    /// </summary>
    /// <returns></returns>
    public static List<int> GetHasEquipPlayerList()
    {
        List<int> list = new List<int>();
        var enumerator = m_player_eqiup.Keys.GetEnumerator();
        int card_id = 0;
        while (enumerator.MoveNext())
        {
            card_id = enumerator.Current;
            if (m_player_eqiup[card_id].Count > 0)
                if (!list.Contains(card_id))
                    list.Add(card_id);
        }


        return list;
    }

    /// <summary>
    /// 通过装备位置获取装备数据
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public static List<EquipItemInfo> GetEquipDataListByPos(int pos)
    {
        List<EquipItemInfo> list = GetEquipDataListByPlayerID(0);
        if (pos == 0)
            return list;

        List<EquipItemInfo> equip_list = new List<EquipItemInfo>();
        for (int i = 0; i < list.Count; i++)
        {
            EquipInfo info = GetEquipInfo(int.Parse(list[i].itemID));
            if (info.position == pos)
                equip_list.Add(list[i]);
        }

        return equip_list;
    }
    /// <summary>
    /// 获取背包球员装备列表
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static List<EquipItemInfo> GetEquipDataListByPlayerID(int id)
    {
        List<EquipItemInfo> list = new List<EquipItemInfo>();
        if (EquipConfig.m_player_eqiup.ContainsKey(id))
             list = EquipConfig.m_player_eqiup[id];
        return list;
    }
    /// <summary>
    /// 通过uuid 获取背包装备信息
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    public static EquipItemInfo GetEquipDataByUUID(string uuid)
    {
        List<EquipItemInfo> list = GetEquipDataListByPlayerID(0);
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].uuid == uuid)
                return list[i];
        }

        return null;
    }
    /// <summary>
    /// 选择背包大于强化等级
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static List<EquipItemInfo> GetEquipDataListByLv(int level)
    {
        List<EquipItemInfo> list =GetEquipDataListByPlayerID(0);
        
        List<EquipItemInfo> equip_list = new List<EquipItemInfo>();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].strongLevel > level)
                equip_list.Add(list[i]);
        }

        return equip_list;
    }
    ///刷新装备数据
    public static bool RefreshEquipData(int player_id,EquipItemInfo data)
    {
        List<EquipItemInfo> list =GetEquipDataListByPlayerID(player_id);

        for (int i = 0; i < list.Count; i++)
        {
            if (player_id == 0 && list[i].uuid == data.uuid || player_id > 0 && list[i].itemID == data.itemID)
            {
                list[i] = data;
                return true;
            }

        }

        return false;

    }
    /// <summary>
    /// 获取属性增加数据
    /// type ==0 升星 ,==1 升级, ==-1 装备当前属性值
    /// </summary>
    /// <returns></returns>

    public static List<EquipAddInfo> GetPropAddDataListByID(int type,int equip_id,int star,int strong)
    {

        EquipAddInfo info = null;
        string[] prop_arr = Define.EquipPropStr.Split(',');

        EquipInfo equip_info= GetEquipInfo(equip_id);
        int next_star_num = star;
        int next_strong_lv = strong;

        if (type == 0)
            next_star_num = star == equip_info.maxStar ? equip_info.maxStar : star + 1;

        if (type == 1)
            next_strong_lv = strong >= equip_info.maxStrongLevel ? equip_info.maxStrongLevel : strong + 1;
        
        List<EquipAddInfo> list = new List<EquipAddInfo>();

        for (int i = 0; i < prop_arr.Length; i++)
        {

            info = new EquipAddInfo();
            
            string prop_str = prop_arr[i];
            int prop_value = equip_info == null ? 0 : GameConvert.IntConvert(equip_info.GetType().GetField(prop_str).GetValue(equip_info));
            if (prop_value == 0)
                continue;

            // 获取星级属性
            EquipStar star_info = EquipStarConfig.GetEquipStarInfo(equip_id, star);
            int prop_star_value = star_info == null ? 0 : GameConvert.IntConvert(star_info.GetType().GetField(prop_str).GetValue(star_info));

            EquipStar star_next_info = EquipStarConfig.GetEquipStarInfo(equip_id, next_star_num);
            int prop_next_star_value = star_next_info == null ? 0 : GameConvert.IntConvert(star_next_info.GetType().GetField(prop_str).GetValue(star_next_info));
            
            //获取强化
            int prop_strong_value = 0;
            int next_prop_star_value = 0;
            EquipStrong strong_info = EquipStrongConfig.GetEquipStrongInfo(equip_info.star, strong);
            EquipStrong next_strong_info = EquipStrongConfig.GetEquipStrongInfo(equip_info.star, next_strong_lv);

            if (strong_info != null)
                prop_strong_value = GameConvert.IntConvert(strong_info.GetType().GetField(prop_str).GetValue(strong_info));

            if (next_strong_info != null)
                next_prop_star_value = GameConvert.IntConvert(next_strong_info.GetType().GetField(prop_str).GetValue(next_strong_info));


            info.prop_name = prop_str;
            info.prop_base_value = prop_value;
            info.prop_star_value = prop_star_value;
            info.prop_next_star_value = prop_next_star_value;
            info.prop_strong_value = prop_strong_value;
            info.prop_next_strong_value = next_prop_star_value;

            list.Add(info);
        }
        return list;
    }


}
