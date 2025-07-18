using System.Collections;
using UnityEngine;

public class PCharManager : MonoBehaviour
{
    private BattleManager battleManager;
    private PlayerManager playerManager;
    private ObjectPoolManager poolManager;
    private BossManager bossManager;
    private GameManager gameManager;


    bool waitAttack;


    private void Start()
    {
        battleManager = GameObject.FindGameObjectWithTag("battleManager").GetComponent<BattleManager>();
        playerManager = transform.parent.GetComponent<PlayerManager>();
        poolManager = GameObject.FindGameObjectWithTag("poolManager").GetComponent<ObjectPoolManager>();
        gameManager = GameObject.FindGameObjectWithTag("gameManager").GetComponent<GameManager>();

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("red") && battleManager.attackState && other.transform.parent.childCount > 1)
        {
            battleManager.KillTheRed(other.gameObject);

            playerManager.TextUpdate();

            poolManager.BlueParticleActivate(transform);
        }


    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Boss") && gameManager.gameState)
        {
            if (waitAttack)
                return;
            bossManager = other.GetComponent<BossManager>();
            StartCoroutine(BossAttack());
            waitAttack = true;
        }
    }


    private IEnumerator BossAttack()
    {
        if (transform.parent.childCount > 5)
        {
            bossManager.BossDamage(0.2f);
            bossManager.bossTxt.text = ((int)bossManager.bossHealth + 1).ToString();
            yield return new WaitForSeconds(0.5f);
            waitAttack = false;
        }
        else
        {
            bossManager.BossDamage(1);
            bossManager.bossTxt.text = ((int)bossManager.bossHealth).ToString();
            yield return new WaitForSeconds(0.75f);
            waitAttack = false;
        }
    }
}

