using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class MentalityUpItem
{
    public string Name;
    public int Value;
}
public class MentalityPanel : TeamMediator
{
    public UISprite uponce;
    public UISprite upten;
    public UIToggle lowItem;
    public UISprite yesUp;
    public UISprite cancleUp;
    public UITexture lowItemIcon;
    public UISprite lowItemColor;
    public UILabel lowItemName;
    public UILabel lowItemNum;
    public UIToggle highItem;
    public UITexture highItemIcon;
    public UISprite highItemColor;
    public UILabel highItemName;
    public UILabel highItemNum;
    private MentalityMaxInfo info;
    public bool canClick = true;
    public static Dictionary<string, MentalityUpItem> mentalityUpList = new Dictionary<string, MentalityUpItem>();
    public MentalityPanel(GameObject go)
    {
        uponce = UtilTools.GetChild<UISprite>(go.transform, "uponce");
        upten = UtilTools.GetChild<UISprite>(go.transform, "upten");
        yesUp = UtilTools.GetChild<UISprite>(go.transform, "yes");
        cancleUp = UtilTools.GetChild<UISprite>(go.transform, "no");
        lowItem = UtilTools.GetChild<UIToggle>(go.transform, "Item_1");
        highItem = UtilTools.GetChild<UIToggle>(go.transform, "Item_2");
        lowItemIcon = UtilTools.GetChild<UITexture>(go.transform, "Item_1/icon");
        lowItemColor = UtilTools.GetChild<UISprite>(go.transform, "Item_1/color");
        lowItemName = UtilTools.GetChild<UILabel>(go.transform, "Item_1/Name");
        lowItemNum = UtilTools.GetChild<UILabel>(go.transform, "Item_1/num");
        highItemIcon = UtilTools.GetChild<UITexture>(go.transform, "Item_2/icon");
        highItemColor = UtilTools.GetChild<UISprite>(go.transform, "Item_2/color");
        highItemName = UtilTools.GetChild<UILabel>(go.transform, "Item_2/Name");
        highItemNum = UtilTools.GetChild<UILabel>(go.transform, "Item_2/num"); 
        UIEventListener.Get(uponce.gameObject).onClick = OnClick;
        UIEventListener.Get(upten.gameObject).onClick = OnClick;
        UIEventListener.Get(yesUp.gameObject).onClick = OnClick;
        UIEventListener.Get(cancleUp.gameObject).onClick = OnClick;
    }
    public override void SetChildPanel()
    {
        lowItem.value = true;
        info = MentalityMaxConfig.GetMentalityMaxInfo(clientInfo.initialstar);
        string itemID1 = info.material.Split(',')[0];
        string itemID2 = info.material.Split(',')[1];
        lowItemName.text = TextManager.GetItemString(itemID1);
        highItemName.text = TextManager.GetItemString(itemID2);
        lowItemNum.text = ItemManager.GetBagItemCount(info.material.Split(',')[0]).ToString();
        highItemNum.text = ItemManager.GetBagItemCount(info.material.Split(',')[1]).ToString();
        ItemInfo item = ItemManager.GetItemInfo(itemID1);
        lowItemColor.spriteName = UtilTools.StringBuilder("color", item.color);
        LoadSprite.LoaderItem(lowItemIcon, itemID1, false);
        LoadSprite.LoaderItem(highItemIcon, itemID2, false);
        item = ItemManager.GetItemInfo(itemID2);
        highItemColor.spriteName = UtilTools.StringBuilder("color", item.color);       
    }

    void OnClick(GameObject go)
    {
        if (go == uponce.gameObject)
        {
            mentalityUpList.Clear();
            if (lowItem.value)
            {
                if (UtilTools.IntParse(lowItemNum.text) >= 4)
                {
                    ServerCustom.instance.SendClientMethods("onClientUpMentality", currentTeamBaller.id, 0);
                    lowItemNum.text = (UtilTools.IntParse(lowItemNum.text) - 4).ToString();
                }
                else
                {
                    GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_7"), null);
                    return;
                }
            }
            else
            {
                if (UtilTools.IntParse(highItemNum.text) >= 4)
                {
                    ServerCustom.instance.SendClientMethods("onClientUpMentality", currentTeamBaller.id, 1);
                    highItemNum.text = (UtilTools.IntParse(highItemNum.text) - 4).ToString();
                }
                else
                {
                    GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_7"), null);
                    return;
                }
            }
        }
        else if (go == upten.gameObject)
        {
            if (ballerRateyInfo.cloneObj.Count > 0)
            {
                for (int i = 0; i < ballerRateyInfo.cloneObj.Count; ++i)
                {
                    MonoBehaviour.Destroy(ballerRateyInfo.cloneObj[i]);
                }
                ballerRateyInfo.cloneObj.Clear();
            }
            if (!canClick)
                return;
            canClick = false;
            mentalityUpList.Clear();
            if (lowItem.value)
            {
                if (UtilTools.IntParse(lowItemNum.text) / 40 >= 1)
                {
                    ServerCustom.instance.SendClientMethods("onClientUpTenMentality", currentTeamBaller.id, 0);
                    lowItemNum.text = (UtilTools.IntParse(lowItemNum.text) - 40).ToString();
                }
                else
                {
                    GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_7"), null);
                    canClick = true;
                    return;
                }
            }
            else
            {
                if (UtilTools.IntParse(highItemNum.text) / 40 >= 1)
                {
                    ServerCustom.instance.SendClientMethods("onClientUpTenMentality", currentTeamBaller.id, 1);
                    highItemNum.text = (UtilTools.IntParse(highItemNum.text) - 40).ToString();
                }
                else
                {
                    GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_7"), null);
                    canClick = true;
                    return;
                }
            }
        
        }
        else if (go == yesUp.gameObject)
        {
            if (ballerRateyInfo.cloneObj.Count > 0)
            {
                for (int i = 0; i < ballerRateyInfo.cloneObj.Count; ++i)
                {
                    MonoBehaviour.Destroy(ballerRateyInfo.cloneObj[i]);
                }
                ballerRateyInfo.cloneObj.Clear();
            }             
            ballerRateyInfo.hasDownValue = true;
            ServerCustom.instance.SendClientMethods("UpDateMentalityInfo", currentTeamBaller.id);           
            mentalityPanel.yesUp.gameObject.SetActive(false);
            mentalityPanel.cancleUp.gameObject.SetActive(false);
            mentalityPanel.uponce.gameObject.SetActive(true);
        }
        else if (go == cancleUp.gameObject)
        {
            if (ballerRateyInfo.cloneObj.Count > 0)
            {
                for (int i = 0; i < ballerRateyInfo.cloneObj.Count; ++i)
                {
                    MonoBehaviour.Destroy(ballerRateyInfo.cloneObj[i]);
                }
                ballerRateyInfo.cloneObj.Clear();
            }
            yesUp.gameObject.SetActive(false);
            cancleUp.gameObject.SetActive(false);
            uponce.gameObject.SetActive(true);
            mentalityUpList.Clear();
            ballerRateyInfo.hasDownValue = false;
        }
    }
}
