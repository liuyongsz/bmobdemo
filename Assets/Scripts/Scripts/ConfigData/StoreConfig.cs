using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class StoreConfig : ConfigLoaderBase
{
    public static DictionaryEx<int, StoreInfo> storeList = new DictionaryEx<int, StoreInfo>();

    protected override void OnLoad()
    {
        if (!ReadConfig<StoreInfo>("Store.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        storeList.Clear();
    }

    private void OnReadRow(StoreInfo row)
    {
        storeList[row.id] = row;
    }

    public static StoreInfo GetStoreInfo(int id)
    {
        if (storeList.ContainsKey(id))
        {
            return storeList[id];
        }
        return null;
    }
}
