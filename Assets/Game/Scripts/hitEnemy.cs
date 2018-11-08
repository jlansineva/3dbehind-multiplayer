using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitEnemy : MonoBehaviour {
    public int hp = 30;
    public int iframes = 60;
    public bool hit = false;
    public Animator anim;

    IEnumerator IframeCalc()
    {
        Debug.Log("asdasd");
        yield return new WaitUntil(() => iframes < 10);
        if (hp > 0)
        {
            hp -= 2;
            Debug.Log("HP: " + hp.ToString());
        }
        else Die();

        iframes = 60;
        hit = false;
    }

    public void Die()
    {
        anim.SetBool("Dead", true);
    }

    public void TakeHit()
    {
        StartCoroutine(IframeCalc());
        hit = true;
    }


	
	// Update is called once per frame
	void Update () {
        if (hit)
        {
            iframes--;
            Debug.Log(iframes);
        }

    }
}
