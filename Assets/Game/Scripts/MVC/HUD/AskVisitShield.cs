using UnityEngine;

public class AskVisitShield : MonoBehaviour
{
    public Sprite[] shields = new Sprite[10];

    // Use this for initialization
    private void Start()
    {
        string teamLocal = PlayerPrefs.GetString("Visit");
        if (teamLocal == string.Empty)
        {
            teamLocal = "Brazil";
        }
        else
        {
            foreach (Sprite shield in shields)
            {
                if (shield.name == teamLocal)
                {
                    GetComponent<SpriteRenderer>().sprite = shield;
                }
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}