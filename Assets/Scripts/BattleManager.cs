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

    #region karakterler düþmanlarý takip eder ve eðer düþman kalmadýysa yapýlacak iþlemler
    public void PlayerOffence(Transform enemy, Transform player)
    {
        //savaþ yoksa diðer komutlarý çalýþtýrmaz
        if (!attackState)
            return;

        //savaþ baþladýysa playerler enemy içine girer
        if (enemy.childCount > 1)
            for (int i = 1; i < player.childCount; i++)
            {
                #region karakterin rotasyonunu düþmanlara çevirir
                enemyDistance = enemy.GetChild(1).position - player.GetChild(i).position;
                player.GetChild(i).rotation = Quaternion.Slerp(player.GetChild(i).rotation, Quaternion.LookRotation(enemyDistance, Vector3.up), Time.fixedDeltaTime * 10);

                #endregion

                #region karakterlerimiz düþmanlarý takip eder


                if (enemyDistance.magnitude < 20f && i < enemy.childCount)      //düþman ile arasýndaki mesafe
                    player.GetChild(i).position = Vector3.MoveTowards(player.GetChild(i).position, enemy.GetChild(i).position, Time.fixedDeltaTime * 1.75f);
                else if (enemyDistance.magnitude < 20)
                    player.GetChild(i).position = Vector3.MoveTowards(player.GetChild(i).position, enemy.GetChild(UnityEngine.Random.Range(1, enemy.childCount)).position, Time.fixedDeltaTime * 1.5f);


                //text 1. karakteri takip eder(bug fix)
                player.GetChild(0).position = new Vector3(player.GetChild(0).position.x, player.GetChild(0).position.y, Mathf.Lerp(player.GetChild(0).position.z, player.GetChild(1).position.z, Time.fixedDeltaTime));
                #endregion
            }
        //düþman bittiyse
        else
        {
            attackState = false;
            enemy.gameObject.SetActive(false);
            playerManager.FormatStickMan();

            if (player.childCount < 2) //düþman ile ayný anda bittiðinde
            {
                gameManager.gameState = false;
                attackState = false;
                player.GetChild(0).gameObject.SetActive(false);
                gameManager.LoseMenuActivity();

            }
        }
    }
    #endregion

    #region düþmanlar karakteri takip eder, rotasyonunu karaktere çevirir ve kaybedildiyse yapýlacak iþlemler
    public void EnemyOffence(Transform player, Transform enemy)
    {
        //savaþ yoksa diðer komutlarý çalýþtýrmaz
        if (!attackState)
            return;

        //2 taraftan birisi nefes alýyorsa:
        if (enemy.childCount > 1 && player.childCount > 1)
            for (int i = 1; i < enemy.childCount; i++)
            {

                #region düþmanlarýn rotasyonunu karaktere çevirir
                playerDistance = player.GetChild(1).position - enemy.GetChild(i).position;
                enemy.GetChild(i).rotation = Quaternion.Slerp(enemy.GetChild(i).rotation, quaternion.LookRotation(playerDistance, Vector3.up), Time.fixedDeltaTime * 10);
                #endregion



                #region karakteri takip eder
                enemy.GetChild(0).gameObject.SetActive(true);

                if (playerDistance.magnitude < 20f && i < player.childCount)     //player ile düþmanlarýn arasýndaki mesafe
                    enemy.GetChild(i).position = Vector3.MoveTowards(enemy.GetChild(i).position, player.GetChild(i).position, Time.fixedDeltaTime * 5);
                else if (playerDistance.magnitude < 20)
                    enemy.GetChild(i).position = Vector3.MoveTowards(enemy.GetChild(i).position, player.GetChild(UnityEngine.Random.Range(0, player.childCount)).position, Time.fixedDeltaTime * 5);


                //text 1. enemyi takip eder(bug fix)
                enemy.GetChild(0).position = new Vector3(Mathf.Lerp(enemy.GetChild(0).position.x, enemy.GetChild(1).position.x, Time.fixedDeltaTime),
                    enemy.GetChild(0).position.y, Mathf.Lerp(enemy.GetChild(0).position.z, enemy.GetChild(1).position.z, Time.fixedDeltaTime));

                #endregion
            }
        #region oyun kaybedildiðinde çalýþacak kodlar
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

    #region DÝE DÝE DÝE DÝE DÝE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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
