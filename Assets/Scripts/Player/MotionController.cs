using UnityEngine;

public class MotionController : MonoBehaviour
{
    private GameManager gameManager;

    private BattleManager battleManager;



    private float _touchPos;

    //týklama kontrolü
    private bool TouchControl;

    #region hareket hýz deðiþkenleri
    [SerializeField] float xSpeed, zSpeed;

    #endregion

    #region saða sola kaydýrma sýnýrý deðiþkeni
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
        //eðer oyun aktif deðilse diðer komutlara girmez.
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
                case TouchPhase.Began: //ekrana dokunmaya baþlandýðýnda deðiþkeni true yap
                    TouchControl = true;
                    break;

                case TouchPhase.Moved: //Dokunma devam ediyorsa parmaðýn hareketine göre güncelleme yap
                    if (TouchControl)
                    {
                        _touchPos += _touch.deltaPosition.x * swipeSpeed * sensibility / Screen.width * 2f;
                    }
                    break;

                case TouchPhase.Ended: // dokunma bittiðinde deðiþkeni false yap
                case TouchPhase.Canceled:
                    TouchControl = false;
                    break;
            }
        }
    }

    void Movement()
    {
        //savaþ baþlarsa
        if (battleManager.attackState)
        {
            #region saða ve sola geçiþ sýnýrý
            if (transform.childCount < 50)
                _touchPos = Mathf.Clamp(_touchPos, -xBorder, xBorder);
            else
                _touchPos = Mathf.Clamp(_touchPos, -3.5f, 3.5f);
            #endregion

            transform.position = new Vector3(Mathf.Lerp(transform.position.x, _touchPos, Time.fixedDeltaTime / 3), transform.position.y, transform.position.z + 2f * Time.fixedDeltaTime);

        }
        else
        {
            #region saða ve sola geçiþ sýnýrý
            if (transform.childCount < 50)
                _touchPos = Mathf.Clamp(_touchPos, -xBorder, xBorder);
            else
                _touchPos = Mathf.Clamp(_touchPos, -3.5f, 3.5f);
            #endregion

            //player saða sola kaydýrma
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, _touchPos, Time.fixedDeltaTime * xSpeed), transform.position.y, transform.position.z + zSpeed * Time.fixedDeltaTime);
        }
    }
}