using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{

    [HideInInspector]
    public Animator anim;
    private AudioSource As;
    [HideInInspector]
    public Rigidbody physic;
    //[HideInInspector]
   // public NavMeshAgent agent;
    [SerializeField]
    private GameObject collisionEffect;
    [SerializeField]
    private GameObject AIsRoot;
    [SerializeField]
    private GameObject DumbellsRoot;
    [SerializeField]
    AudioClip[] punchSounds;


    private Transform target;

    public int DumblleScore;

    private float targetCalculatingTimer = 0;
    private float targetCalculatingTimeStep = 5;
    private float growingTimer = 0;
    private float growingTimeStep = 60;
    private float RunningSpeed = 100;

    private bool InPlatform;

    void Start()
    {
        anim = GetComponent<Animator>();
        physic = GetComponent<Rigidbody>();
        As = GetComponent<AudioSource>();
       // agent = GetComponent<NavMeshAgent>();
        DumblleScore = 0;
        InPlatform = true;
        target = DumbellsRoot.transform.GetChild(Random.Range(0, DumbellsRoot.transform.childCount));
    }

    
    void Update()
    {
        if (GameManager.instance.GameStarted)
        {
            anim.SetBool("GameStarted", true);

            targetCalculatingTimer += Time.deltaTime;
            if (targetCalculatingTimer >= targetCalculatingTimeStep || Vector3.Distance(transform.position, target.position) < 1)
            {
                CalculateTarget();
                targetCalculatingTimer = 0;
            }

            if (!PlayerController.instance.gameEnd)
                Movement();
            else
                physic.constraints = RigidbodyConstraints.FreezeAll;

            if (growingTimer < growingTimeStep)
            {
                GrowUp();

            }
            growingTimer += 1;

        }
    
    }

    private void Movement()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), 3 * Time.deltaTime);
        transform.Translate(new Vector3(0, 0, Time.deltaTime * 2));
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

    public void CalculateTarget()
    {
        target = DumbellsRoot.transform.GetChild(Random.Range(0, DumbellsRoot.transform.childCount));
        
    }


    public void DumbellAnimationStart()
    {
        transform.GetChild(5).gameObject.SetActive(true);
        anim.SetTrigger("TakeDumbell");
    }

    public void DumbellAnimationEnd()
    {
        transform.GetChild(5).gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PushTriggerPlayer"))
        {
            Rigidbody _push = PlayerController.instance.physic;
            
            As.PlayOneShot(punchSounds[Random.Range(0, 1)]);

            GameObject _collisionEffect = Instantiate(collisionEffect);
            _collisionEffect.transform.position = other.transform.parent.GetChild(8).position;

            Destroy(_collisionEffect, 2f);


            if (PlayerController.instance.DumblleScore <= DumblleScore)
            {
                _push.AddForce(transform.forward * (1000 + ((DumblleScore - PlayerController.instance.DumblleScore) * 2)), ForceMode.Impulse);

            }
        }
        else if (other.gameObject.CompareTag("PushTriggerAI") && other.gameObject.transform.parent != this.transform)
        {
            Rigidbody _push = PlayerController.instance.AIsRigidbodies[other.transform.parent.name];
            AIController _ai = PlayerController.instance.AIsControllers[other.transform.parent.name];


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
