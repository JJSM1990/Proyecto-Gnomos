using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag=="Player")
        {
            var player = other.gameObject.GetComponent<PlayerControl>();
            GameManager.Instance.updateCheckpoint(this.gameObject, player.ReturnTotalGnomeFollowers());
        }
    }
}
