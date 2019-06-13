using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopMenu : MonoBehaviour
{

    public enum ShopPrices 
    {
        song_0_1 = 500, song_1_0 = 1000, song_1_1 = 1500
    }

    public TextMeshProUGUI currentStarsText;

    public bool infiniteStars;
    public Button purchaseButton00;
    public Button purchaseButton01;
    public Button purchaseButton10;
    public Button purchaseButton11;

    public AudioSource soundtrack;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        if(infiniteStars)
            PlayerPrefs.DeleteAll();
        
        UpdateButtons();
        UpdateStars(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    int UpdateStars(bool setInfinite)
    {
        int currentStars;
        if(infiniteStars && setInfinite)
            PlayerPrefs.SetInt("starsTotal", 10000);

        currentStars = PlayerPrefs.GetInt("starsTotal", 0);
        currentStarsText.text = "Current stars: " + currentStars;
        return currentStars;
    }

    void UpdateButtons()
    {
        
        purchaseButton00.GetComponentInChildren<TextMeshProUGUI>().text = "Set";

        if(PlayerPrefs.GetInt("song_0_1") != 0)
            purchaseButton01.GetComponentInChildren<TextMeshProUGUI>().text = "Set";
        else
            purchaseButton01.GetComponentInChildren<TextMeshProUGUI>().text = "Purchase (" + (int)ShopPrices.song_0_1 + ")";

        if(PlayerPrefs.GetInt("song_1_0") != 0)
            purchaseButton10.GetComponentInChildren<TextMeshProUGUI>().text = "Set";
        else
            purchaseButton10.GetComponentInChildren<TextMeshProUGUI>().text = "Purchase (" + (int)ShopPrices.song_1_0 + ")";

        if(PlayerPrefs.GetInt("song_1_1") != 0)
            purchaseButton11.GetComponentInChildren<TextMeshProUGUI>().text = "Set";
        else
            purchaseButton11.GetComponentInChildren<TextMeshProUGUI>().text = "Purchase (" + (int)ShopPrices.song_1_1 + ")";

        switch(PlayerPrefs.GetString("currentSong","song_0_0"))
        {
            case "song_0_0":
                purchaseButton00.GetComponentInChildren<TextMeshProUGUI>().text = "Currently set";
            break;
            case "song_0_1":
                purchaseButton01.GetComponentInChildren<TextMeshProUGUI>().text = "Currently set";
            break;
            case "song_1_0":
                purchaseButton10.GetComponentInChildren<TextMeshProUGUI>().text = "Currently set";
            break;
            case "song_1_1":
                purchaseButton11.GetComponentInChildren<TextMeshProUGUI>().text = "Currently set";
            break;
        }
    }

    public void SetSong(int buttonId)
    {
        switch(buttonId)
        {
            case 0:
                PlayerPrefs.SetString("currentSong","song_0_0");
            break;
            case 1:
            if(PlayerPrefs.GetInt("song_0_1",0) != 0)
                PlayerPrefs.SetString("currentSong","song_0_1");
            break;
            case 2:
            if(PlayerPrefs.GetInt("song_1_0",0) != 0)
                PlayerPrefs.SetString("currentSong","song_1_0");
            break;
            case 3:
            if(PlayerPrefs.GetInt("song_1_1",0) != 0)
                PlayerPrefs.SetString("currentSong","song_1_1");
            break;
        }
        UpdateButtons();
    }

    public void PurchaseSong(int buttonId)
    {
        int currentStars = UpdateStars(false);
        
        switch(buttonId)
        {
            case 1:
            if(currentStars >= (int)ShopPrices.song_0_1 && PlayerPrefs.GetInt("song_0_1",0) == 0)
            {
                PlayerPrefs.SetInt("starsTotal", currentStars - (int)ShopPrices.song_0_1);
                PlayerPrefs.SetInt("song_0_1",1);
            }
            break;
            case 2:
            if(currentStars >= (int)ShopPrices.song_1_0 && PlayerPrefs.GetInt("song_1_0",0) == 0)
            {
                PlayerPrefs.SetInt("starsTotal", currentStars - (int)ShopPrices.song_1_0);
                PlayerPrefs.SetInt("song_1_0",1);
            }
            break;
            case 3:
            if(currentStars >= (int)ShopPrices.song_1_1 && PlayerPrefs.GetInt("song_1_1",0) == 0)
            {
                PlayerPrefs.SetInt("starsTotal", currentStars - (int)ShopPrices.song_1_1);
                PlayerPrefs.SetInt("song_1_1",1);
            }
            break;
        }

        UpdateStars(false);
        UpdateButtons();
    }

    public void PlaySong(int id)
    {
        switch (id)
        {
            case 0:
                soundtrack.time = 0;
                break;
            case 1:
                soundtrack.time = 64;
                break;
            case 2:
                soundtrack.time = 373;
                break;
            case 3:
                soundtrack.time = 550;
                break;
        }
        if(!soundtrack.isPlaying)
            soundtrack.Play();
        
    }

    public void StopSong()
    {
        soundtrack.Stop();
    }
}
