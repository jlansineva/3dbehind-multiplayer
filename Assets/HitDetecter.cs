using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetecter : MonoBehaviour {
    List<GameObject> hitEnemy = new List<GameObject>();
    
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "enemy") hitEnemy.Add(col.gameObject);
    }

        void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "enemy") hitEnemy.Remove(col.gameObject);
    }

    void Update()
    {
        foreach (var enemy in hitEnemy)
        {
            enemy.BroadcastMessage("TakeHit");
        }
    }
}
