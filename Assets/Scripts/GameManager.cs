using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] BattleManager battleManager;

    private SoundManager soundManager;

    private ObjectPoolManager poolManager;

    private VCamManager camManager;

    //true = oyun �al���yor
    public bool gameState, bossBattlestate, permission;         //bossManager scriptinden buraya ta��nd�.

    //oyun durdurdu�unda attack da pasif oluyor, oyunu tekrar ba�latt���nda e�er sava�ta ise tekrar aktif olmas� gerekiyor.
    public bool attackDebug;


    #region S�re de�i�kenleri   ek: kazan�nca, kalan prisoner say�s�
    private float elapsedTime = 0f;
    private int seconds = 0;
    private int minutes = 0;
    [SerializeField] TMP_Text TimeDisplayLose;
    [SerializeField] TMP_Text TimeDisplayWin;
    //karakter say�s�
    [SerializeField] Transform player;
    [SerializeField] TMP_Text prisonerCountTxt;
    #endregion



    #region Level Test
    [SerializeField] private LevelManager levelManager;
    private int currentLevelIndex;
    private GameObject currentLevel;
    #endregion



    private void Awake()
    {
        currentLevelIndex = PlayerPrefs.GetInt("CurrentLevel", currentLevelIndex);
        if (currentLevelIndex < 10)
            currentLevel = Instantiate(levelManager.levels[currentLevelIndex]);
        else
            currentLevel = Instantiate(levelManager.levels[Random.Range(0, 10)]);

    }



    public GameObject StartMenu, LoseMenu, WinMenu, StopMenuContents, StopButton, settings, contributedMenu, restartAllGameMenu;
    public TextMeshProUGUI[] levelTxt;
    private void Start()
    {
        gameState = false;
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        poolManager = GameObject.FindGameObjectWithTag("poolManager").GetComponent<ObjectPoolManager>();
        camManager = GameObject.FindGameObjectWithTag("vCam").GetComponent<VCamManager>();
        //start butonunu belirenen s�re sonra �a��r�r
        StartCoroutine(StartButtonActive());
    }

    private void Update()
    {
        if (gameState)
            TimeCounter();
    }


    #region butonlar
    public void StartButtonClicked()
    {
        StopButton.SetActive(true);
        gameState = true;
        battleManager.attackState = false;
        bossBattlestate = false;

        StartMenu.SetActive(!StartMenu.activeSelf);
        levelTxt[0].text = "Level " + (currentLevelIndex + 1).ToString();

    }

    public void SettingsOpen()
    {
        StopMenuContents.SetActive(false);
        settings.SetActive(true);
    }



    public void SettingsClose()
    {
        settings.SetActive(false);
        contributedMenu.SetActive(false);
        restartAllGameMenu.SetActive(false);
        StopMenuContents.SetActive(true);
        soundManager.SaveSound(soundManager.soundVolume);
        soundManager.SaveSong(soundManager.songVolume);
        PlayerPrefs.Save();
    }


    //************************************************ D�ZENLENECEK!
    public void RetryButtonClicked()
    {
        LoseMenu.SetActive(false);
        ResetAll();
        Destroy(currentLevel);
        if (currentLevelIndex < 10)
            currentLevel = Instantiate(levelManager.levels[currentLevelIndex]);
        else
            currentLevel = Instantiate(levelManager.levels[Random.Range(0, 10)]);
    }

    //************************************************************************************************************* D�ZENLENECEK!
    public void WinButtonClicked()
    {
        CreateNextLevel();          //e�er oyunu durdurup tekrar ba�latt���nda ayn� b�l�mden devam etmek istiyorsan bu metodu ResetAll metodun �st�ne koy   
        ResetAll();                 //e�er oyunu durdurup tekrar ba�latt���nda ge�mi� oldu�un b�l�mde kalmak istiyorsan bu metodu CreateNextLevel metodun �st�ne koy(ge�ti�in b�l�mde oyunu kaybedip tekrar dene dersen g.o .d)

    }
    public void StopButtonClicked()
    {
        //bunu kullanmaktan nefret ediyorum!
        Time.timeScale = 0f;

        StopButton.SetActive(false);
        attackDebug = battleManager.attackState;
        gameState = false;
        battleManager.attackState = false;
        StopMenuContents.SetActive(true);

    }
    public void StopMenuButtonClicked()
    {
        Time.timeScale = 1;
        StopMenuContents.SetActive(false);
        gameState = true;
        battleManager.attackState = attackDebug;
        StopButton.SetActive(true);
    }

    public void LoseMenuActivity() // Oyuncu �ld���nde Lose menusunu a�mak i�in
    {
        soundManager.LoseSound();
        levelTxt[2].text = "Level " + (currentLevelIndex + 1).ToString();
        TimeDisplayLose.text = (minutes.ToString("00") + ":" + seconds.ToString("00"));
        StopButton.SetActive(false);
        gameState = false;
        LoseMenu.SetActive(true);
    }

    public void ContributedButtonClicked()
    {
        StopMenuContents.SetActive(false);
        contributedMenu.SetActive(true);
    }

    public void RestartYesButtonClicked()
    {
        currentLevelIndex = 0;
        Time.timeScale = 1;
        StopMenuContents.SetActive(false);
        restartAllGameMenu.SetActive(false);
        ResetAll();
        Destroy(currentLevel);
        currentLevel = Instantiate(levelManager.levels[currentLevelIndex]);
    }
    public void RestartGameMenuActivity()
    {
        restartAllGameMenu.SetActive(true);
        StopMenuContents.SetActive(false);
    }


    #endregion


    public IEnumerator GameWin()
    {
        levelTxt[1].text = "Level " + (currentLevelIndex + 1).ToString() + " Completed";
        TimeDisplayWin.text = (minutes.ToString("00") + ":" + seconds.ToString("00"));
        prisonerCountTxt.text = "Survivors: " + (player.childCount - 1).ToString();
        gameState = false;
        yield return new WaitForSeconds(0.5f);
        StopButton.SetActive(false);
        WinMenu.SetActive(true);
        soundManager.WinSound();
    }

    #region yeni seviyeye ge�i�
    //yeni seviyeyi olu�turur ve �nceki seviyeyi destroy eder.
    void CreateNextLevel()
    {
        Destroy(currentLevel);

        currentLevelIndex++;

        if (currentLevelIndex < levelManager.levels.Length)
            currentLevel = Instantiate(levelManager.levels[currentLevelIndex]);
        else
        {
            currentLevel = Instantiate(levelManager.randomLevels[Random.Range(0, 10)]);
        }


    }

    //her �eyi s�f�rlar(next butona bas�ld���nda)
    void ResetAll()
    {
        WinMenu.SetActive(false);
        StartCoroutine(StartButtonActive());
        while (player.childCount > 1)
            poolManager.GiveBlueStickman(player.GetChild(1).gameObject);
        poolManager.GetBlueStickman();
        player.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = (player.childCount - 1).ToString();
        player.position = new Vector3(0, 0.5f, 0);
        player.GetChild(0).gameObject.SetActive(true);
        player.GetChild(0).position = new Vector3(player.position.x, player.GetChild(0).position.y, player.position.z);
        permission = false;
        gameState = false;
        battleManager.attackState = false;
        Time.timeScale = 1;
        minutes = 0;
        seconds = 0;
        camManager.GetCameraPos();
        PlayerPrefs.SetInt("CurrentLevel", currentLevelIndex);
        PlayerPrefs.Save();
    }

    #endregion

    //zaman sayac�
    private void TimeCounter()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= 1f)
        {
            elapsedTime = 0f;
            seconds++;

            if (seconds >= 60)
            {
                seconds = 0;
                minutes++;
            }
        }
    }


    IEnumerator StartButtonActive()
    {
        yield return null;      //kameran�n playere gelme s�resiyle e�it yap�n!!
        StartMenu.SetActive(true);
    }


}