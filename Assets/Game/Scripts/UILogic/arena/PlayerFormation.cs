using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerFormation : MonoBehaviour
{ 
    public int enemyFormation;
    public int enemyfigthValue;
    public string enemy;
    private Transform myFormineList;
    private Transform myBall;
    private Transform enemyFormineList;
    private Transform enemyBall;
    private UILabel myFromationLabel;
    private UILabel enemyFormationLabel;
    private UILabel myFight;
    private UILabel enemyFight;
    private UILabel myName;
    private UILabel enemyName;
    private UISprite startBtn;
    private UISprite changeBtn;
    private UISprite backBtn;

    void Awake()
    {
        myFormineList = UtilTools.GetChild<Transform>(this.transform, "myformation/formineList");
        enemyFormineList = UtilTools.GetChild<Transform>(this.transform, "enemyformation/formineList");
        myBall = UtilTools.GetChild<Transform>(this.transform, "myformation/ball");
        enemyBall = UtilTools.GetChild<Transform>(this.transform, "enemyformation/ball");
        myFromationLabel = UtilTools.GetChild<UILabel>(this.transform, "myformation/formation");
        myFight = UtilTools.GetChild<UILabel>(this.transform, "myformation/teamfight");
        enemyFormationLabel = UtilTools.GetChild<UILabel>(this.transform, "enemyformation/formation");
        enemyFight = UtilTools.GetChild<UILabel>(this.transform, "enemyformation/teamfight");
        myName = UtilTools.GetChild<UILabel>(this.transform, "myformation/name");
        enemyName = UtilTools.GetChild<UILabel>(this.transform, "enemyformation/name");
        startBtn = UtilTools.GetChild<UISprite>(this.transform, "startBtn");
        changeBtn = UtilTools.GetChild<UISprite>(this.transform, "changeBtn");
        backBtn = UtilTools.GetChild<UISprite>(this.transform, "backBtn");
        UIEventListener.Get(startBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(changeBtn.gameObject).onClick = OnClick;
        UIEventListener.Get(backBtn.gameObject).onClick = OnClick;
    }
    void Start()
    {
        myName.text = PlayerMediator.playerInfo.name.ToString();
        enemyName.text = enemy;
        myFight.text = PlayerMediator.playerInfo.fightValue.ToString();
        enemyFight.text = enemyfigthValue.ToString();
        SetFormineBaller(PlayerMediator.playerInfo.formation, myFormineList, myBall, myFromationLabel);
        SetFormineBaller(enemyFormation, enemyFormineList, enemyBall, enemyFormationLabel);
    }
    void OnClick(GameObject go)
    {
        if (go == startBtn.gameObject)
        {
            ServerCustom.instance.SendClientMethods("onClientStartArenaPVP");
            MonoBehaviour.Destroy(this.gameObject);
        }
        else if (go == changeBtn.gameObject)
        {
            PureMVC.Patterns.Facade.Instance.SendNotification(NotificationID.TeamFormine_Show);
        }
        else if (go == backBtn.gameObject)
            MonoBehaviour.Destroy(this.gameObject);
    }
    private void SetFormineBaller(int fromation, Transform formation, Transform ball, UILabel desc)
    {
        List<FormationBaller> listObj = new List<FormationBaller>();
        TeamFormation info = TeamFormationConfig.GetTeamFormation(fromation);
        desc.text = info.name;
        string[] posIdArr = info.playerPos.Split(',');
        string[] posArr = info.position.Split(';');
        FormationBaller baller = null;
        for (int i = posIdArr.Length - 1; i >= 0; i--)
        {
            baller = new FormationBaller();
            int pos_id = GameConvert.IntConvert(posIdArr[i]);
            baller.pos_id = pos_id;
            baller.vector = ConfigParseUtil.ParseVec3(posArr[i]);


            listObj.Add(baller);
        }



        SetFormationList(listObj, formation, ball);

    }
    public void SetFormationList(List<FormationBaller> list, Transform formation, Transform ball)
    {
        GameObject temp = null;
        for (int i = 0; i < list.Count; i++)
        {
            FormationBaller baller = list[i];
            int index = i + 1;
            if (index < formation.childCount)
            {
                temp = formation.GetChild(index).gameObject;
            }
            else
                temp = NGUITools.AddChild(formation.gameObject, ball.gameObject);
            temp.SetActive(true);
            SetFormainBallerInfo(temp, baller);
        }

    }
    //设置上阵球员信息
    public void SetFormainBallerInfo(GameObject temp, FormationBaller baller)
    {
        UILabel name = temp.transform.FindChild("name").GetComponent<UILabel>();
        UILabel pos = temp.transform.FindChild("posLabel").GetComponent<UILabel>();
        UISprite color = temp.transform.FindChild("color").GetComponent<UISprite>();
        UISprite icon = temp.transform.FindChild("icon").GetComponent<UISprite>();
        GameObject bg = temp.transform.FindChild("bg").gameObject;
        GameObject gantan = temp.transform.FindChild("gantan").gameObject;

        temp.name = "item" + baller.pos_id;
        if (baller.baller != null)
        {
            TD_Player player = Instance.Get<PlayerManager>().GetItem(UtilTools.IntParse(baller.baller.configId));
            name.text = player.name;

            color.spriteName = "smallcolor" + baller.baller.star;
            icon.spriteName = baller.pos_id == 1 ? "baiyi" : "hongyi";

        }

        gantan.SetActive(false);

        temp.transform.localPosition = baller.vector;
        TeamPosition info = TeamPositionConfig.GetTeamPosition(baller.pos_id);
        if (info != null)
            pos.text = TextManager.GetUIString(info.name);

    }
    void OnDestroy()
    {
        myFormineList = null;
        myBall = null;
        enemyFormineList = null;
        enemyBall = null;
        myFromationLabel = null;
        enemyFormationLabel = null;
        myFight = null;
        enemyFight = null;
        myName = null;
        enemyName = null;
        startBtn = null;
        changeBtn = null;
        backBtn = null;
    }
}
