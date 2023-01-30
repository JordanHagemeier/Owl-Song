using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailTrigger : MonoBehaviour
{
    [SerializeField] float waitTimeBetweenChecks;
    [SerializeField] float waitTimeBetweenMove;
    private Animator snailAnimator;

    void Start(){
      snailAnimator = gameObject.GetComponent<Animator>();
      StartCoroutine(RandomMove());
    }

    public void OnTriggerEnter(Collider col){
        if (col.CompareTag("Player"))
        {
            snailAnimator.SetBool("snailHide", true);
        }
    }

    public void OnTriggerExit(Collider col){
        if (col.CompareTag("Player"))
        {
            snailAnimator.SetBool("snailHide", false);
        }
    }

    public void SnailMove(){
      gameObject.transform.Rotate(Vector3.up, Random.Range(-120, 120));
    }

    IEnumerator RandomMove(){
      bool generateRandom = true;

      while(generateRandom){
        int randomValue = Random.Range(0, 2);
        if(randomValue == 0){
          SnailMove();

          yield return new WaitForSeconds(waitTimeBetweenMove);
        }
            yield return new WaitForSeconds(waitTimeBetweenChecks);
      }
      yield return new WaitForEndOfFrame();
    }
}
