using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class GateManager : MonoBehaviour
{
    PlayerManager playerManager;

    ObjectPoolManager objectPoolManager;

    #region geçit deðiþkenleri
    public TextMeshPro GateNo;

    //gatelerde yazacak sayý
    public int gateNumber;

    //yapýlan iþlemler
    private enum GateType { multiply, addition, subtraction }

    [SerializeField] GateType gateType;
    #endregion



    private void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("player").GetComponent<PlayerManager>();

        objectPoolManager = GameObject.FindGameObjectWithTag("poolManager").GetComponent<ObjectPoolManager>();

        GetGateType();
    }


    //gelecek türe göre geçit sayýsý
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


        #region text ve format güncellemesi
        playerManager.TextUpdate();

        playerManager.FormatStickMan();
        #endregion

    }
    #endregion

}
