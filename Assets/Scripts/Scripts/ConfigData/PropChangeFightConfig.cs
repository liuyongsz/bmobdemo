using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PropFightInfo
{
    public int id;
    public string Property;
    public int IsPercent;
    public float ChangeFightPower;
  

}
public class PropChangeFightConfig : ConfigLoaderBase
{

    private static  Dictionary<int, PropFightInfo> prop_list = new Dictionary<int, PropFightInfo>();

    protected override void OnLoad()
    {
        if (!ReadConfig<PropFightInfo>("PropChangeFgiht.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        prop_list.Clear();
    }

    private void OnReadRow(PropFightInfo row)
    {
        prop_list[row.id] = row;
    }

    public static PropFightInfo GetEquipInfo(string prop_name)
    {
        var enumerator = prop_list.Values.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (prop_name == enumerator.Current.Property)
            {
                return enumerator.Current;
            }
        }
        return null;
    }
    public static float GetPropForFightValue(string prop_name, float prop_value)
    {
        float fight = 0;
        
        PropFightInfo info = GetEquipInfo(prop_name);
        if (info == null)
            return 0;

        fight = info.IsPercent == 0 ? prop_value * info.ChangeFightPower : prop_value * info.ChangeFightPower / 100;

        return fight;

    }
}
