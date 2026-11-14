using UnityEngine;
using TMPro;
using DG;
using DG.Tweening;

public class WallScriptTimer : MonoBehaviour
{
    private float amountOfTime; // total amount of time player has
    private TextMeshProUGUI timerText; // text of the timer
    private float currentTimer; // current elapsed time state

    void Update()
    {
        int minutes = Mathf.FloorToInt(currentTimer / 60f);
        int seconds = Mathf.FloorToInt(currentTimer % 60f);

        // print to text
        timerText.text = $"{minutes:00} : {seconds:00}";

        // warning if  timer got less than 30 sec
        if (seconds < 30)
        {
            if (ColorUtility.TryParseHtmlString("#FF5733", out Color newColor))
            {
                timerText.color = newColor; 

                transform.DOScale(1.15f, 0.5f). // pulse anim
                SetLoops(-1, LoopType.Yoyo).
                SetEase(Ease.InOutSine);
            }
        }

    }
}