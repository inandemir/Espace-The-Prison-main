using System.Collections;
using TMPro;
using UnityEngine;
using Unity.Mathematics;
using System;

public class PlayerManager : MonoBehaviour
{
    #region çaðýrdýðým classlar
    [Serializable]
    private class GetClass
    {
        public BattleManager battleManager;

        public AnimatorManager animatorManager;

        public GameManager gameManager;

        public ObjectPoolManager objectPoolManager;

        public SoundManager soundManager;

        public VCamManager vCamManager;

        public BossManager bossManager;
    }

    [SerializeField] GetClass getClass;
    #endregion



    //karakter sayýsýný tutar. ek bilgi: player objesinin alt objesindeki textte tutar.
    [SerializeField] private TextMeshPro CounterTxt;


    #region karakterlerin pozisyonlarý için gereken deðiþkenler
    [Range(0f, 2f)][SerializeField] float distanceFactor, radius;
    #endregion

    #region savaþacaðý düþmaný takip etmesi için deðiþken
    private Transform enemy;
    #endregion


    private void Start()
    {
        #region baþlangýç karakteri oluþturma, text güncelleme
        getClass.objectPoolManager.GetBlueStickman();
        TextUpdate();
        //text aktif oluyor(hiyeraþide kapattým)
        transform.GetChild(0).gameObject.SetActive(true);
        #endregion
        getClass.soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    }

    private void FixedUpdate()
    {
        //bunun updateden kaldýrýp, daha az performans harcayacak bir yere taþýnmasý gerekiyor.
        getClass.animatorManager.PlayerAnimation(transform);

        #region player savaþ baþladýysa: düþmaný takip eder, düþmana doðru rotasyonunu çevirir.
        getClass.battleManager.PlayerOffence(enemy, transform);

        #endregion

    }

    #region stickman hizaya getirme iþlemi
    //kopyalanan stickmanlarýn pozisyonu

    public void FormatStickMan()
    {
        getClass.vCamManager.GetCameraPos();
        TextUpdate();
        Alignment();
        for (int i = 1; i < transform.childCount; i++)
        {
            var x = distanceFactor * Mathf.Sqrt(i) * Mathf.Cos(i * radius);
            var z = distanceFactor * Mathf.Sqrt(i) * Mathf.Sin(i * radius);

            var newPos = new Vector3(x, -0.4445743f, z);

            // Eðer hareket süresi sabitse
            float moveTime = 1.5f;

            // Eðer hareket süresi her obje için farklý ise
            // float moveTime = i * 0.1f + 1.0f; // Örnek: her obje 0.1 saniye daha fazla sürede hareket etsin


            if (!getClass.battleManager.attackState && !getClass.gameManager.bossBattlestate)
                MoveObjectLinear(transform.GetChild(i), newPos, moveTime);
        }
        if (!getClass.gameManager.bossBattlestate)
            transform.GetChild(0).position = new Vector3(transform.position.x, transform.GetChild(0).position.y, transform.position.z);
    }

    // Kullanýcý tanýmlý lineer hareket fonksiyonu
    private void MoveObjectLinear(Transform obj, Vector3 targetPosition, float duration)
    {
        StartCoroutine(MoveObjectLinearCoroutine(obj, targetPosition, duration));
    }

    private IEnumerator MoveObjectLinearCoroutine(Transform obj, Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startingPosition = obj.localPosition;

        while (elapsedTime < duration && obj != null) // obj null deðilse devam et
        {
            obj.localPosition = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime * 3f;
            yield return null;
        }

        // Coroutine sona erdiðinde obje hala null deðilse güncelle
        if (obj != null)
        {
            obj.localPosition = targetPosition;
        }
    }
    #endregion

    #region karakterlerin rotasyon sýfýrlanmasý
    public void Alignment()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).rotation = Quaternion.Slerp(transform.GetChild(0).rotation, quaternion.identity, Time.deltaTime);
        }
    }

    #endregion

    #region karakterlerin sayýsýný gösteren text güncellemesi
    public void TextUpdate()
    {
        CounterTxt.text = (transform.childCount - 1).ToString();
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        #region karakter kopyalama
        if (other.gameObject.CompareTag("gate"))
        {
            other.transform.parent.GetChild(0).GetComponent<BoxCollider>().enabled = false;

            other.transform.parent.GetChild(1).GetComponent<BoxCollider>().enabled = false;

            other.gameObject.SetActive(false);

            //çarptýðý objenin scriptini "_gateManager" olarak miras almamýza olanak saðlar
            GateManager _gateManager = other.GetComponent<GateManager>();

            //geçitten geçince stickman kopyalanýr
            _gateManager.GetStickman(transform);
            getClass.soundManager.GateSound();
        }
        #endregion

        #region düþman gördüyse saldýrý moduna geçer
        if (other.CompareTag("enemyManager"))
        {
            getClass.battleManager.attackState = true;

            //düþmana bakmak için ama bu deðiþtirilecek
            enemy = other.transform;
        }
        #endregion

        #region finishe ulaþtýðýnda yada bossa ulaþtýðýnda
        if (other.CompareTag("finish"))
        {
            StartCoroutine(getClass.gameManager.GameWin());
        }

        if (other.CompareTag("BossFight"))
        {
            getClass.gameManager.bossBattlestate = true;
            FormatStickMan();
            getClass.vCamManager.GetCameraPos();
        }
        #endregion
    }
}