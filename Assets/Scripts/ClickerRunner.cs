using System.Collections;
using uClicker;
using UnityEngine;

public class ClickerRunner : MonoBehaviour
{
    public ClickerManager Manager;

    // Use this for initialization
    IEnumerator Start()
    {
        Manager.LoadProgress();
        Manager.StartBuyMax(); 
        while (Application.isPlaying)
        {
            yield return new WaitForSecondsRealtime(1);
            //Debug.Log("tik");
            Manager.Tick();
            Manager.SaveProgress();
            PlayerPrefs.Save();
        }
    }

    private void OnDestroy()
    {
        Manager.SaveProgress();
    }
}