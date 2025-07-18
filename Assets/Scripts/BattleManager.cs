using Unity.Mathematics;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    // bu scriptteki player yazan her transform playerManager scriptine ait olan playerin transformu


    private SoundManager soundManager;
    [SerializeField] PlayerManager playerManager;
    [SerializeField] GameManager gameManager;
    [SerializeField] ObjectPoolManager objectPoolManager;

    #region atak kontrol
    public bool attackState;
    #endregion

    private Vector3 playerDistance;
    private Vector3 enemyDistance;



    private void Start()
    {
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    }

    #region karakterler d��manlar� takip eder ve e�er d��man kalmad�ysa yap�lacak i�lemler
    public void PlayerOffence(Transform enemy, Transform player)
    {
        //sava� yoksa di�er komutlar� �al��t�rmaz
        if (!attackState)
            return;

        //sava� ba�lad�ysa playerler enemy i�ine girer
        if (enemy.childCount > 1)
            for (int i = 1; i < player.childCount; i++)
            {
                #region karakterin rotasyonunu d��manlara �evirir
                enemyDistance = enemy.GetChild(1).position - player.GetChild(i).position;
                player.GetChild(i).rotation = Quaternion.Slerp(player.GetChild(i).rotation, Quaternion.LookRotation(enemyDistance, Vector3.up), Time.fixedDeltaTime * 10);

                #endregion

                #region karakterlerimiz d��manlar� takip eder


                if (enemyDistance.magnitude < 20f && i < enemy.childCount)      //d��man ile aras�ndaki mesafe
                    player.GetChild(i).position = Vector3.MoveTowards(player.GetChild(i).position, enemy.GetChild(i).position, Time.fixedDeltaTime * 1.75f);
                else if (enemyDistance.magnitude < 20)
                    player.GetChild(i).position = Vector3.MoveTowards(player.GetChild(i).position, enemy.GetChild(UnityEngine.Random.Range(1, enemy.childCount)).position, Time.fixedDeltaTime * 1.5f);


                //text 1. karakteri takip eder(bug fix)
                player.GetChild(0).position = new Vector3(player.GetChild(0).position.x, player.GetChild(0).position.y, Mathf.Lerp(player.GetChild(0).position.z, player.GetChild(1).position.z, Time.fixedDeltaTime));
                #endregion
            }
        //d��man bittiyse
        else
        {
            attackState = false;
            enemy.gameObject.SetActive(false);
            playerManager.FormatStickMan();

            if (player.childCount < 2) //d��man ile ayn� anda bitti�inde
            {
                gameManager.gameState = false;
                attackState = false;
                player.GetChild(0).gameObject.SetActive(false);
                gameManager.LoseMenuActivity();

            }
        }
    }
    #endregion

    #region d��manlar karakteri takip eder, rotasyonunu karaktere �evirir ve kaybedildiyse yap�lacak i�lemler
    public void EnemyOffence(Transform player, Transform enemy)
    {
        //sava� yoksa di�er komutlar� �al��t�rmaz
        if (!attackState)
            return;

        //2 taraftan birisi nefes al�yorsa:
        if (enemy.childCount > 1 && player.childCount > 1)
            for (int i = 1; i < enemy.childCount; i++)
            {

                #region d��manlar�n rotasyonunu karaktere �evirir
                playerDistance = player.GetChild(1).position - enemy.GetChild(i).position;
                enemy.GetChild(i).rotation = Quaternion.Slerp(enemy.GetChild(i).rotation, quaternion.LookRotation(playerDistance, Vector3.up), Time.fixedDeltaTime * 10);
                #endregion



                #region karakteri takip eder
                enemy.GetChild(0).gameObject.SetActive(true);

                if (playerDistance.magnitude < 20f && i < player.childCount)     //player ile d��manlar�n aras�ndaki mesafe
                    enemy.GetChild(i).position = Vector3.MoveTowards(enemy.GetChild(i).position, player.GetChild(i).position, Time.fixedDeltaTime * 5);
                else if (playerDistance.magnitude < 20)
                    enemy.GetChild(i).position = Vector3.MoveTowards(enemy.GetChild(i).position, player.GetChild(UnityEngine.Random.Range(0, player.childCount)).position, Time.fixedDeltaTime * 5);


                //text 1. enemyi takip eder(bug fix)
                enemy.GetChild(0).position = new Vector3(Mathf.Lerp(enemy.GetChild(0).position.x, enemy.GetChild(1).position.x, Time.fixedDeltaTime),
                    enemy.GetChild(0).position.y, Mathf.Lerp(enemy.GetChild(0).position.z, enemy.GetChild(1).position.z, Time.fixedDeltaTime));

                #endregion
            }
        #region oyun kaybedildi�inde �al��acak kodlar
        else if (enemy.childCount > 1)
        {
            #region oyunu bitirir            
            EnemyManager enemyManager = enemy.GetComponent<EnemyManager>();
            enemyManager.TextUpdate();
            gameManager.gameState = false;
            attackState = false;
            player.GetChild(0).gameObject.SetActive(false);
            gameManager.LoseMenuActivity();
            #endregion
        }
        #endregion
    }

    #endregion

    #region D�E D�E D�E D�E D�E!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public void KillTheRed(GameObject red)
    {
        objectPoolManager.GiveRedStickman(red);
        soundManager.BattleSound();
    }

    public void KillTheBlue(GameObject blue)
    {
        objectPoolManager.GiveBlueStickman(blue);
        soundManager.BattleSound();
    }
    #endregion
}
