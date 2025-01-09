using UnityEngine;
using TMPro;

public class Ranking : MonoBehaviour
{
    [SerializeField] private TMP_Text rank;
    [SerializeField] private TMP_Text score;
    [SerializeField] private TMP_Text playerName;

    private void Start()
    {
        transform.localScale = Vector3.one;
    }
    public void InitRank(User userDetails,int leaderBoardRank)
    {
        rank.SetText(leaderBoardRank.ToString());
        score.SetText(userDetails.score.ToString());
        playerName.SetText(userDetails.username.ToString());
        transform.localScale = Vector3.one;
    }
}
