using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region singleton

    public static GameManager instance;
    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
    }

    #endregion

    public bool GameStarted;

    [SerializeField]
    private GameObject MainPanel;
    [SerializeField]
    private GameObject PausedPanel;
    [SerializeField]
    private GameObject PauseBtn;
    [SerializeField]
    private GameObject DumbellsRoot;
    [SerializeField]
    private GameObject DumbellOriginal;
    [SerializeField]
    private TMP_Text timeText;

    [HideInInspector]
    public int activeDumbell;
    
    public float GameTimer = 15;

    private float dumbellCreateTimer = 0;
    private float dumbellCreateTimeStep = 5;

    void Start()
    {
        GameStarted = false;

        activeDumbell = 0;

        CreateDumbell();
    }

    
    void Update()
    {

        if (GameStarted && !PlayerController.instance.isDead)
        {
            if (GameTimer > 0)
                GameTimer -= Time.deltaTime;
            else if(GameTimer <= 0)
            {
                GameTimer = 0;
                if (!PlayerController.instance.isDead)
                    PlayerController.instance.Win();
            }
            timeText.text = GameTimer.ToString("0.0");
            if (GameTimer < 10)
                timeText.color = Color.red;

            if (PlayerController.instance.gameEnd)
                PauseBtn.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (GameStarted)
        {
            dumbellCreateTimer += Time.deltaTime;

            if(dumbellCreateTimer >= dumbellCreateTimeStep)
            {
                CreateDumbellRuntime();
                dumbellCreateTimer = 0;
            }
        }
    }

    public void StartGame()
    {
        GameStarted = true;
        MainPanel.SetActive(false);
        PauseBtn.SetActive(true);
        PlayerController.instance.PlayerJoyStick.gameObject.SetActive(true);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        PausedPanel.SetActive(true);
        PauseBtn.SetActive(false);
    }
    public void ContinueGame()
    {
        Time.timeScale = 1;
        PausedPanel.SetActive(false);
        PauseBtn.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

   

    private void CreateDumbell()
    {
        for (int i = 0; i < DumbellsRoot.transform.childCount; i++)
        {
            if (DumbellsRoot.transform.GetChild(i).childCount == 0)
            {
                GameObject _dumbell = Instantiate(DumbellOriginal, DumbellsRoot.transform.GetChild(i));
                _dumbell.transform.localPosition = new Vector3(0, 0, 0);
                _dumbell.transform.localRotation = Quaternion.Euler(0, 0, 0);
                _dumbell.transform.localScale = new Vector3(1, 1, 1);

                activeDumbell++;
            }
        }
    }

    private void CreateDumbellRuntime()
    {
        for (int i = 0; i < DumbellsRoot.transform.childCount; i++)
        {
            if (DumbellsRoot.transform.GetChild(i).childCount == 0)
            {
                GameObject _dumbell = Instantiate(DumbellOriginal, DumbellsRoot.transform.GetChild(i));
                _dumbell.transform.localPosition = new Vector3(0, 0, 0);
                _dumbell.transform.localRotation = Quaternion.Euler(0, 0, 0);
                _dumbell.transform.localScale = new Vector3(1, 1, 1);

                activeDumbell++;

                break;
            }
        }
    }
}
