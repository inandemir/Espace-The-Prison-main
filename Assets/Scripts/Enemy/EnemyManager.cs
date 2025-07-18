using TMPro;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    GameManager gameManager;

    BattleManager battleManager;

    ObjectPoolManager poolManager;

    #region d��man say�s�, objesi ve Text

    [SerializeField] TextMeshPro CounterTxt;
    [SerializeField] int redStickmanNumber;
    #endregion

    #region d��manlar�n hizas� i�in gereken de�i�kenler
    [Range(0f, 2f)][SerializeField] float distanceFactor, radius;
    #endregion

    private Transform player;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("gameManager").GetComponent<GameManager>();

        battleManager = GameObject.FindGameObjectWithTag("battleManager").GetComponent<BattleManager>();

        poolManager = GameObject.FindGameObjectWithTag("poolManager").GetComponent<ObjectPoolManager>();

        poolManager.MakeRedStickman(redStickmanNumber, transform);

        player = GameObject.FindGameObjectWithTag("player").transform;

        CounterTxt.text = (transform.childCount - 1).ToString();

        FormatEnemyStickMan();
    }

    private void FixedUpdate()
    {
        if (battleManager.attackState)
        {
            battleManager.EnemyOffence(player, transform);
        }
    }

    #region kopyalanan d��man hizas�
    private void FormatEnemyStickMan()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            //UFUFU WEWEWE ONYETEN WEWEWE UG�M�M� OSAS
            var x = distanceFactor * Mathf.Sqrt(i) * Mathf.Cos(i * radius);
            //UFUFU WEWEWE ONYETEN WEWEWE UG�M�M� OSAS
            var z = distanceFactor * Mathf.Sqrt(i) * Mathf.Sin(i * radius);

            var _NewPos = new Vector3(x, -0.4445743f, z);

            transform.GetChild(i).localPosition = _NewPos;
        }

    }
    #endregion

    #region d��manlar i�in animasyon 
    public void EnemyAnimation(Transform police)
    {
        if (gameManager.gameState && battleManager.attackState)
            police.GetComponent<Animator>().SetBool("runner", true);                 //getcomponentden daha iyi ��z�m?


        //oyun durursa veya sava� biterse 
        else if (!gameManager.gameState || !battleManager.attackState)
            police.GetComponent<Animator>().SetBool("runner", false);
    }
    #endregion

    #region d��manlar�n say�s�n� g�steren text g�ncellemesi
    public void TextUpdate()
    {
        if (transform.childCount == 1)
            transform.GetChild(0).gameObject.SetActive(false);

        //ge� yans�tt��� i�in - 2 yap�yorum.
        CounterTxt.text = (transform.childCount - 2).ToString();

    }
    #endregion

}
