using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECharManager : MonoBehaviour
{
    private EnemyManager enemyManager;

    private BattleManager battleManager;

    private ObjectPoolManager poolManager;

    private void Start()
    {
        battleManager = GameObject.FindGameObjectWithTag("battleManager").GetComponent<BattleManager>();
        enemyManager = GetComponentInParent<EnemyManager>();
        poolManager = GameObject.FindGameObjectWithTag("poolManager").GetComponent<ObjectPoolManager>();

    }

    private void Update()
    {
        enemyManager.EnemyAnimation(transform);
    }

    #region düþmanlar karakterlerimizi öldürür
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("blue") && battleManager.attackState && other.transform.parent.childCount > 1)
        {
            enemyManager.TextUpdate();

            battleManager.KillTheBlue(other.gameObject);

            poolManager.RedParticleActivate(transform);


        }
    }
    #endregion

}
