using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogTrigger : MonoBehaviour
{
    [SerializeField] float waitTimeBetweenChecks;
    [SerializeField] float waitTimeBetweenJumps;
    private Animator frogAnimator;

    void Start(){
      frogAnimator = gameObject.GetComponent<Animator>();
      StartCoroutine(RandomJump());
    }

    public void OnTriggerEnter(Collider col){
        if (col.CompareTag("Player"))
        {
            FrogJumping();
        }

    }

    public void FrogJumping(){
      gameObject.transform.Rotate(Vector3.up, Random.Range(-120, 120));
        frogAnimator.SetTrigger("frogJumping");
    }

    IEnumerator RandomJump(){
      bool generateRandom = true;

      while(generateRandom){
        int randomValue = Random.Range(0, 2);
        if(randomValue == 0){
          FrogJumping();

          yield return new WaitForSeconds(waitTimeBetweenJumps);
        }
            yield return new WaitForSeconds(waitTimeBetweenChecks);
      }
      yield return new WaitForEndOfFrame();
    }
}
