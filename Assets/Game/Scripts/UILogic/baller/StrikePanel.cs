using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StrikePanel : TeamMediator
{
    private UISprite strikeInfoBtn;
    private UISprite strikeBtn;
    private UISprite switchBtn;
    private UISprite helpBtn;
    private UISprite closeBtn;
    private UISprite subtractBtn;
    private UISprite minBtn;
    private UISprite addBtn;
    private UISprite maxBtn;
    private UISprite okBtn;
    private UISprite cancleBtn;
    private UISprite di1;
    private UISprite di2;
    private UISprite di3;
    private UITexture icon;
    private UITexture changeIcon;
    private UITexture chooseSuipian;
    private int pieceCount;
    private UILabel needCount;
    private UILabel Count;
    private UILabel chaneCount;
    private UILabel neddMoney;
    private UILabel pieceNum;
    private UIGrid ChooseClipGrid;   
    private UIGrid cloneGrid;
    private Transform scroView;
    private Transform switchClip;
    private Transform tishi;
    private static BallerPiece ballerInfo;
    public StrikePanel(GameObject go)
    {
        switchClip = UtilTools.GetChild<Transform>(go.transform, "switchClip");
        scroView = UtilTools.GetChild<Transform>(go.transform, "switchClip/ScrollView");
        tishi = UtilTools.GetChild<Transform>(go.transform, "tishi");
        strikeInfoBtn = UtilTools.GetChild<UISprite>(go.transform, "strikeUI/strikeInfoBtn");
        strikeBtn = UtilTools.GetChild<UISprite>(go.transform, "strikeUI/strikeBtn");
        icon = UtilTools.GetChild<UITexture>(go.transform, "strikeUI/material");
        di1 = UtilTools.GetChild<UISprite>(go.transform, "strikeUI/di1");
        di2 = UtilTools.GetChild<UISprite>(go.transform, "ChageUI/di2");
        di3 = UtilTools.GetChild<UISprite>(go.transform, "ChageUI/di3");
        changeIcon = UtilTools.GetChild<UITexture>(go.transform, "ChageUI/material");
        switchBtn = UtilTools.GetChild<UISprite>(go.transform, "ChageUI/switchBtn");
        helpBtn = UtilTools.GetChild<UISprite>(go.transform, "ChageUI/helpBtn");
        closeBtn = UtilTools.GetChild<UISprite>(go.transform, "switchClip/closeBtn");
        minBtn = UtilTools.GetChild<UISprite>(go.transform, "switchClip/minBtn");
        maxBtn = UtilTools.GetChild<UISprite>(go.transform, "switchClip/maxBtn");
        addBtn = UtilTools.GetChild<UISprite>(go.transform, "switchClip/addBtn");
        cancleBtn = UtilTools.GetChild<UISprite>(go.transform, "switchClip/cancleBtn");
        subtractBtn = UtilTools.GetChild<UISprite>(go.transform, "switchClip/subtractBtn");
        okBtn = UtilTools.GetChild<UISprite>(go.transform, "switchClip/okBtn");
        chooseSuipian = UtilTools.GetChild<UITexture>(go.transform, "ChageUI/chooseSuipian");
        needCount = UtilTools.GetChild<UILabel>(go.transform, "strikeUI/needCount");
        neddMoney = UtilTools.GetChild<UILabel>(go.transform, "ChageUI/money");
        Count = UtilTools.GetChild<UILabel>(go.transform, "ChageUI/Count");
        chaneCount = UtilTools.GetChild<UILabel>(go.transform, "ChageUI/chaneCount");
        pieceNum = UtilTools.GetChild<UILabel>(go.transform, "switchClip/pieceNum");
        ChooseClipGrid = UtilTools.GetChild<UIGrid>(go.transform, "switchClip/ScrollView/ChooseClipGrid");
        UIEventListener.Get(strikeInfoBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(strikeBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(switchBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(helpBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(chooseSuipian.gameObject).onClick = OnClick;
        UIEventListener.Get(closeBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(cancleBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(minBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(maxBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(okBtn.gameObject).onClick = OnClick;
        LongClickEvent.Get(addBtn.gameObject).onPress = OnPress;
        LongClickEvent.Get(subtractBtn.gameObject).onPress = OnPress;
    }
    public override void SetChildPanel()
    {
        if (currentTeamBaller.configId.Contains("600") || currentTeamBaller.brokenLayer == 20)
        {
            tishi.gameObject.SetActive(true);
            return;
        }
        else
            tishi.gameObject.SetActive(false);        
        pieceNum.text = "1";
        neddMoney.text = "0";
        Count.text = "0";
        chaneCount.gameObject.SetActive(false);
        ballerInfo = null;        
        string itemID = ItemManager.GetMaterialID(currentTeamBaller.configId);
        LoadSprite.LoaderImage(chooseSuipian, "jiahao", false);
        LoadSprite.LoaderItem(icon, itemID, false);
        LoadSprite.LoaderItem(changeIcon, itemID, false);
        di1.spriteName = "color" + clientInfo.CardID;
        di3.spriteName = "color" + clientInfo.CardID;
        StrikeInfo item = PlayerManager.GetStrikeInfoByID(clientInfo.initialstar * 100 + currentTeamBaller.brokenLayer + 1);
        pieceCount = ItemManager.GetBagItemCount(itemID);
        needCount.text = UtilTools.StringBuilder(pieceCount, "/", item.needCount.ToString());
    }
    private void OnPress(GameObject go, bool pressed)
    {
        if (go == addBtn.gameObject)
        {
            int amount = int.Parse(pieceNum.text);
            if (int.Parse(pieceNum.text) == ballerInfo.amount)
            {
                return;
            }
            amount++;
            pieceNum.text = amount.ToString();
        }
        else if (go == subtractBtn.gameObject)
        {
            int amount = int.Parse(pieceNum.text);
            if (int.Parse(pieceNum.text) == 1)
            {
                return;
            }
            amount--;
            pieceNum.text = amount.ToString();
        }
    }
    void OnClick(GameObject go)
    {
        if (go == chooseSuipian.gameObject)
        {
            ballerInfo = null;
            pieceNum.text = "1";
            List<object> list = new List<object>();
            TD_Player player;
            foreach (BallerPiece info in clipList.Values)
            {
                player = Instance.Get<PlayerManager>().GetItem(int.Parse(info.configID));
                if (info.amount > 0 && info.configID != currentTeamBaller.configId && player.initialstar == clientInfo.initialstar)
                {
                    list.Add(info);
                }
            }
            if (list.Count > 0)
            {
                switchClip.gameObject.SetActive(true);
                ballerInfo = list[0] as BallerPiece;
                cloneGrid = GameObject.Instantiate(ChooseClipGrid.gameObject).GetComponent<UIGrid>();
                UtilTools.SetParentWithPosition(cloneGrid.transform, scroView, ChooseClipGrid.transform.localPosition, Vector3.one);
                cloneGrid.enabled = true;
                cloneGrid.BindCustomCallBack(UpdatePieceGird);
                cloneGrid.StartCustom();
                cloneGrid.AddCustomDataList(list);
            }
            else
                GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_32"), null);
        }
        else if (go == strikeInfoBtn.gameObject)
        {
            List<object> decsList = new List<object>();
            string key = string.Empty;
            if (clientInfo.initialstar == 5)
            {
                key = "strikehighAdd";
            }
            else
            {
                key = "strikelowAdd";
            }
            for (int i = 1; i <= 20; i++)
            {
                string Name = key + i;
                string text = TextManager.GetPropsString(Name);
                if (text == Name)
                {
                    continue;
                }
                StrikeInfo info = PlayerManager.GetStrikeInfoByID(clientInfo.initialstar * 100 + i);
                string desc = string.Empty;
                if (info.add.Split(',').Length >= 2)
                {
                    if (info.add.Split(',').Length == 2)
                    {
                        desc = string.Format(text, info.add.Split(',')[0], info.add.Split(',')[1]);
                    }
                    else
                    {
                        desc = string.Format(text, info.add.Split(',')[0], info.add.Split(',')[1], info.add.Split(',')[2]);
                    }
                }
                else
                {
                    desc = string.Format(text, info.addSkill);
                }
                if (currentTeamBaller.brokenLayer >= i)
                    decsList.Add(string.Format("[05FF2D]{0}[-]", desc));
                else
                    decsList.Add(desc);
            }
            GUIManager.ShowHelpPanel(decsList);
        }
        else if (go == helpBtn.gameObject)
        {
            List<object> decsList = new List<object>();
            string text = TextManager.GetPropsString("state10001");
            decsList.Add(text);
            GUIManager.ShowHelpPanel(decsList);
        }
        else if (go == strikeBtn.gameObject)
        {
            string itemID = ItemManager.GetMaterialID(clientInfo.id.ToString());
            ServerCustom.instance.SendClientMethods("StrikeBaller", currentTeamBaller.id, itemID);
        }
        else if (go == switchBtn.gameObject)
        {
            if (ballerInfo == null)
            {
                GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_21"), null);
                return;
            }
            string itemID = ItemManager.GetMaterialID(clientInfo.id.ToString());
            ServerCustom.instance.SendClientMethods("SwitchPiece", itemID.ToString(), ballerInfo.itemID.ToString(), UtilTools.IntParse(Count.text), currentTeamBaller.id);
        }
        else if (go == closeBtn.gameObject || go == cancleBtn.gameObject)
        {
            CloseSwitchUI();
        }
        else if (go == minBtn.gameObject)
        {
            pieceNum.text = "1";
        }
        else if (go == maxBtn.gameObject)
        {
            pieceNum.text = ballerInfo.amount.ToString();
        }
        else if (go = okBtn.gameObject)
        {
            int money = CardConfig.GetCardInfo(clientInfo.initialstar).money * UtilTools.IntParse(pieceNum.text);
            neddMoney.text = money.ToString();
            switchClip.gameObject.SetActive(false);
            TD_Player player = Instance.Get<PlayerManager>().GetItem(int.Parse(ballerInfo.configID));
            LoadSprite.LoaderItem(chooseSuipian, ballerInfo.itemID, false);
            di2.spriteName = "color" + player.CardID;
            Count.text = pieceNum.text;
            chaneCount.gameObject.SetActive(true);
            chaneCount.text = pieceNum.text;
            if (cloneGrid != null)
                MonoBehaviour.Destroy(cloneGrid.gameObject);
        }
    }

    void UpdatePieceGird(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        BallerPiece piece = item.oData as BallerPiece;
        UILabel Name = item.mScripts[0] as UILabel;
        UISprite color = item.mScripts[1] as UISprite;
        UITexture head = item.mScripts[2] as UITexture;
        UILabel amount = item.mScripts[3] as UILabel;
        item.GetComponent<UIToggle>().value = piece == ballerInfo;
        item.onClick = ChoosePiece;
        amount.text = piece.amount.ToString();
        TD_Player player = Instance.Get<PlayerManager>().GetItem(int.Parse(piece.configID));
        if (player == null)
        {
            return;
        }
        Name.text = player.name;
        color.spriteName = UtilTools.StringBuilder("color", player.initialstar);
        LoadSprite.LoaderItem(head, piece.itemID, false);
    }
    void ChoosePiece(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        ballerInfo = item.oData as BallerPiece;
        pieceNum.text = "1";
    }

    public void StrikeSucessCallBack()
    {
        StrikeInfo item = PlayerManager.GetStrikeInfoByID(clientInfo.initialstar * 100 + currentTeamBaller.brokenLayer);
        pieceCount -= item.needCount;       
        teamMediator.panel.ballerName.text = TextManager.GetItemString(currentTeamBaller.configId) + string.Format("[05FF2D]{0}[-]", "  +" + currentTeamBaller.brokenLayer);
        if (currentTeamBaller.brokenLayer == 20)
        {
            tishi.gameObject.SetActive(true);
            return;
        }
        item = PlayerManager.GetStrikeInfoByID(clientInfo.initialstar * 100 + currentTeamBaller.brokenLayer + 1);
        needCount.text = UtilTools.StringBuilder(pieceCount, "/", item.needCount.ToString());
        //teamMediator.ShowBallerInfo();     
    }
    void CloseSwitchUI()
    {
        if (cloneGrid != null)
            MonoBehaviour.Destroy(cloneGrid.gameObject);
        LoadSprite.LoaderImage(chooseSuipian, "jiahao", false);
        di2.spriteName = "wupinkuang";
        ballerInfo = null;
        Count.text = "0";
        chaneCount.gameObject.SetActive(false);
        neddMoney.text = "0";
        switchClip.gameObject.SetActive(false);
    }
    public void SwtichSucessCallBack()
    {
        StrikeInfo item = PlayerManager.GetStrikeInfoByID(clientInfo.initialstar * 100 + currentTeamBaller.brokenLayer + 1);
        string itemID = ItemManager.GetMaterialID(clientInfo.id.ToString());
        if (clipList.ContainsKey(itemID))
        {
            clipList[itemID].amount += UtilTools.IntParse(Count.text);
        }
        if (clipList.ContainsKey(ballerInfo.itemID))
        {
            clipList[ballerInfo.itemID].amount -= UtilTools.IntParse(pieceNum.text);
        }
        pieceCount += UtilTools.IntParse(Count.text);
        needCount.text = UtilTools.StringBuilder(pieceCount, "/", item.needCount.ToString());
        CloseSwitchUI();
    }
    protected override void OnDestroy()
    {
        Debug.Log("+++++++++++++++++++++");
    }
}
