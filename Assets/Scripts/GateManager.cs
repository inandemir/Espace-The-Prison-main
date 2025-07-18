using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class GateManager : MonoBehaviour
{
    PlayerManager playerManager;

    ObjectPoolManager objectPoolManager;

    #region ge�it de�i�kenleri
    public TextMeshPro GateNo;

    //gatelerde yazacak say�
    public int gateNumber;

    //yap�lan i�lemler
    private enum GateType { multiply, addition, subtraction }

    [SerializeField] GateType gateType;
    #endregion



    private void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("player").GetComponent<PlayerManager>();

        objectPoolManager = GameObject.FindGameObjectWithTag("poolManager").GetComponent<ObjectPoolManager>();

        GetGateType();
    }


    //gelecek t�re g�re ge�it say�s�
    void GetGateType()
    {
        switch (gateType)
        {
            case GateType.multiply:
                GateNo.text = "X " + gateNumber.ToString();
                break;

            case GateType.subtraction:
                GateNo.text = "- " + gateNumber.ToString();
                break;

            case GateType.addition:
                GateNo.text = gateNumber.ToString();
                break;
        }
    }


    #region stickman kopyalama 
    public void GetStickman(Transform player)
    {
        if (gateType == GateType.multiply)
        {
            gateNumber = (player.transform.childCount - 1) * gateNumber - player.transform.childCount + 1;
            for (int i = 0; i < gateNumber; i++)
                objectPoolManager.GetBlueStickman();
        }

        else if (gateType == GateType.addition)
            for (int i = 0; i < gateNumber; i++)
                objectPoolManager.GetBlueStickman();

        else if (gateType == GateType.subtraction)
            for (int i = 0; i < gateNumber; i++)
                if (player.transform.childCount > 2)
                    objectPoolManager.GiveBlueStickman(player.GetChild(2).gameObject);


        #region text ve format g�ncellemesi
        playerManager.TextUpdate();

        playerManager.FormatStickMan();
        #endregion

    }
    #endregion

}
