using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbellController : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerController.instance.IncreaseDumblleScore();
            PlayerController.instance.DumbellAnimationStart();
            Destroy(this.gameObject);
            
        }
        else if (other.CompareTag("AI"))
        {
            other.gameObject.GetComponent<AIController>().IncreaseDumblleScore();
            other.gameObject.GetComponent<AIController>().DumbellAnimationStart();
            Destroy(this.gameObject);
        }

        GameManager.instance.activeDumbell--;
    }

}
