using System.Collections;
using TMPro;
using UnityEngine;
using Unity.Mathematics;
using System;

public class PlayerManager : MonoBehaviour
{
    #region �a��rd���m classlar
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



    //karakter say�s�n� tutar. ek bilgi: player objesinin alt objesindeki textte tutar.
    [SerializeField] private TextMeshPro CounterTxt;


    #region karakterlerin pozisyonlar� i�in gereken de�i�kenler
    [Range(0f, 2f)][SerializeField] float distanceFactor, radius;
    #endregion

    #region sava�aca�� d��man� takip etmesi i�in de�i�ken
    private Transform enemy;
    #endregion


    private void Start()
    {
        #region ba�lang�� karakteri olu�turma, text g�ncelleme
        getClass.objectPoolManager.GetBlueStickman();
        TextUpdate();
        //text aktif oluyor(hiyera�ide kapatt�m)
        transform.GetChild(0).gameObject.SetActive(true);
        #endregion
        getClass.soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    }

    private void FixedUpdate()
    {
        //bunun updateden kald�r�p, daha az performans harcayacak bir yere ta��nmas� gerekiyor.
        getClass.animatorManager.PlayerAnimation(transform);

        #region player sava� ba�lad�ysa: d��man� takip eder, d��mana do�ru rotasyonunu �evirir.
        getClass.battleManager.PlayerOffence(enemy, transform);

        #endregion

    }

    #region stickman hizaya getirme i�lemi
    //kopyalanan stickmanlar�n pozisyonu

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

            // E�er hareket s�resi sabitse
            float moveTime = 1.5f;

            // E�er hareket s�resi her obje i�in farkl� ise
            // float moveTime = i * 0.1f + 1.0f; // �rnek: her obje 0.1 saniye daha fazla s�rede hareket etsin


            if (!getClass.battleManager.attackState && !getClass.gameManager.bossBattlestate)
                MoveObjectLinear(transform.GetChild(i), newPos, moveTime);
        }
        if (!getClass.gameManager.bossBattlestate)
            transform.GetChild(0).position = new Vector3(transform.position.x, transform.GetChild(0).position.y, transform.position.z);
    }

    // Kullan�c� tan�ml� lineer hareket fonksiyonu
    private void MoveObjectLinear(Transform obj, Vector3 targetPosition, float duration)
    {
        StartCoroutine(MoveObjectLinearCoroutine(obj, targetPosition, duration));
    }

    private IEnumerator MoveObjectLinearCoroutine(Transform obj, Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startingPosition = obj.localPosition;

        while (elapsedTime < duration && obj != null) // obj null de�ilse devam et
        {
            obj.localPosition = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime * 3f;
            yield return null;
        }

        // Coroutine sona erdi�inde obje hala null de�ilse g�ncelle
        if (obj != null)
        {
            obj.localPosition = targetPosition;
        }
    }
    #endregion

    #region karakterlerin rotasyon s�f�rlanmas�
    public void Alignment()
    {
        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i).rotation = Quaternion.Slerp(transform.GetChild(0).rotation, quaternion.identity, Time.deltaTime);
        }
    }

    #endregion

    #region karakterlerin say�s�n� g�steren text g�ncellemesi
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

            //�arpt��� objenin scriptini "_gateManager" olarak miras almam�za olanak sa�lar
            GateManager _gateManager = other.GetComponent<GateManager>();

            //ge�itten ge�ince stickman kopyalan�r
            _gateManager.GetStickman(transform);
            getClass.soundManager.GateSound();
        }
        #endregion

        #region d��man g�rd�yse sald�r� moduna ge�er
        if (other.CompareTag("enemyManager"))
        {
            getClass.battleManager.attackState = true;

            //d��mana bakmak i�in ama bu de�i�tirilecek
            enemy = other.transform;
        }
        #endregion

        #region finishe ula�t���nda yada bossa ula�t���nda
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