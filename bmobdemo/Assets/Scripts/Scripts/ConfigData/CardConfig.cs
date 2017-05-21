using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CardInfo
{
    public int id;
    public int exp;
    public int money;
}
public class CardConfig : ConfigLoaderBase
{
    private static  Dictionary<int, CardInfo> CardList = new Dictionary<int, CardInfo>();

    protected override void OnLoad()
    {
        if (!ReadConfig<CardInfo>("Card.xml", OnReadRow))
            return;
    }
    protected override void OnUnload()
    {
        CardList.Clear();
    }

    private void OnReadRow(CardInfo row)
    {
        CardList[row.id] = row;
    }

    public static CardInfo GetCardInfo(int id)
    {
        if (CardList.ContainsKey(id))
        {
            return CardList[id];
        }
        return null;
    }
}
