using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class InheritPanel : TeamMediator
{
    private Transform left;
    private Transform right;
    private Transform addValue;
    private UILabel inheriterName;
    private UILabel inheriteLevel;
    private UIToggle Item_1;
    private UIToggle Item_2;
    private UIToggle Item_3;
    private UISprite InheritBtn;
    private UISprite beInheriteColor;
    private UISprite See;
    private UITexture inheriteHead;
    private UITexture inheriteAdd;
    private Transform inheriterStarTrans;
    private Transform SeeInherit;
    private Transform check;
    private UISprite inheritereColor;
    private UISprite[] inheriterStar;
    private UISlider inheriterStarExp;
    private UILabel beInheriterName;
    private UILabel beInheriterLevel;
    private UITexture beInheriteHead;
    private UISprite helpBtn;
    private UISprite[] beInheriterStar;
    private UISprite CloseBtn;
    private UISlider beInheriterExp;
    private UILabel expNum;
    private UILabel money;
    private TeamBaller inHeriter;
    private int chooseMaterial;
    private bool haveCost;
    private List<ConSuleItem> ConSuleList = new List<ConSuleItem>();
    private List<GameObject> cloneItem = new List<GameObject>();

    public InheritPanel(GameObject go)
    {
        check = UtilTools.GetChild<Transform>(go.transform, "check");
        addValue = UtilTools.GetChild<Transform>(go.transform, "Add");
        SeeInherit = UtilTools.GetChild<Transform>(go.transform, "SeeInherit");
        left = UtilTools.GetChild<Transform>(go.transform, "Left");
        right = UtilTools.GetChild<Transform>(go.transform, "Right");
        inheriterName = UtilTools.GetChild<UILabel>(go.transform, "Left/Text");
        inheriteLevel = UtilTools.GetChild<UILabel>(go.transform, "Left/Level");
        money = UtilTools.GetChild<UILabel>(go.transform, "money");
        inheriteHead = UtilTools.GetChild<UITexture>(go.transform, "Left/head");
        inheriteAdd = UtilTools.GetChild<UITexture>(go.transform, "Left/add");
        helpBtn = UtilTools.GetChild<UISprite>(go.transform, "helpBtn");
        inheritereColor = UtilTools.GetChild<UISprite>(go.transform, "Left/color");
        inheriterStar = UtilTools.GetChilds<UISprite>(go.transform, "Left/star");
        inheriterStarTrans = UtilTools.GetChild<Transform>(go.transform, "Left/star");
        inheriterStarExp = UtilTools.GetChild<UISlider>(go.transform, "Left/Slider");
        beInheriterName = UtilTools.GetChild<UILabel>(go.transform, "Right/Text");
        beInheriterLevel = UtilTools.GetChild<UILabel>(go.transform, "Right/Level");
        beInheriteHead = UtilTools.GetChild<UITexture>(go.transform, "Right/head");
        beInheriterStar = UtilTools.GetChilds<UISprite>(go.transform, "Right/star");
        beInheriteColor = UtilTools.GetChild<UISprite>(go.transform, "Right/color");
        beInheriterExp = UtilTools.GetChild<UISlider>(go.transform, "Right/Slider");
        InheritBtn = UtilTools.GetChild<UISprite>(go.transform, "Inherit");
        See = UtilTools.GetChild<UISprite>(go.transform, "See");
        CloseBtn = UtilTools.GetChild<UISprite>(go.transform, "SeeInherit/CloseBtn");
        expNum = UtilTools.GetChild<UILabel>(go.transform, "Right/Slider/Num");
        UIEventListener.Get(inheriteAdd.gameObject).onClick = OnClick;
        UIEventListener.Get(InheritBtn.gameObject).onClick = OnClick;
        ConSuleList = UtilTools.SetConSumeItemList(3, go.transform);
        UIEventListener.Get(helpBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(inheriteHead.gameObject).onClick = OnClick;
        UIEventListener.Get(ConSuleList[0].trans.gameObject).onClick = OnClick;
        UIEventListener.Get(ConSuleList[1].trans.gameObject).onClick = OnClick;
        UIEventListener.Get(ConSuleList[2].trans.gameObject).onClick = OnClick;
        UIEventListener.Get(See.gameObject).onClick = OnClick;
        UIEventListener.Get(CloseBtn.gameObject).onClick = OnClick;
    }

    public override void SetChildPanel()
    {
        InitializeUI();
        beInheriterName.text = clientInfo.name;
        for (int i = 0; i < ConSuleList.Count; ++i)
        {
            string itemID = PlayerManager.inheritItemList[i + 1].itemId;
            ConSuleList[i].name.text = TextManager.GetItemString(itemID);
            ConSuleList[i].count.text = ItemManager.GetBagItemCount(itemID).ToString();
            ItemInfo item = ItemManager.GetItemInfo(itemID);
            if (item == null)
                return;
            ConSuleList[i].color.spriteName = UtilTools.StringBuilder("color" + item.color);
            LoadSprite.LoaderItem(ConSuleList[i].Icon, itemID);
            if (UtilTools.IntParse(ConSuleList[i].count.text) <= 0 && haveCost)
            {
                haveCost = false;
            }
        }
    }

    void InitializeUI()
    {
        inHeriter = null;
        haveCost = true;
        money.text = "0";
        check.gameObject.SetActive(false);
        chooseMaterial = 0;
        inheriterName.gameObject.SetActive(false);
        inheriteLevel.gameObject.SetActive(false);
        inheritereColor.gameObject.SetActive(false);
        inheriterStarTrans.gameObject.SetActive(false);
        inheriteHead.gameObject.SetActive(false);
        inheriteAdd.gameObject.SetActive(true);
        inheriterStarExp.gameObject.SetActive(false);
        beInheriterLevel.text = UtilTools.StringBuilder("Lv ", currentTeamBaller.level);
        beInheriteColor.spriteName = UtilTools.StringBuilder("color", currentTeamBaller.star);
        LoadSprite.LoaderImage(inheriteAdd, "jiahao", false);
        LoadSprite.LoaderHead(beInheriteHead, "Card" + currentTeamBaller.configId, false);
        UtilTools.SetStar(currentTeamBaller.star, beInheriterStar, clientInfo.maxstar);
        PlayerItem item = PlayerManager.GetPlayerItem(currentTeamBaller.level);
        expNum.text = UtilTools.StringBuilder(currentTeamBaller.exp, "/", item.needExp);
        beInheriterExp.value = currentTeamBaller.exp * 1.0f / item.needExp;
    }
    void OnClick(GameObject go)
    {
        if (go == inheriteAdd.gameObject || go == inheriteHead.gameObject)
        {
            if (teamList.ContainsKey(currentTeamBaller.configId))
            {
                teamList[currentTeamBaller.configId] = currentTeamBaller;
            }
            ItemMediator.panelType = PanelType.SwitchInherit;
            List<object> list = new List<object>();
            foreach (TeamBaller item in teamList.Values)
            {
                if (item.isSelf > 0 || item.id == currentTeamBaller.id || item.level <= currentTeamBaller.level)
                {
                    continue;
                }
                list.Add(item as object);
            }
            if (list.Count < 1)
            {
                GUIManager.SetJumpText(TextManager.GetSystemString("ui_system_51"));
                return;
            }
            Facade.SendNotification(NotificationID.ItemInfo_Show, list);
        }
        else if (go == InheritBtn.gameObject)
        {
            if (inHeriter == null)
            {
                GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_1"), null);
                return;
            }
            else if (haveCost && chooseMaterial == 0)
            {
                GUIManager.SetPromptInfoChoose(TextManager.GetUIString("UICreate1"), TextManager.GetSystemString("ui_system_2"), UseCost);
                return;
            }
            string pieceID = ItemManager.GetMaterialID(inHeriter.configId);
            ServerCustom.instance.SendClientMethods("OnClientInherit", inHeriter.id, currentTeamBaller.id, UtilTools.IntParse(pieceID), UtilTools.IntParse(PlayerManager.inheritItemList[chooseMaterial].itemId));
        }
        else if (go == ConSuleList[0].trans.gameObject)
        {
            if (chooseMaterial == 1)
            {
                check.gameObject.SetActive(false);
                chooseMaterial = 0;
                return;
            }
            check.SetParent(ConSuleList[0].trans);
            check.gameObject.SetActive(true);
            check.localPosition = new Vector3(34, 50, 0);
            chooseMaterial = 1;
        }
        else if (go == ConSuleList[1].trans.gameObject)
        {
            if (chooseMaterial == 2)
            {
                check.gameObject.SetActive(false);
                chooseMaterial = 0;
                return;
            }
            check.SetParent(ConSuleList[1].trans);
            check.gameObject.SetActive(true);
            check.localPosition = new Vector3(34, 50, 0);
            chooseMaterial = 2;
        }
        else if (go == ConSuleList[2].trans.gameObject)
        {
            if (chooseMaterial == 3)
            {
                check.gameObject.SetActive(false);
                chooseMaterial = 0;
                return;
            }
            check.SetParent(ConSuleList[2].trans);
            check.gameObject.SetActive(true);
            check.localPosition = new Vector3(34, 50, 0);
            chooseMaterial = 3;
        }
        else if (go == See.gameObject)
        {
            teamMediator.ShowBallerInfo();
            if (cloneItem.Count > 0)
            {
                for (int i = 0; i < cloneItem.Count; i++)
                {
                    MonoBehaviour.Destroy(cloneItem[i]);
                }
                cloneItem.Clear();
            }
            if (inHeriter == null)
            {
                GUIManager.SetPromptInfo(TextManager.GetUIString("UI1139"), null);
                return;
            }
            SeeInherit.gameObject.SetActive(true);
            GameObject item0 = GameObject.Instantiate(teamMediator.panel.ballerInfo.gameObject);
            UtilTools.SetParentWithPosition(item0.transform, SeeInherit, new Vector3(-194, 41, 0), Vector3.one);
            item0.AddComponent<UIPanel>().depth = 40;
            item0.GetComponentsInChildren<UISprite>()[0].gameObject.SetActive(false);
            item0.gameObject.SetActive(true);

            InheritItem item = PlayerManager.inheritItemList[chooseMaterial];
            PlayerItem player = null;
            int changeExp = 0;
            int changeLevel = 0;
            int exp = (int)(inHeriter.exp * item.expRate);
            if (exp > currentTeamBaller.exp)
            {
                changeExp = exp;
                player = PlayerManager.GetPlayerItem(currentTeamBaller.level);
                while (player.needExp <= exp)
                {
                    changeLevel++;
                    player = PlayerManager.GetPlayerItem(changeLevel);
                    if (player == null)
                        break;
                    else if (changeLevel == PlayerMediator.playerInfo.level)
                    {
                        changeExp = player.needExp;
                        break;
                    }
                }
            }
            else
            {
                changeLevel = currentTeamBaller.level;
                changeExp = currentTeamBaller.exp;
            }
            List<int> infoList = new List<int>();
            Dictionary<string, string> name = new Dictionary<string, string>();
            name.Add("shootExp", "shootM");
            name.Add("passBallExp", "passBallM");
            name.Add("reelExp", "reelM");
            name.Add("keepExp", "keepM");
            name.Add("stealExp", "stealM");
            name.Add("trickExp", "trickM");
            name.Add("defendExp", "defendM");
            name.Add("controllExp", "controllM");
            int addValues = 0;
            foreach (string key in name.Keys)
            {
                addValues = 0;
                int shootExp = (int)(((int)inHeriter.GetType().GetField(key).GetValue(inHeriter)) * item.powerLevelRate)
                    - (int)currentTeamBaller.GetType().GetField(key).GetValue(currentTeamBaller);
                int shootM = (int)(((int)inHeriter.GetType().GetField(name[key]).GetValue(inHeriter)) * item.powerLevelRate)
                    - (int)currentTeamBaller.GetType().GetField(name[key]).GetValue(currentTeamBaller);
                if (shootExp > 0)
                    addValues += shootExp;
                if (shootM > 0)
                    addValues += shootM;
                infoList.Add(addValues);
            }
            if (changeLevel != currentTeamBaller.level)
            {
                int shoot = 0;
                int passBall = 0;
                int reel = 0;
                int keep = 0;
                int steal = 0;
                int trick = 0;
                int defend = 0;
                int controll = 0;
                for (int i = currentTeamBaller.level; i < changeLevel; ++i)
                {
                    player = PlayerManager.GetPlayerItem(i);
                    shoot += player.shoot;
                    passBall += player.pass;
                    reel += player.reel;
                    keep += player.keep;
                    steal += player.steal;
                    trick += player.trick;
                    defend += player.def;
                    controll += player.control;
                }
                infoList[0] += shoot;
                infoList[1] += passBall;
                infoList[2] += reel;
                infoList[3] += keep;
                infoList[4] += steal;
                infoList[5] += trick;
                infoList[6] += defend;
                infoList[7] += controll;
            }
            UISprite[] info = item0.GetComponentsInChildren<UISprite>();
            for (int i = 0; i < 10; i++)
            {
                if (i == 4 || i == 5)
                {
                    continue;
                }
                GameObject prefab = GameObject.Instantiate(addValue.gameObject);
                if (i >= 6)
                    prefab.transform.FindChild("Label").GetComponent<UILabel>().text = infoList[i - 2].ToString();
                else
                    prefab.transform.FindChild("Label").GetComponent<UILabel>().text = infoList[i].ToString();
                UtilTools.SetParentWithPosition(prefab.transform, item0.transform, info[i].transform.localPosition + new Vector3(160, 0, 0), Vector3.one);
                prefab.gameObject.SetActive(true);
                cloneItem.Add(prefab);
            }
            cloneItem.Add(item0);
            GameObject item1 = GameObject.Instantiate(left.gameObject);
            UtilTools.SetParentWithPosition(item1.transform, SeeInherit, new Vector3(-212, -218, 0), Vector3.one);
            item1.AddComponent<UIPanel>().depth = 40;
            cloneItem.Add(item1);
            GameObject item2 = GameObject.Instantiate(right.gameObject);
            item2.transform.FindChild("Level").GetComponent<UILabel>().text = "Lv" + changeLevel.ToString();
            player = PlayerManager.GetPlayerItem(changeLevel);
            item2.transform.FindChild("Slider/Num").GetComponent<UILabel>().text = UtilTools.StringBuilder(changeExp, "/", player.needExp);
            item2.transform.FindChild("Slider").GetComponent<UISlider>().value = changeExp * 1.0f / player.needExp;

            UtilTools.SetParentWithPosition(item2.transform, SeeInherit, new Vector3(151, -218, 0), Vector3.one);
            item2.AddComponent<UIPanel>().depth = 40;
            cloneItem.Add(item2);
        }
        else if (go == helpBtn.gameObject)
        {
            List<object> decsList = new List<object>();
            for (int i = 2; i <= 5; i++)
            {
                string Name = "state1000" + i.ToString();
                string text = TextManager.GetPropsString(Name);
                decsList.Add(text);
            }
            GUIManager.ShowHelpPanel(decsList);
        }
        else if (go == CloseBtn.gameObject)
        {
            SeeInherit.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 确定不使用道具
    /// </summary>
    void UseCost()
    {
        string pieceID = ItemManager.GetMaterialID(inHeriter.configId);
        ServerCustom.instance.SendClientMethods("OnClientInherit", inHeriter.id, currentTeamBaller.id, UtilTools.IntParse(pieceID), UtilTools.IntParse(PlayerManager.inheritItemList[chooseMaterial].itemId));
    }
    public void CheckInheriter(string configID)
    {
        inHeriter = teamList[configID];
        inheriterStarExp.gameObject.SetActive(true);
        inheriteLevel.gameObject.SetActive(true);
        inheriterName.gameObject.SetActive(true);
        inheriterStarTrans.gameObject.SetActive(true);
        inheritereColor.gameObject.SetActive(true);
        inheriteHead.gameObject.SetActive(true);
        inheriteAdd.gameObject.SetActive(false);
        inheriterName.text = TextManager.GetItemString(configID);
        inheriteLevel.text = UtilTools.StringBuilder("Lv", inHeriter.level);
        money.text = PlayerManager.inheritItemList[chooseMaterial].money.ToString();
        PlayerItem item = PlayerManager.GetPlayerItem(inHeriter.level);
        TD_Player player = Instance.Get<PlayerManager>().GetItem(int.Parse(inHeriter.configId));
        inheriterStarExp.transform.FindChild("Num").GetComponent<UILabel>().text = UtilTools.StringBuilder(inHeriter.exp, "/", item.needExp);
        inheriterStarExp.value = inHeriter.exp * 1.0f / item.needExp;
        inheritereColor.spriteName = UtilTools.StringBuilder("color", currentTeamBaller.star);
        LoadSprite.LoaderHead(inheriteHead, "Card" + configID, false);
        UtilTools.SetStar(inHeriter.star, inheriterStar, player.maxstar);
        if (cloneItem.Count > 0)
        {
            for (int i = 0; i < cloneItem.Count; i++)
            {
                MonoBehaviour.Destroy(cloneItem[i]);
            }
            cloneItem.Clear();
        }
    }

    public void InHeriSucess()
    {       
        inHeriter.level = 1;
        inHeriter.exp = 0;
        inHeriter.shootM = 0;
        inHeriter.shootExp = 0;
        inHeriter.defendExp = 0;
        inHeriter.defendM = 0;
        inHeriter.passBallM = 0;
        inHeriter.passBallExp = 0;
        inHeriter.trickM = 0;
        inHeriter.trickExp = 0;
        inHeriter.stealM = 0;
        inHeriter.stealExp = 0;
        inHeriter.controllM = 0;
        inHeriter.controllExp = 0;
        inHeriter.keepM = 0;
        inHeriter.keepExp = 0;
        inHeriter.reelM = 0;
        inHeriter.reelExp = 0;
        if (chooseMaterial != 0)
            ConSuleList[chooseMaterial - 1].count.text = (UtilTools.IntParse(ConSuleList[chooseMaterial - 1].count.text) - 1).ToString();
        InitializeUI();     
    }
}
