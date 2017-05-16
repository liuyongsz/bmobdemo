using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class BagSize
{
    public int ID;
    public int maxSize;
    public int needDimaond;
}

public class BagSizeConfig : ConfigLoaderBase
{
    public static Dictionary<int, BagSize> list = new Dictionary<int, BagSize>();

    protected override void OnLoad()
    {
        if (!ReadConfig<BagSize>("BagSize.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        list.Clear();
    }

    private void OnReadRow(BagSize row)
    {
        list[row.ID] = row;
    }


    public static BagSize GetBagSize(int id)
    {
        if (list.ContainsKey(id))
        {
            return list[id];
        }
        return null;
    }

}
