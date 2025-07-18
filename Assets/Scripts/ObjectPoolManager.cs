using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GateManager;

public class ObjectPoolManager : MonoBehaviour
{
    PlayerManager playerManager;

    #region mavi karakter sýralama    
    private Queue<GameObject> poolBlueFalseObjects;     //kapalý tutacak sýra    
    private List<GameObject> poolBlueTrueObjects;       //açýk tutan liste    
    [SerializeField] private GameObject blueGO;         //kopyalanacak karakter objesi
    [SerializeField] private int poolSize;              //kopyalanacak karakter sayýsý
    #endregion

    #region kýrmýzý karakter sýralama
    private Queue<GameObject> poolRedFalseObjects;
    [SerializeField] private GameObject redGO;
    #endregion

    #region particle sýralama
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

        #region kýrmýzý karakter

        poolRedFalseObjects = new Queue<GameObject>();

        #endregion

        #region particleSystem
        blueParticleQueue = new Queue<ParticleSystem>();
        MakeBlueParticleEffect();
        redParticleQueue = new Queue<ParticleSystem>();
        MakeRedParticleEffect();
        #endregion

    }

    #region mavi karakter iþlemleri
    #region oyun baþladýðýnda görünmez mavi karakter spawnlanýr
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

    #region mavi karakter objesi görünür olur ve poolBlueTrueObjects sýrasýna girer
    public GameObject GetBlueStickman()
    {
        //poolblueObject sýrasýndan objeyi çýkarýr, bu objeyi _blueGO objesine atar.
        GameObject _blueGO = poolBlueFalseObjects.Dequeue();

        _blueGO.transform.parent = playerManager.transform;

        _blueGO.transform.position = new Vector3(playerManager.transform.position.x, playerManager.transform.position.y + -0.4445743f, playerManager.transform.position.z);

        //_playerGO objesini aktif eder
        _blueGO.SetActive(true);

        //true listesine girer
        poolBlueTrueObjects.Add(_blueGO);

        //eðer false karakter kalmamýþsa karakter oluþturur.
        if (poolBlueFalseObjects.Count <= 1)
            MakeBlueStickman_Precaution();

        return _blueGO;
    }
    #endregion

    #region önlem
    //eðer mavi karakter sýrada yoksa, mavi karakter oluþturur.
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

    #region mavi obje görünmez olur ve poolBlueFalseObjects sýrasýna girer
    public GameObject GiveBlueStickman(GameObject blueGO)
    {

        //ebeveyni artýk bu scripte baðlý objenin alt objesi olur
        blueGO.transform.parent = transform.GetChild(0);

        blueGO.transform.rotation = Quaternion.identity;
        //_playerGO objesini deaktif eder
        blueGO.SetActive(false);

        //false listesine girer
        poolBlueFalseObjects.Enqueue(blueGO);

        //poolBlueTrueObjects sýrasýndan objeyi çýkarýr
        poolBlueTrueObjects.Remove(blueGO);



        return blueGO;
    }
    #endregion
    #endregion

    #region oyun baþladýðýnda kýrmýzý karakter oluþturur
    public void MakeRedStickman(int randomEnemy, Transform blueTransform)
    {

        for (int i = 0; i < randomEnemy; i++)
        {
            Instantiate(redGO, blueTransform.position, new Quaternion(0, 180, 0, 1), blueTransform);
        }


    }
    #endregion

    #region red karakterler görünmez olur
    public GameObject GiveRedStickman(GameObject redGO)
    {
        //ebeveyni artýk bu scripte baðlý objenin alt objesi olur
        redGO.transform.parent = transform.GetChild(1);

        //false listesine girer
        poolRedFalseObjects.Enqueue(redGO);

        //redGO görünmez olur
        redGO.SetActive(false);

        return redGO;
    }
    #endregion


    #region mavi karakter için efekt iþlemleri

    #region oyun baþladýðýnda mavi efekt oluþturur
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

    #region mavi efekti görünür yapar
    public void BlueParticleActivate(Transform blueParticleTransform)
    {
        ParticleSystem _blueParticle = blueParticleQueue.Dequeue();
        _blueParticle.transform.position = blueParticleTransform.position;
        _blueParticle.gameObject.SetActive(true);
        _blueParticle.Play();
        blueParticleQueue.Enqueue(_blueParticle);
        //burada da falseye çevirir
        StartCoroutine(ParticleDeactivate(_blueParticle));
    }
    #endregion

    #endregion

    #region kýrmýzý karakter için efekt iþlemleri

    #region oyun baþladýðýnda kýrmýzý efekt oluþturur


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

    #region kýrmýzý efekti görünür yapar
    public void RedParticleActivate(Transform redParticleTransform)
    {
        ParticleSystem _redParticle = redParticleQueue.Dequeue();
        _redParticle.transform.position = redParticleTransform.position;
        _redParticle.gameObject.SetActive(true);
        _redParticle.Play();
        redParticleQueue.Enqueue(_redParticle);
        //burada da falseye çevirir
        StartCoroutine(ParticleDeactivate(_redParticle));
    }
    #endregion

    #endregion


    #region çaðrýlan metotlardaki particle efekti false yapar
    IEnumerator ParticleDeactivate(ParticleSystem particle)
    {
        yield return new WaitForSecondsRealtime(2);
        particle.gameObject.SetActive(false);
    }
    #endregion



}
