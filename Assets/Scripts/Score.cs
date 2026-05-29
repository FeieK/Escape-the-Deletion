using System;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public TextMeshProUGUI ftTimeText;
    public TextMeshProUGUI dfTimeText;
    public TextMeshProUGUI prTimeText;
    public GameObject how;

    private void Start()
    {
        SetTimeText(ftTimeText, GameManager.time);
        SetTimeText(prTimeText, GameManager.instance.saveData.prTime);

        int ftTick = GameManager.FinalTime.GetTick(GameManager.time);
        int prTick = GameManager.FinalTime.GetTick(GameManager.instance.saveData.prTime);
        int dfTick = Math.Abs(prTick - ftTick);

        if (GameManager.instance.saveData.hasPR)
        {
            if (ftTick < prTick) dfTimeText.color = Color.green;
            if (ftTick > prTick) dfTimeText.color = Color.red;
            if (ftTick == prTick)
            {
                dfTimeText.color = Color.orange;
                how.SetActive(true);
            }
            SetTimeText(dfTimeText, GameManager.FinalTime.GetTime(dfTick), ftTick < prTick ? "-" : ftTick > prTick ? "+" : "");
        }
        else
        {
            prTimeText.text = "No record";
            dfTimeText.transform.parent.gameObject.SetActive(false);
        }



        if (GameManager.FinalTime.IsNewRecord(GameManager.time, GameManager.instance.saveData.prTime) || !GameManager.instance.saveData.hasPR)
        {
            GameManager.instance.saveData.hasPR = true;
            GameManager.instance.saveData.prTime = GameManager.time;
            SaveSystem.Save();
        }
    }

    private void SetTimeText(TextMeshProUGUI textMesh, GameManager.FinalTime time, string op = "")
    {
        if (time == null)
        {
            return;
        }

        string milSec = time.minSecond.ToString();
        string sec = time.second.ToString();
        string min = time.minute.ToString();
        string hour = time.hour.ToString();
        if (time.minSecond < 100)
        {
            milSec = $"0{milSec}";
        }
        if (time.minSecond < 10)
        {
            milSec = $"0{milSec}";
        }
        if (time.second < 10)
        {
            sec = $"0{sec}";
        }
        if (time.minute < 10)
        {
            min = $"0{min}";
        }

        textMesh.text = $"{op}{hour}:{min}:{sec}:{milSec}";
    }

    public void Next()
    {
        GameManager.NextScene();
    }
}
