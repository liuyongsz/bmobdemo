using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerNameFollow : MonoBehaviour {

    public Transform Owner;
    public UITexture EffIcon;
    public UILabel TxtName;
    public int xOffset;
    public int yOffset;

    private MatchPlayerItem m_matchItem;
    // Use this for initialization
    void Start () {

        yOffset = 2;
        transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
    }

    public void SetTopIcon(string iconId)
    {
        if (!string.IsNullOrEmpty(iconId))
        {
            EffIcon.gameObject.SetActive(true);
            LoadSprite.LoaderImage(EffIcon, "skill/" + iconId);
        }
        else
        {
            EffIcon.gameObject.SetActive(false);
        }
    }


    void Update()
    {
        if (null == Owner) return;

        if (!Owner.gameObject.activeSelf)
        {
            return;
        }

        if (null != Owner && gameObject.activeSelf)
        {
            if(null == m_matchItem && null != MatchManager.Instace)
                m_matchItem = MatchManager.GetPlayerItem(Owner);

            if (null != m_matchItem && null != m_matchItem.monsterCfg)
            {
                TxtName.text = m_matchItem.monsterCfg.name;
            }
            else if (null != m_matchItem && null != m_matchItem.player)
            {
                TxtName.text = m_matchItem.player.name;
            }
        }

        if(null != Camera.main)
            transform.rotation = Camera.main.transform.rotation;

        //Vector3 player2DPosition = Camera.main.WorldToScreenPoint(Owner.transform.position);
       // Vector3 player2DPosition = Owner.transform.position;
        //Vector3 player2DPosition = RectTransformUtility.WorldToScreenPoint(Main.UI3DCamera,transform.position);
        //player2DPosition = new Vector3(player2DPosition.x, (int)player2DPosition.y, player2DPosition.z);
        //player2DPosition.y = -373f;
        //recTransform.position = player2DPosition + new Vector3(xOffset, yOffset,0);
        //recTransform.position = player2DPosition;

        //Vector3 pos = recTransform.anchoredPosition3D;
        //pos.y = -371.8f;
        //recTransform.anchoredPosition3D = pos;

        Vector3 newP = Owner.transform.position + new Vector3(xOffset, yOffset, 0);
        newP.y = yOffset;
        transform.position = newP;
    }

    public void Clear()
    {
        GameObject.DestroyObject(gameObject);
    }
}


