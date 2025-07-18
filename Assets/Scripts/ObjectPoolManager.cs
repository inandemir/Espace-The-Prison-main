using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GateManager;

public class ObjectPoolManager : MonoBehaviour
{
    PlayerManager playerManager;

    #region mavi karakter s�ralama    
    private Queue<GameObject> poolBlueFalseObjects;     //kapal� tutacak s�ra    
    private List<GameObject> poolBlueTrueObjects;       //a��k tutan liste    
    [SerializeField] private GameObject blueGO;         //kopyalanacak karakter objesi
    [SerializeField] private int poolSize;              //kopyalanacak karakter say�s�
    #endregion

    #region k�rm�z� karakter s�ralama
    private Queue<GameObject> poolRedFalseObjects;
    [SerializeField] private GameObject redGO;
    #endregion

    #region particle s�ralama
    Queue<ParticleSystem> blueParticleQueue, redParticleQueue;
    [SerializeField] private ParticleSystem blueParticleEffectPS, redParticleEffectPS;
    #endregion


    private void Awake()
    {
        #region mavi karakter
        playerManager = GameObject.FindGameObjectWithTag("player").GetComponent<PlayerManager>();
        poolBlueFalseObjects = new Queue<GameObject>();
        poolBlueTrueObjects = new List<GameObject>();
        MakeBlueStickman();
        #endregion

        #region k�rm�z� karakter

        poolRedFalseObjects = new Queue<GameObject>();

        #endregion

        #region particleSystem
        blueParticleQueue = new Queue<ParticleSystem>();
        MakeBlueParticleEffect();
        redParticleQueue = new Queue<ParticleSystem>();
        MakeRedParticleEffect();
        #endregion

    }

    #region mavi karakter i�lemleri
    #region oyun ba�lad���nda g�r�nmez mavi karakter spawnlan�r
    void MakeBlueStickman()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject _blueGO = Instantiate(blueGO, transform.GetChild(0));
            _blueGO.SetActive(false);

            //false listesine girer
            poolBlueFalseObjects.Enqueue(_blueGO);
        }
    }
    #endregion

    #region mavi karakter objesi g�r�n�r olur ve poolBlueTrueObjects s�ras�na girer
    public GameObject GetBlueStickman()
    {
        //poolblueObject s�ras�ndan objeyi ��kar�r, bu objeyi _blueGO objesine atar.
        GameObject _blueGO = poolBlueFalseObjects.Dequeue();

        _blueGO.transform.parent = playerManager.transform;

        _blueGO.transform.position = new Vector3(playerManager.transform.position.x, playerManager.transform.position.y + -0.4445743f, playerManager.transform.position.z);

        //_playerGO objesini aktif eder
        _blueGO.SetActive(true);

        //true listesine girer
        poolBlueTrueObjects.Add(_blueGO);

        //e�er false karakter kalmam��sa karakter olu�turur.
        if (poolBlueFalseObjects.Count <= 1)
            MakeBlueStickman_Precaution();

        return _blueGO;
    }
    #endregion

    #region �nlem
    //e�er mavi karakter s�rada yoksa, mavi karakter olu�turur.
    void MakeBlueStickman_Precaution()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject _blueGO = Instantiate(blueGO, transform.GetChild(0));
            _blueGO.SetActive(false);

            //false listesine girer
            poolBlueFalseObjects.Enqueue(_blueGO);
        }
    }
    #endregion

    #region mavi obje g�r�nmez olur ve poolBlueFalseObjects s�ras�na girer
    public GameObject GiveBlueStickman(GameObject blueGO)
    {

        //ebeveyni art�k bu scripte ba�l� objenin alt objesi olur
        blueGO.transform.parent = transform.GetChild(0);

        blueGO.transform.rotation = Quaternion.identity;
        //_playerGO objesini deaktif eder
        blueGO.SetActive(false);

        //false listesine girer
        poolBlueFalseObjects.Enqueue(blueGO);

        //poolBlueTrueObjects s�ras�ndan objeyi ��kar�r
        poolBlueTrueObjects.Remove(blueGO);



        return blueGO;
    }
    #endregion
    #endregion

    #region oyun ba�lad���nda k�rm�z� karakter olu�turur
    public void MakeRedStickman(int randomEnemy, Transform blueTransform)
    {

        for (int i = 0; i < randomEnemy; i++)
        {
            Instantiate(redGO, blueTransform.position, new Quaternion(0, 180, 0, 1), blueTransform);
        }


    }
    #endregion

    #region red karakterler g�r�nmez olur
    public GameObject GiveRedStickman(GameObject redGO)
    {
        //ebeveyni art�k bu scripte ba�l� objenin alt objesi olur
        redGO.transform.parent = transform.GetChild(1);

        //false listesine girer
        poolRedFalseObjects.Enqueue(redGO);

        //redGO g�r�nmez olur
        redGO.SetActive(false);

        return redGO;
    }
    #endregion


    #region mavi karakter i�in efekt i�lemleri

    #region oyun ba�lad���nda mavi efekt olu�turur
    private void MakeBlueParticleEffect()
    {
        for (int i = 0; i < 20; i++)
        {
            ParticleSystem blueParticle = Instantiate(blueParticleEffectPS, transform.GetChild(2));
            blueParticle.gameObject.SetActive(false);
            blueParticleQueue.Enqueue(blueParticle);
        }
    }
    #endregion

    #region mavi efekti g�r�n�r yapar
    public void BlueParticleActivate(Transform blueParticleTransform)
    {
        ParticleSystem _blueParticle = blueParticleQueue.Dequeue();
        _blueParticle.transform.position = blueParticleTransform.position;
        _blueParticle.gameObject.SetActive(true);
        _blueParticle.Play();
        blueParticleQueue.Enqueue(_blueParticle);
        //burada da falseye �evirir
        StartCoroutine(ParticleDeactivate(_blueParticle));
    }
    #endregion

    #endregion

    #region k�rm�z� karakter i�in efekt i�lemleri

    #region oyun ba�lad���nda k�rm�z� efekt olu�turur


    private void MakeRedParticleEffect()
    {
        for (int i = 0; i < 20; i++)
        {
            ParticleSystem redParticle = Instantiate(redParticleEffectPS, transform.GetChild(3));
            redParticle.gameObject.SetActive(false);
            redParticleQueue.Enqueue(redParticle);
        }
    }
    #endregion

    #region k�rm�z� efekti g�r�n�r yapar
    public void RedParticleActivate(Transform redParticleTransform)
    {
        ParticleSystem _redParticle = redParticleQueue.Dequeue();
        _redParticle.transform.position = redParticleTransform.position;
        _redParticle.gameObject.SetActive(true);
        _redParticle.Play();
        redParticleQueue.Enqueue(_redParticle);
        //burada da falseye �evirir
        StartCoroutine(ParticleDeactivate(_redParticle));
    }
    #endregion

    #endregion


    #region �a�r�lan metotlardaki particle efekti false yapar
    IEnumerator ParticleDeactivate(ParticleSystem particle)
    {
        yield return new WaitForSecondsRealtime(2);
        particle.gameObject.SetActive(false);
    }
    #endregion



}
