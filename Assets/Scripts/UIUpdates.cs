using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpdates : MonoBehaviour
{
    [SerializeField] private Button joinBtn;
    [SerializeField] private TMP_Text matchMakingTMP;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMP_Text winStatusText;

    public void ShowGameOverUI(bool isWinner)
    {
        winPanel.SetActive(true);
        winStatusText.text = isWinner ? "YOU WON" : "YOU LOSE";
        winStatusText.color = isWinner ? Color.green : Color.red;
    }

    public void MatchMakingUIUpdate(bool isActive)
    {
        matchMakingTMP.gameObject.SetActive(isActive);
    }

    public void JoinButtonUIUpdate(bool isActive)
    {
        joinBtn.gameObject.SetActive(isActive);
    }
}
