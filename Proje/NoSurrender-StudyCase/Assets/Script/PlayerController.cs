using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    #region singleton

    public static PlayerController instance;
    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
    }

    #endregion

    private Animator anim;
    private AudioSource As;
    [HideInInspector]
    public Rigidbody physic;
    [HideInInspector]
    public Dictionary<string,Rigidbody> AIsRigidbodies;
    [HideInInspector]
    public Dictionary<string,AIController> AIsControllers;
    public FloatingJoystick PlayerJoyStick;
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;

    [SerializeField]
    private GameObject collisionEffect;
    [SerializeField]
    private GameObject DeadPanel;
    [SerializeField]
    private GameObject WinPanel;
    [SerializeField]
    private GameObject AIsRoot;
    [SerializeField]
    private GameObject WinnersRoot;

    [SerializeField]
    AudioClip[] punchSounds;

    [SerializeField]
    private TMP_Text scoreTextDead;
    [SerializeField]
    private TMP_Text scoreTextWin;
    [SerializeField]
    private TMP_Text scoreTextPlaytime;
    [SerializeField]
    private TMP_Text leftTimeTextDead;
    [SerializeField]
    private TMP_Text leftTimeTextWin;

    public int DumblleScore;

    private float growingTimer = 0;
    private float growingTimeStep = 60;
    private float RunningSpeed = 100;
    private float RotationSpeed = 50;
    private float sensivityY = .1f;
    private float CurrentY;

    [HideInInspector]
    public bool isDead;
    [HideInInspector]
    public bool gameEnd;
    private bool pushable;
    void Start()
    {
        anim = GetComponent<Animator>();
        physic = GetComponent<Rigidbody>();
        As = GetComponent<AudioSource>();

        DumblleScore = 0;
        isDead = false;
        gameEnd = false;
        pushable = false;

        GetAIsRigidbodies();
    }

    private void Update()
    {
        if (GameManager.instance.GameStarted)
        {
            CurrentY += PlayerJoyStick.Horizontal * sensivityY / 10;

           if(growingTimer < growingTimeStep)
           {
                GrowUp();
                
           }
           growingTimer +=  1;


           scoreTextPlaytime.text = DumblleScore.ToString();


           if(AIsRoot.transform.childCount == 0)
            {
                Win();
            }

           if (gameEnd)
           {
                anim.SetBool("GameEnd", gameEnd);
                if(!isDead)
                    virtualCamera.enabled = false;
                physic.constraints = RigidbodyConstraints.FreezeAll;

                for(int i = 0; i < WinnersRoot.transform.childCount; i++)
                {
                    WinnersRoot.transform.GetChild(i).GetComponent<TMP_Text>().text = " ";

                    if(i == 0)
                        WinnersRoot.transform.GetChild(i).GetComponent<TMP_Text>().text = this.gameObject.name;
                    else
                    {
                        if (AIsRoot.transform.GetChild(i - 1).gameObject != null)
                            WinnersRoot.transform.GetChild(i).GetComponent<TMP_Text>().text = AIsRoot.transform.GetChild(i - 1).gameObject.name;
                    }
                }



           }


           
           
        }
        
    }

    void FixedUpdate()
    {
        if (GameManager.instance.GameStarted && !gameEnd)
        {
            anim.SetBool("GameStarted", true);

            Run();
            Rotate();

           

        }
        else
        {

        }
    }

    private void GetAIsRigidbodies()
    {
        AIsRigidbodies = new Dictionary<string, Rigidbody>();
        AIsControllers = new Dictionary<string, AIController>();

        for (int i = 0; i < AIsRoot.transform.childCount; i++)
        {
            AIsRigidbodies[AIsRoot.transform.GetChild(i).gameObject.name] = AIsRoot.transform.GetChild(i).GetComponent<Rigidbody>();
            AIsControllers[AIsRoot.transform.GetChild(i).gameObject.name] = AIsRoot.transform.GetChild(i).GetComponent<AIController>();
        }
    }

    private void Run()
    {

        if (!gameEnd)
        {

            transform.Translate(new Vector3(0, 0, Time.deltaTime * 2));

        }
    }

    private void Rotate()
    {
        Quaternion rotationPlayer = Quaternion.Euler(0, CurrentY * RotationSpeed, 0);
        transform.localRotation = rotationPlayer;
    }

    public void Dead()
    {
        scoreTextDead.text = DumblleScore.ToString();
        leftTimeTextDead.text = GameManager.instance.GameTimer.ToString("0.0");
        PlayerJoyStick.gameObject.SetActive(false);
        virtualCamera.Follow = null;
        virtualCamera.LookAt = null;

        DeadPanel.SetActive(true);
        isDead = true;
        gameEnd = true;
    }

    public void Win()
    {
        scoreTextWin.text = DumblleScore.ToString();
        leftTimeTextWin.text = GameManager.instance.GameTimer.ToString("0.0");
        PlayerJoyStick.gameObject.SetActive(false);

        WinPanel.SetActive(true);
        gameEnd = true;

        for(int i = 0; i < AIsRoot.transform.childCount; i++)
        {
            if(AIsRoot.transform.GetChild(i) != null)
            {
                AIsRoot.transform.GetChild(i).GetComponent<AIController>().anim.SetBool("GameEnd", gameEnd);
            }
        }
    }

   

    public void IncreaseDumblleScore()
    {
        DumblleScore++;
        growingTimer = 0;

    }

    private void GrowUp()
    {
        transform.localScale = new Vector3(Mathf.Lerp(transform.localScale.x, transform.localScale.x + (DumblleScore * 0.003f), 0.05f),
                                            Mathf.Lerp(transform.localScale.y, transform.localScale.y + (DumblleScore * 0.003f), 0.05f),
                                            Mathf.Lerp(transform.localScale.z, transform.localScale.z + (DumblleScore * 0.003f), 0.05f));
    }

   

    public void DumbellAnimationStart()
    {
        transform.GetChild(9).gameObject.SetActive(true);
        anim.SetTrigger("TakeDumbell");
    }

    public void DumbellAnimationEnd()
    {
        transform.GetChild(9).gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("PushTriggerAI"))
        {
            Rigidbody _push = AIsRigidbodies[other.transform.parent.name];
            AIController _ai = AIsControllers[other.transform.parent.name];


            As.PlayOneShot(punchSounds[Random.Range(0, 1)]);

            GameObject _collisionEffect = Instantiate(collisionEffect);
            _collisionEffect.transform.position = other.transform.parent.GetChild(4).position;

            Destroy(_collisionEffect, 2f);


            if (_ai.DumblleScore <= DumblleScore)
            {
                _push.AddForce(transform.forward * (1000 + ((DumblleScore - _ai.DumblleScore) * 2)), ForceMode.Impulse);
                
            }
        }

        
    }

}
