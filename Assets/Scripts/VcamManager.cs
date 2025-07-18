using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class VCamManager : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    private CinemachineTransposer transposer;

    private GameManager gameManager;

    //kamera pozisyon deðiþtirme süresi
    float duration = 1.5f;



    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("gameManager").GetComponent<GameManager>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        GetCameraPos();
    }


    public void GetCameraPos()
    {
        if (gameManager.bossBattlestate)
        {
            StartCoroutine(ChangeFollowOffset(new(0, 9.40f, -6f)));
            player.GetChild(0).localScale = Vector3.one;
            return;
        }

        Vector3 playerOffset = new(0, 7f, -3.40f);

        if (player.childCount <= 29)
            playerOffset = new(0, 7f, -3.40f);
        else if (player.childCount >= 30 && player.childCount <= 60)
            playerOffset = new(0, 7.40f, -3.60f);
        else if (player.childCount >= 61 && player.childCount <= 90)
            playerOffset = new(0, 7.80f, -3.80f);
        else if (player.childCount >= 91 && player.childCount <= 120)
            playerOffset = new(0, 8.20f, -4f);
        else if (player.childCount >= 121 && player.childCount <= 150)
            playerOffset = new(0, 8.60f, -4.20f);
        else if (player.childCount >= 151 && player.childCount <= 180)
            playerOffset = new(0, 9f, -4.40f);
        else if (player.childCount >= 181)
            playerOffset = new(0, 9.40f, -5f);

        player.GetChild(0).localScale = Vector3.one / 1.31f;
        StartCoroutine(ChangeFollowOffset(playerOffset));
    }
    float elapsedTime;
    bool offsetChange = true;
    public IEnumerator ChangeFollowOffset(Vector3 targetOffset)
    {
        Vector3 startFollowOffset = transposer.m_FollowOffset;
        if (offsetChange)
            while (elapsedTime < duration)
            {
                offsetChange = false;
                elapsedTime += Time.deltaTime;

                float t = Mathf.Clamp01(elapsedTime / duration);

                transposer.m_FollowOffset = Vector3.Lerp(startFollowOffset, targetOffset, t);

                yield return null;
            }

        yield return new WaitForSeconds(1.5f);
        offsetChange = true;
        elapsedTime = 0;

    }


}