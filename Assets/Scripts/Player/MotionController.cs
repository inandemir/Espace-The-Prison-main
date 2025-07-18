using UnityEngine;

public class MotionController : MonoBehaviour
{
    private GameManager gameManager;

    private BattleManager battleManager;



    private float _touchPos;

    //t�klama kontrol�
    private bool TouchControl;

    #region hareket h�z de�i�kenleri
    [SerializeField] float xSpeed, zSpeed;

    #endregion

    #region sa�a sola kayd�rma s�n�r� de�i�keni
    private float xBorder = 4.5f;
    #endregion

    [SerializeField] float swipeSpeed = 2f;
    [SerializeField] float sensibility = 0.1f;

    private void Start()
    {

        gameManager = GameObject.FindGameObjectWithTag("gameManager").GetComponent<GameManager>();

        battleManager = GameObject.FindGameObjectWithTag("battleManager").GetComponent<BattleManager>();

    }

    void Update()
    {
        Swipe();
    }

    private void FixedUpdate()
    {
        //e�er oyun aktif de�ilse di�er komutlara girmez.
        if (!gameManager.gameState)
        {
            return;
        }

        if (!gameManager.bossBattlestate || !gameManager.permission)
        {
            Movement();
        }
    }

    void Swipe()
    {
        if (Input.touchCount > 0)
        {
            Touch _touch = Input.GetTouch(0);

            switch (_touch.phase)
            {
                case TouchPhase.Began: //ekrana dokunmaya ba�land���nda de�i�keni true yap
                    TouchControl = true;
                    break;

                case TouchPhase.Moved: //Dokunma devam ediyorsa parma��n hareketine g�re g�ncelleme yap
                    if (TouchControl)
                    {
                        _touchPos += _touch.deltaPosition.x * swipeSpeed * sensibility / Screen.width * 2f;
                    }
                    break;

                case TouchPhase.Ended: // dokunma bitti�inde de�i�keni false yap
                case TouchPhase.Canceled:
                    TouchControl = false;
                    break;
            }
        }
    }

    void Movement()
    {
        //sava� ba�larsa
        if (battleManager.attackState)
        {
            #region sa�a ve sola ge�i� s�n�r�
            if (transform.childCount < 50)
                _touchPos = Mathf.Clamp(_touchPos, -xBorder, xBorder);
            else
                _touchPos = Mathf.Clamp(_touchPos, -3.5f, 3.5f);
            #endregion

            transform.position = new Vector3(Mathf.Lerp(transform.position.x, _touchPos, Time.fixedDeltaTime / 3), transform.position.y, transform.position.z + 2f * Time.fixedDeltaTime);

        }
        else
        {
            #region sa�a ve sola ge�i� s�n�r�
            if (transform.childCount < 50)
                _touchPos = Mathf.Clamp(_touchPos, -xBorder, xBorder);
            else
                _touchPos = Mathf.Clamp(_touchPos, -3.5f, 3.5f);
            #endregion

            //player sa�a sola kayd�rma
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, _touchPos, Time.fixedDeltaTime * xSpeed), transform.position.y, transform.position.z + zSpeed * Time.fixedDeltaTime);
        }
    }
}