using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    ObjectPoolManager poolManager; PlayerManager playerManager; BattleManager battleManager; SoundManager soundManager; GameManager gameManager;

    private void Start()
    {
        poolManager = GameObject.FindGameObjectWithTag("poolManager").GetComponent<ObjectPoolManager>();
        playerManager = GameObject.FindGameObjectWithTag("player").GetComponent<PlayerManager>();
        battleManager = GameObject.FindGameObjectWithTag("battleManager").GetComponent<BattleManager>();
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        gameManager = GameObject.FindGameObjectWithTag("gameManager").GetComponent<GameManager>();
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player") && gameManager.gameState)
        {
            //3 tane karakterimizi öldürür
            for (int i = 0; i < 5; i++)
            {
                battleManager.KillTheBlue(other.transform.GetChild(1).gameObject);
                soundManager.BossAttackSound();
                poolManager.BlueParticleActivate(other.transform);

                if (other.transform.childCount < 2)
                {
                    playerManager.transform.GetChild(0).gameObject.SetActive(false);
                    gameManager.LoseMenuActivity();
                    return;
                }
                playerManager.TextUpdate();
                // playerManager.FormatStickMan();
            }

        }
    }
}