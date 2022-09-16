using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public UIManager uiManager;
    public EnemyManager enemyManager;

    enum EnemyDecisionStates
    {
        Waiting,
        Move,
        Attack,
        Flee,
        Heal,
    };

    public void EnemyKilled()
    {
        GetComponent<Data>().isDead = true;
        uiManager.EnemyDied(gameObject);
    }

    public void BeginTurn()
    {
        EnemyDecisionStates decision = EnemyDecisionStates.Move;

        switch (decision)
        {
            case EnemyDecisionStates.Attack:

                break;
            case EnemyDecisionStates.Move:
                MoveState();
                break;
        }

        enemyManager.NextEnemyTurn();
    }

    void MoveState()
    {
        transform.position = new Vector3(transform.position.x + (Random.Range(-2, 3) * 2), transform.position.y, transform.position.z + (Random.Range(-2, 3) * 2));
    }
}
