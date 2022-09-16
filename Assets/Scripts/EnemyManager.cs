using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject[] enemiesAlive;
    int enemiesTurn;

    public void StartEnemiesTurns()
    {
        enemiesTurn = -1;

        NextEnemyTurn();

        /*foreach (GameObject enemy in enemiesAlive)
        {
            EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
            enemyScript.BeginTurn();
        }*/
    }

    public void NextEnemyTurn()
    {
        enemiesTurn++;
        if (enemiesTurn >= enemiesAlive.Length)
        {
            enemiesAlive[enemiesTurn].GetComponent<EnemyScript>().BeginTurn();
        }
        else
        {
            print("end of enemies turns");
        }
    }
}
