using System.Collections.Generic;
using UnityEngine;


public class BallerRateyInfo : TeamMediator
{
    private UIToggle[] infoTog;
    private UISlider trick;
    private UISlider keep;
    private UISlider steal;
    private UISlider def;
    private UISlider pass;
    private UISlider shoot;
    private UISlider control;
    private UISlider reel;
    private UISprite addItem;
    private MentalityMaxInfo maxMentalityinfo;
    private string chooseInfoName;
    private UISlider chooseToggle;
    public bool hasDownValue = false;
    private bool canClickOnece = true;
    private GameObject parent;
    public List<GameObject> cloneObj = new List<GameObject>();
    private Dictionary<string,UISlider> perCentList = new Dictionary<string, UISlider>();
    public BallerRateyInfo(GameObject go)
    {
        parent = go;
        addItem = UtilTools.GetChild<UISprite>(go.transform, "Add");
        trick = UtilTools.GetChild<UISlider>(go.transform, "trickExp");
        perCentList.Add("trickExp", trick);
        keep = UtilTools.GetChild<UISlider>(go.transform, "keepExp");
        perCentList.Add("keepExp", keep);
        steal = UtilTools.GetChild<UISlider>(go.transform, "stealExp");
        perCentList.Add("stealExp", steal);
        def = UtilTools.GetChild<UISlider>(go.transform, "defendExp");
        perCentList.Add("defendExp", def);
        pass = UtilTools.GetChild<UISlider>(go.transform, "passBallExp");
        perCentList.Add("passBallExp", pass);
        shoot = UtilTools.GetChild<UISlider>(go.transform, "shootExp");
        perCentList.Add("shootExp", shoot);
        control = UtilTools.GetChild<UISlider>(go.transform, "controllExp");
        perCentList.Add("controllExp", control);
        reel = UtilTools.GetChild<UISlider>(go.transform, "reelExp");
        perCentList.Add("reelExp", reel);
        infoTog = UtilTools.GetChilds<UIToggle>(go.transform, string.Empty);
    }

    public override void SetChildPanel()
    {
        if (cloneObj.Count > 0)
        {
            for (int i = 0; i < cloneObj.Count; ++i)
            {
                MonoBehaviour.Destroy(cloneObj[i]);
            }
            cloneObj.Clear();
        }
        for (int i = 0; i < infoTog.Length; ++i)
        {
            infoTog[i].gameObject.SetActive(currentType == BallType.Ability);
            UIEventListener.Get(infoTog[i].gameObject).onClick = ChooseInfoUpAbility;
        }
        // 能力
        if (currentType == BallType.Ability)
        {
            chooseToggle = shoot;
            infoTog[0].value = true;
            chooseInfoName = "shootExp";
            foreach (string key in perCentList.Keys)
            {
                int currentExp = (int)currentTeamBaller.GetType().GetField(key).GetValue(currentTeamBaller);
                string text = ComputeRate(currentExp);
                if (text.Split(',').Length > 1)
                {
                    perCentList[key].value = int.Parse(text.Split(',')[0]) * 1.0f / int.Parse(text.Split(',')[1]);
                    perCentList[key].transform.FindChild("percent").GetComponent<UILabel>().text = UtilTools.StringBuilder(int.Parse(text.Split(',')[0]), "/", int.Parse(text.Split(',')[1]));
                }
                else
                {
                    perCentList[key].value = 1;
                    perCentList[key].transform.FindChild("percent").GetComponent<UILabel>().text = text;
                }
            }
        }
        // 意识
        else
        {
            maxMentalityinfo = MentalityMaxConfig.GetMentalityMaxInfo(clientInfo.initialstar);
            foreach (string key in perCentList.Keys)
            {
                string Name = key.Replace("Exp", "") + "M";
                int currentExp = (int)currentTeamBaller.GetType().GetField(Name).GetValue(currentTeamBaller);
                perCentList[key].value = currentExp * 1.0f / GetProperty(key);
                perCentList[key].transform.FindChild("percent").GetComponent<UILabel>().text = UtilTools.StringBuilder(currentExp, "/", GetProperty(key));
            }
        }
    }

    /// <summary>
    /// 计算百分比
    /// </summary>
    /// <param name="exp"></param>
    /// <returns></returns>
    string ComputeRate(int exp)
    {
        AbilityItem lastInfo;
        AbilityItem info = AbilityConfig.GetAbilityItem(1);
        if (exp < info.needExp)
            return exp.ToString() + "," + info.needExp.ToString();
        foreach (AbilityItem item in AbilityConfig.GetAbilityList().Values)
        {
            if (item.needExp == 0)
            {
                return TextManager.GetSystemString("ui_system_20");
            }
            else if (exp < item.needExp)
            {
                info = AbilityConfig.GetAbilityItem(item.level);
                lastInfo = AbilityConfig.GetAbilityItem(item.level - 1);
                return (exp - lastInfo.needExp).ToString() + "," + (info.needExp - lastInfo.needExp).ToString();
            }
            else if (exp == item.needExp)
            {
                lastInfo = AbilityConfig.GetAbilityItem(item.level);
                info = AbilityConfig.GetAbilityItem(item.level + 1);
                return "0" + "," + (info.needExp- lastInfo.needExp).ToString();
            }
        }
        return "0";
    }
    void ChooseInfoUpAbility(GameObject go)
    {
        if (go == chooseToggle.GetComponentInChildren<UIToggle>().gameObject)
        {
            return;
        }
        if (abilityPanel.choosePieceCount > 0)
        {
            GUIManager.SetPromptInfo(TextManager.GetSystemString("ui_system_29"), null);
            chooseToggle.GetComponentInChildren<UIToggle>().value = true;
            return;
        }
        chooseToggle = go.GetComponentInParent<UISlider>();
        chooseInfoName = go.name;      
        abilityPanel.ChooseOneInfo(go);
    }

    public void UpdateToggleValue(int addExp)
    {
        string text = ComputeRate(addExp);
        if (text.Split(',').Length > 1)
        {
            chooseToggle.value = int.Parse(text.Split(',')[0]) * 1.0f / int.Parse(text.Split(',')[1]);
            chooseToggle.transform.FindChild("percent").GetComponent<UILabel>().text = UtilTools.StringBuilder(int.Parse(text.Split(',')[0]), "/", int.Parse(text.Split(',')[1]));
        }
        else
        {
            chooseToggle.value = 1;
            chooseToggle.transform.FindChild("percent").GetComponent<UILabel>().text = text;
        }     
    }

    public void CheckUpMentality()
    {
        CreateObj();
        if (!hasDownValue)
        {
            foreach (MentalityUpItem item in MentalityPanel.mentalityUpList.Values)
            {
                if (item.Value < 0)
                {
                    HaveDownInfo();
                    return;
                }
            }
        }
        ServerCustom.instance.SendClientMethods("UpDateMentalityInfo", currentTeamBaller.id);      
    }
    public void MentalitySuceess()
    {
        if (MentalityPanel.mentalityUpList.Count <= 3)
        {
            foreach (MentalityUpItem item in MentalityPanel.mentalityUpList.Values)
            {
                AddProperty(item.Name, item.Value);
            }
            hasDownValue = false;
        }
        else
        {
            foreach (MentalityUpItem item in MentalityPanel.mentalityUpList.Values)
            {
                AddProperty(item.Name.Replace(item.Name.ToCharArray()[0].ToString(),""), item.Value);
            }
        }
    }
    void CreateObj()
    {      
        if (cloneObj.Count > 0)
        {
            for (int i = 0; i < cloneObj.Count; ++i)
            {
                MonoBehaviour.Destroy(cloneObj[i]);
            }
            cloneObj.Clear();
        }
        foreach (MentalityUpItem item in MentalityPanel.mentalityUpList.Values)
        {
            string key = item.Name + "Exp";
            GameObject obj = GameObject.Instantiate(addItem).gameObject;
            if (perCentList[key].transform.localPosition.x > 300)
                UtilTools.SetParentWithPosition(obj.transform, parent.transform, perCentList[key].transform.localPosition - new Vector3(86, 0, 0), Vector3.one);
            else
                UtilTools.SetParentWithPosition(obj.transform, parent.transform, perCentList[key].transform.localPosition + new Vector3(100, 0, 0), Vector3.one);
            obj.SetActive(true);
            if (item.Value < 0)
                obj.GetComponent<UISprite>().spriteName = "hongjiantou";
            else
                obj.GetComponent<UISprite>().spriteName = "lvjiantou";
            obj.transform.FindChild("Label").GetComponent<UILabel>().text = item.Value.ToString();
            cloneObj.Add(obj);
        }
    }
    public static void HaveDownInfo()
    {
        mentalityPanel.yesUp.gameObject.SetActive(true);
        mentalityPanel.cancleUp.gameObject.SetActive(true);
        mentalityPanel.uponce.gameObject.SetActive(false);
    }
    public void AddProperty(string Name, int number)
    {
        string key = Name + "M";
        int currentM = GetPropertyInfo(key);
        string keys = key.Replace("M", "") + "Exp";
        perCentList[keys].value = currentM * 1.0f / GetProperty(keys);
        perCentList[keys].transform.FindChild("percent").GetComponent<UILabel>().text = UtilTools.StringBuilder(currentM, "/", GetProperty(keys));
    }
    int GetPropertyInfo(string Name)
    {
        switch (Name)
        {
            case "shootM":
                return currentTeamBaller.shootM;
            case "passBallM":
                return currentTeamBaller.passBallM;
            case "reelM":
                return currentTeamBaller.reelM;
            case "defendM":
                return currentTeamBaller.defendM;
            case "trickM":
                return currentTeamBaller.trickM;
            case "stealM":
                return currentTeamBaller.stealM;
            case "controllM":
                return currentTeamBaller.controllM;
            case "keepM":
                return currentTeamBaller.keepM;
        }
        return 0;
    }
    int GetProperty(string Name)
    {
        switch (Name)
        {
            case "shootExp":
               return maxMentalityinfo.shoot;
            case "passBallExp":
                return maxMentalityinfo.pass;
            case "reelExp":
                return maxMentalityinfo.reel;
            case "defendExp":
                return maxMentalityinfo.def;
            case "trickExp":
                return maxMentalityinfo.trick;
            case "stealExp":
                return maxMentalityinfo.steal;
            case "controllExp":
                return maxMentalityinfo.control;
            case "keepExp":
                return maxMentalityinfo.keep;
        }
        return 0;
    }
}
