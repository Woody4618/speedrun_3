using System;
using Solana.Unity.SDK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BirdController : MonoBehaviour
{
   public Button SendBirdButton;
   public Animator BirdAnimator;
   public TextMeshProUGUI BirdCountDown;

   private void Start()
   {
     SendBirdButton.onClick.AddListener(OnSendBirdButtonClicked);
   }

   private void Update()
   {
     if (AnchorService.Instance == null || AnchorService.Instance.CurrentGameData == null)
     {
       return;
     }

     var currentGameData = AnchorService.Instance.CurrentGameData;
     var lastTimeEaten = currentGameData.LastSnailEatenTime;

     var timePassed = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - lastTimeEaten;
     var timeUntilNextRefill = AnchorService.BIRD_EAT_DELAY - timePassed;

     if (timeUntilNextRefill > 0)
     {
       BirdCountDown.text = timeUntilNextRefill.ToString();
     }
     else
     {
       BirdCountDown.text = "Ready";
     }

     SendBirdButton.gameObject.SetActive(timeUntilNextRefill <= 0);
   }

   private void OnSendBirdButtonClicked()
   {
     AnchorService.Instance.SendBird(!Web3.Rpc.NodeAddress.AbsoluteUri.Contains("localhost"), () =>
     {
       BirdAnimator.Play("FlyAndEat");
       Debug.Log("Send bird success");
     });
   }
}
