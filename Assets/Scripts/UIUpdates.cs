using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpdates : MonoBehaviour
{
    [SerializeField] private Button joinBtn;
    [SerializeField] private Image joinButtonBGIMG;
    [SerializeField] private TMP_Text findingBallsTMP;
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private GameObject ballControlls;

    public void MatchMakingUIUpdate(bool isActive)
    {
        findingBallsTMP.gameObject.SetActive(isActive);
    }

    public void JoinButtonUIUpdate(bool isActive)
    {
        joinBtn.gameObject.SetActive(isActive);
        joinButtonBGIMG.gameObject.SetActive(isActive);
    }

    public void TryAgainButtonUIUpdate(bool isActive)
    {
        tryAgainButton.gameObject.SetActive(isActive);
    }

    public void BallControllsUIUpdate(bool isActive)
    {
        ballControlls.SetActive(isActive);
    }
}
