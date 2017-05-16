using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AbilityPanel : TeamMediator
{
    private Transform tishi;
    private UILabel nowLevel;
    private UILabel nextLevel;
    private UILabel property;
    private UILabel property1;
    private UILabel nowValue;
    private UILabel nextValue;
    private UISprite cancleBtn;
    private UISprite okBtn;
    private UIGrid ConsuleView;
    private int lastLevel;
    private int currentLevel;
    public  int choosePieceCount = 0;
    private int pieceAddExp = 0;
    private int lastExp = 0;
    private int addValues = 0;
    private int time = 0;
    private string chooseName = string.Empty;
    private BallerPiece chooseinfo;
    public static List<GameObject> pieceItemList = new List<GameObject>();
    public static Dictionary<int, BallerPiece> ballerPieceList = new Dictionary<int, BallerPiece>();

    public AbilityPanel(GameObject go)
    {
        tishi = UtilTools.GetChild<Transform>(go.transform, "tishi");
        ConsuleView = UtilTools.GetChild<UIGrid>(go.transform, "ScrollView/ConsuleView");
        nowLevel = UtilTools.GetChild<UILabel>(go.transform, "nowLevel");
        nextLevel = UtilTools.GetChild<UILabel>(go.transform, "nextLevel");
        property = UtilTools.GetChild<UILabel>(go.transform, "property");
        property1 = UtilTools.GetChild<UILabel>(go.transform, "property1");
        nowValue = UtilTools.GetChild<UILabel>(go.transform, "nowValue");
        nextValue = UtilTools.GetChild<UILabel>(go.transform, "nextValue");
        cancleBtn = UtilTools.GetChild<UISprite>(go.transform, "no");
        okBtn = UtilTools.GetChild<UISprite>(go.transform, "yes");
        UIEventListener.Get(cancleBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(okBtn.gameObject).onClick = OnClick;
    }
    void OnClick(GameObject go)
    {
        if (choosePieceCount == 0)
        {
            GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_21"), null);
            return;
        }
        if (go == okBtn.gameObject)
        {
            List<object> pieceList = new List<object>();
            foreach (int key in ballerPieceList.Keys)
            {
                Dictionary<string, object> list = new Dictionary<string, object>();
                list.Add("itemID", key);
                list.Add("number", ballerPieceList[key].amount);
                pieceList.Add(list);
            }
            ServerCustom.instance.SendClientMethods("onClientUpAbilityInfo", currentTeamBaller.id, chooseName, pieceList);

        }
        else if (go == cancleBtn.gameObject)
        {
            foreach (int key in ballerPieceList.Keys)
                clipList[key.ToString()].amount += ballerPieceList[key].amount;
            ballerPieceList.Clear();
            AddGridList();
            CheckLevel(lastExp);
            SetTextValue();
            lastLevel = currentLevel;
            ballerRateyInfo.UpdateToggleValue(lastExp);
        }
    }

    /// <summary>
    /// 能力提升成功回调
    /// </summary>
    public void AbilitySucess()
    {
        currentTeamBaller.GetType().GetField(chooseName).SetValue(currentTeamBaller, pieceAddExp + lastExp);
        lastExp += pieceAddExp;
        int infoValue =(int) currentTeamBaller.GetType().GetField(chooseName.Replace("Exp", "")).GetValue(currentTeamBaller);
        currentTeamBaller.GetType().GetField(chooseName.Replace("Exp", "")).SetValue(currentTeamBaller, infoValue + addValues);
        ballerInfoList[chooseName.Replace("Exp", "")].text = currentTeamBaller.GetType().GetField(chooseName.Replace("Exp", "")).GetValue(currentTeamBaller).ToString();
        AddGridList();
    }

    /// <summary>
    /// 界面初始化
    /// </summary>
    public override void SetChildPanel()
    {
        ConsuleView.enabled = true;
        ConsuleView.BindCustomCallBack(UpdatePieceGrid);
        ConsuleView.StartCustom();
        AddGridList();
        chooseName = "shootExp";
        lastExp = (int)currentTeamBaller.GetType().GetField(chooseName).GetValue(currentTeamBaller);
        CheckLevel(lastExp);
        SetTextValue();
        chooseinfo = null;
        lastLevel = currentLevel;   
        property.text = TextManager.GetUIString("Shoot");
        property1.text = TextManager.GetUIString("Shoot");        
        tishi.gameObject.SetActive(clipList.Count <= 0);      
    }
    /// <summary>
    /// 刷新碎片
    /// </summary>
    void UpdatePieceGrid(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        BallerPiece info = item.oData as BallerPiece;
        UITexture head = item.mScripts[0] as UITexture;
        UILabel count = item.mScripts[1] as UILabel;
        UISprite color = item.mScripts[2] as UISprite;
        LoadSprite.LoaderItem(head, info.itemID, false);
        TD_Player player = Instance.Get<PlayerManager>().GetItem(int.Parse(info.configID));
        color.spriteName = "color" + player.initialstar;
        count.text = info.amount.ToString();
        item.onClick = OnClickItemEx;
        LongClickEvent.Get(item.gameObject).onPress = ClickItem;
        LongClickEvent.Get(item.gameObject).duration = 4f;
    }
    void OnClickItemEx(UIGridItem item)
    {
        if (item == null || item.mScripts == null || item.oData == null)
            return;
        time = 0;
        LongClickEvent.Get(item.gameObject).time = 0;
        ClickItem(item.gameObject, true);
    }
    /// <summary>
    /// 初始化碎片Grid
    /// </summary>
    void AddGridList()
    {
        addValues = 0;
        pieceAddExp = 0;
        choosePieceCount = 0;
        time = 0;
        ballerPieceList.Clear();
        cancleBtn.gameObject.SetActive(false);
        List<object> listObj = new List<object>();
        foreach (BallerPiece item in clipList.Values)
        {
            if (item.amount < 1)
            {
                continue;
            }
            listObj.Add(item);
        }
        if (listObj.Count < 1)
            return;
        ConsuleView.AddCustomDataList(listObj);
    }

    /// <summary>
    /// 长按碎片
    /// </summary>
    /// <param name="data"></param>
    /// <param name="go"></param>
    void ClickItem(GameObject go, bool pressed)
    {
        if (!pressed)
            return;
        int count = 0;
        if (chooseinfo != null && chooseinfo != go.GetComponent<UIGridItem>().oData as BallerPiece)
            time = 0;
        chooseinfo = go.GetComponent<UIGridItem>().oData as BallerPiece;
        if (chooseinfo.amount < 1)
        {
            return;
        }
        if (currentLevel == AbilityConfig.GetAbilityList().Count)
        {
            GUIManager.SetPromptInfo(TextManager.GetUIString("UILevelMax"), null);
            return;
        }
        if (!cancleBtn.gameObject.activeSelf)
        {
            cancleBtn.gameObject.SetActive(true);
        }
        if (time >= 3)
            time = 3;
        TD_Player player = Instance.Get<PlayerManager>().GetItem(int.Parse(chooseinfo.configID));      
        count = (int)Mathf.Pow(4, time);
        if (chooseinfo.amount >= count)
            choosePieceCount += count;
        else
        {
            count = chooseinfo.amount;
            choosePieceCount += count;
        }
        pieceAddExp = CardConfig.GetCardInfo(player.initialstar).exp * choosePieceCount;
        ballerRateyInfo.UpdateToggleValue(pieceAddExp + lastExp);
        CheckLevel(pieceAddExp + lastExp);
        if (lastLevel < currentLevel)
        {
            SetTextValue();
            addValues += GetAbiltyAddValue(currentLevel, chooseName);
            lastLevel = currentLevel;
        }
        chooseinfo.amount -= count;
        ConsuleView.UpdateCustomData(chooseinfo);
        BallerPiece Piece = new BallerPiece();
        Piece.itemID = chooseinfo.itemID;
        Piece.amount = count;
        if (ballerPieceList.ContainsKey(UtilTools.IntParse(Piece.itemID)))
            ballerPieceList[UtilTools.IntParse(Piece.itemID)].amount += Piece.amount;
        else
            ballerPieceList.Add(UtilTools.IntParse(Piece.itemID), Piece);
        time++;
    }

    /// <summary>
    /// 选择一个属性
    /// </summary>
    public void ChooseOneInfo(GameObject go)
    {
        property.text = TextManager.GetUIString(go.name);
        property1.text = TextManager.GetUIString(go.name);
        chooseName = go.GetComponentInParent<UISlider>().name;
        lastExp = (int)currentTeamBaller.GetType().GetField(chooseName).GetValue(currentTeamBaller);
        CheckLevel(lastExp);
        SetTextValue();
        lastLevel = currentLevel;
    }

    /// <summary>
    /// 计算该属性当前等级
    /// </summary>
    void CheckLevel(int exp)
    {
        if (exp < AbilityConfig.GetAbilityItem(1).needExp)
            currentLevel = 1;
        else
        {
            foreach (AbilityItem item in AbilityConfig.GetAbilityList().Values)
            {
                if (item.needExp==0)
                {
                    currentLevel = AbilityConfig.GetAbilityList().Count;
                }
                if (exp < item.needExp)
                {
                    currentLevel = item.level;
                    break;
                }
                else if (exp == item.needExp)
                {
                    currentLevel = item.level + 1;
                    break;
                }
            }
        }
    }
    void SetTextValue()
    {
        nowLevel.text = UtilTools.StringBuilder("Lv ", currentLevel);
        nowValue.text = UtilTools.StringBuilder(" +", GetAbiltyAddValue(currentLevel, chooseName));
        if (currentLevel == AbilityConfig.GetAbilityList().Count)
        {
            nowLevel.text = UtilTools.StringBuilder("Lv ", currentLevel);
            nextLevel.text = TextManager.GetUIString("UILevelMax");
            return;
        }
        nextLevel.text = UtilTools.StringBuilder("Lv ", currentLevel + 1);
        nextValue.text = UtilTools.StringBuilder(" +", GetAbiltyAddValue(currentLevel + 1, chooseName));
    }
    /// <summary>
    /// 获取当前属性能力值
    /// </summary>
    int GetAbiltyAddValue(int level,string Name)
    {
        AbilityItem info = AbilityConfig.GetAbilityItem(level);
        if (info == null)
        {
            return 0;
        }
        switch (Name)
        {
            case "shootExp":
                return info.shoot;
            case "trickExp":
                return info.trick;
            case "keepExp":
                return info.keep;
            case "defendExp":
                return info.def;
            case "stealExp":
                return info.steal;
            case "passBallExp":
                return info.pass;
            case "controllExp":
                return info.control;
            case "reelExp":
                return info.reel;
        }
        return 0;
    }
}
