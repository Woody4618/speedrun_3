using System;
using System.Collections;
using System.Collections.Generic;
using Solana.Unity.SDK;
using UnityEngine;
using UnityEngine.UI;

public class BirdController : MonoBehaviour
{
   public Button SendBirdButton;
   public Animator BirdAnimator;

   private void Start()
   {
     SendBirdButton.onClick.AddListener(OnSendBirdButtonClicked);
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
