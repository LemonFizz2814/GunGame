using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public UIManager uiManager;

    public void EnemyKilled()
    {
        GetComponent<Data>().isDead = true;
        uiManager.EnemyDied(gameObject);
    }
}
