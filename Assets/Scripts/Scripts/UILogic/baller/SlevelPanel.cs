using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class ConSuleItem
{
    public string itemID;
    public Transform trans;
    public UISprite color;
    public UILabel name;
    public UITexture Icon;
    public UILabel count;
}


public class SlevelPanel : TeamMediator
{
    private UISprite slevelBtn;
    private Transform tishi;
    private UISprite helpBtn;
    private List<ConSuleItem> ConSuleList = new List<ConSuleItem>();
    public SlevelPanel(GameObject go)
    {
        helpBtn = UtilTools.GetChild<UISprite>(go.transform, "helpBtn");
        slevelBtn = UtilTools.GetChild<UISprite>(go.transform, "slevelBtn");
        tishi = UtilTools.GetChild<Transform>(go.transform, "tishi");
        UIEventListener.Get(slevelBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(helpBtn.gameObject).onClick = OnClick;
        ConSuleList = UtilTools.SetConSumeItemList(4, go.transform);
    }
    public override void SetChildPanel()
    {
        tishi.gameObject.SetActive(currentTeamBaller.star == clientInfo.maxstar);
        slevelBtn.gameObject.SetActive(!(currentTeamBaller.star == clientInfo.maxstar));
        if (currentTeamBaller.star == clientInfo.maxstar)
            return;
        slevelBtn.gameObject.SetActive(currentTeamBaller.star != clientInfo.maxstar);
        PlayerItem info = PlayerManager.GetPlayerSlevelInfo(currentTeamBaller.star);
        int length = info.material.Split(';').Length;
        for (int i = 0; i < ConSuleList.Count; ++i)
        {
            ConSuleList[i].trans.gameObject.SetActive(length > i);
            if (length > i)
            {
                string itemID = info.material.Split(';')[i].Split(',')[0].ToString();
                int count = UtilTools.IntParse(info.material.Split(';')[i].Split(',')[1]);
                ConSuleList[i].name.text = TextManager.GetItemString(itemID);
                ConSuleList[i].count.text = UtilTools.StringBuilder(ItemManager.GetBagItemCount(itemID), "/", count);
                ItemInfo item = ItemManager.GetItemInfo(itemID);
                if (item == null)
                    return;
                ConSuleList[i].color.spriteName = UtilTools.StringBuilder("color" + item.color);
                LoadSprite.LoaderItem(ConSuleList[i].Icon, itemID);
            }
        }
    }
    void OnClick(GameObject go)
    {
        if (go == helpBtn.gameObject)
        {
            List<object> decsList = new List<object>();
            string text = TextManager.GetPropsString("state10006");
            decsList.Add(text);
            GUIManager.ShowHelpPanel(decsList);
        }
        else if (go == slevelBtn.gameObject)
            ServerCustom.instance.SendClientMethods("onClientUpStar", currentTeamBaller.id);
    }
    public void SlevelSucess()
    {
        tishi.gameObject.SetActive(currentTeamBaller.star == clientInfo.maxstar - 1);
        slevelBtn.gameObject.SetActive(currentTeamBaller.star != clientInfo.maxstar - 1);
        PlayerItem info = PlayerManager.GetPlayerSlevelInfo(currentTeamBaller.star);
        currentTeamBaller.star += 1;
        currentTeamBaller.shoot += info.shoot;
        currentTeamBaller.passBall += info.pass;
        currentTeamBaller.reel += info.reel;
        currentTeamBaller.keep += info.keep;
        currentTeamBaller.controll += info.control;
        currentTeamBaller.defend += info.def;
        currentTeamBaller.trick += info.trick;
        currentTeamBaller.steal += info.steal;
        UtilTools.SetStar(currentTeamBaller.star, teamMediator.stars, clientInfo.maxstar);
        teamMediator.ShowBallerInfo();
        if (currentTeamBaller.star == clientInfo.maxstar)
            return;
        info = PlayerManager.GetPlayerSlevelInfo(currentTeamBaller.star);
        ItemInfo item;
        int length = info.material.Split(';').Length;
        for (int i = 0; i < ConSuleList.Count; ++i)
        {
            ConSuleList[i].trans.gameObject.SetActive(length > i);
            if (length > i)
            {
                string itemID = info.material.Split(';')[i].Split(',')[0].ToString();
                item = ItemManager.GetItemInfo(itemID);
                int count = UtilTools.IntParse(info.material.Split(';')[i].Split(',')[1]);
                info = PlayerManager.GetPlayerSlevelInfo(currentTeamBaller.star);
                if (info != null)
                {
                    itemID = info.material.Split(';')[i].Split(',')[0].ToString();
                    count = UtilTools.IntParse(info.material.Split(';')[i].Split(',')[1]);
                    ConSuleList[i].count.text = UtilTools.StringBuilder(ItemManager.GetBagItemCount(itemID), "/", count);
                    if (ConSuleList[i].Icon.mainTexture==null)
                    {
                        ConSuleList[i].color.spriteName = UtilTools.StringBuilder("color" + item.color);
                        LoadSprite.LoaderItem(ConSuleList[i].Icon, itemID);
                    }                  
                }
            }
        }
    }
}
