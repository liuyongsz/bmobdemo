using UnityEngine;
/// <summary>
/// 旋转选择器
/// </summary>
public class RotateSelector : MonoBehaviour
{
    /// <summary>设置目标人(球员)</summary>
    public Player player;
	
    private CoreGame coreGame;

    private void Start()
    {
        this.coreGame = GameObject.FindObjectOfType(typeof(CoreGame)) as CoreGame;

        AllRef.RotateSelector = this;
    }

    public void ResetMatch()
    {
        player = null;
        transform.position = Define.NullPos;
    }

    private void LateUpdate()
    {
        if (null == player || null == AllRef.Ball)
            return;

        Vector3 lookPos = AllRef.Ball.transform.position;
        lookPos.y = 0.1f;

        transform.LookAt(lookPos);
        Vector3 angle = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(90f, angle.y,180 + angle.z);

        UpdatePosition();
    }

    public void UpdatePosition()
    {
        if(null != player && player.gameObject.activeSelf)
        {
            Vector3 vec = player.transform.position.Clone();
            vec.y = 0.1f;
            transform.position = vec;
        }
        else
        {
            transform.position = Define.NullPos;
        }
    }
}