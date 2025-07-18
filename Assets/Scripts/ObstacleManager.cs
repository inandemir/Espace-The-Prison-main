using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerManager playerManager;
    private BattleManager battleManager;
    private ObjectPoolManager poolManager;
    private SoundManager soundManager;


    bool formatStickmanBool = true;


    private void Start()
    {
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        playerManager = GameObject.FindGameObjectWithTag("player").GetComponent<PlayerManager>();
        battleManager = GameObject.FindGameObjectWithTag("battleManager").GetComponent<BattleManager>();
        poolManager = GameObject.FindGameObjectWithTag("poolManager").GetComponent<ObjectPoolManager>();
        gameManager = GameObject.FindGameObjectWithTag("gameManager").GetComponent<GameManager>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("blue"))
        {
            if (gameManager.gameState)
            {
                playerManager.TextUpdate();
                battleManager.KillTheBlue(other.gameObject);
                soundManager.BattleSound();
                poolManager.BlueParticleActivate(other.transform);

                if (playerManager.transform.childCount < 2)         //obstacle tarafýndan öldürüldüðünde
                {
                    playerManager.transform.GetChild(0).gameObject.SetActive(false);
                    gameManager.LoseMenuActivity();
                }

                if (formatStickmanBool)
                {
                    StartCoroutine(FormatStickman());
                    formatStickmanBool = false;
                }
            }
        }
    }

    IEnumerator FormatStickman()
    {
        yield return new WaitForSeconds(1.2f);
        playerManager.FormatStickMan();
        yield return new WaitForSeconds(1f);
        formatStickmanBool = true;
    }
}
