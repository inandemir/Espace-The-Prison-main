using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    #region animasyon kontrol
    public void PlayerAnimation(Transform player)
    {
        if (gameManager.gameState)
            for (int i = 1; i < player.childCount; i++)
            {
                player.GetChild(i).GetComponent<Animator>().SetBool("runner", true);
            }
        //oyun durursa
        else if (!gameManager.gameState)
            for (int i = 1; i < player.childCount; i++)
            {
                player.GetChild(i).GetComponent<Animator>().SetBool("runner", false);
            }
    }
    #endregion

    #region bilgi amaçlý 2. yöntem
    //yöntem 2
    /* void PlayerAnimation2(Transform player)
     {
         for (int i = 0; i < player.childCount; i++)
         {
             Animator _PlayerAnimator = player.GetChild(i).GetComponent<Animator>();        
             _PlayerAnimator.SetBool("runner", true);
         }
     }*/
    #endregion



}