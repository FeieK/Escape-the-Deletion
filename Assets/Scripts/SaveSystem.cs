using UnityEngine;

public class SaveSystem
{
    public GameManager.FinalTime prTime;
    public bool hasPR = false;

    public SaveSystem()
    {
        this.prTime = new GameManager.FinalTime();
        this.hasPR = false;
    }
    public SaveSystem(GameManager.FinalTime prTime, bool hasPR)
    {
        this.prTime = prTime;
        this.hasPR = hasPR;
    }

    public static void Save()
    {
        SaveSystem data = GameManager.instance.saveData;
        int tick = GameManager.FinalTime.GetTick(data.prTime);
        int hasTime = data.hasPR ? 1 : 0;

        PlayerPrefs.SetInt("prTick", tick);
        PlayerPrefs.SetInt("hasTime", hasTime);
    }
    public static void Load()
    {
        if (PlayerPrefs.HasKey("prTick") && PlayerPrefs.HasKey("hasTime"))
        {
            int tick = PlayerPrefs.GetInt("prTick");
            int hasTime = PlayerPrefs.GetInt("hasTime");

            SaveSystem data = new SaveSystem(GameManager.FinalTime.GetTime(tick), hasTime == 1);
            GameManager.instance.saveData = data;
        }
        else
        {
            SaveSystem data = new SaveSystem();
            GameManager.instance.saveData = data;
        }
    }

    public static void Delete()
    {
        PlayerPrefs.DeleteKey("prTick");
        PlayerPrefs.DeleteKey("hasTime");
    }
}