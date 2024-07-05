
using Frictionless;
using Lastforever.Types;
using Solana.Unity.SDK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SnailContextMenu : MonoBehaviour
{
  public GameObject Root;
  public Button CloseButton;
  public Button CloseButton2;
  public Button DelayButton;
  public Button SpeedupButton;
  public Button UpgradeArmor;
  public TextMeshProUGUI CurrentPositionText;
  public GameObject YourSnailRoot;

  private SnailData currentSnailData;

  private void Awake()
  {
    ServiceFactory.RegisterSingleton(this);
    CloseButton.onClick.AddListener(Close);
    CloseButton2.onClick.AddListener(Close);
    DelayButton.onClick.AddListener(OnDelayClicked);
    SpeedupButton.onClick.AddListener(OnSpeedupClicked);
    UpgradeArmor.onClick.AddListener(OnUpgradeArmorClicked);
  }

  private bool CheckForEnoughSlime(ulong amount)
  {
    if (AnchorService.Instance.CurrentPlayerData.Energy < amount)
    {
      ServiceFactory.Resolve<ErrorPopup>().Open("You need more slime!");
      return false;
    }
    return true;
  }

  private void OnDelayClicked()
  {
    if (!CheckForEnoughSlime(20))
    {
      return;
    }
    AnchorService.Instance.SnailAction(!Web3.Rpc.NodeAddress.AbsoluteUri.Contains("localhost"), () =>
    {
      Debug.Log("Success snail interact");
    }, 0, currentSnailData.Authority);
    Close();
  }

  private void OnSpeedupClicked()
  {
    if (!CheckForEnoughSlime(30))
    {
      return;
    }
    AnchorService.Instance.SnailAction(!Web3.Rpc.NodeAddress.AbsoluteUri.Contains("localhost"), () =>
    {
      Debug.Log("Success snail interact");
    }, 1, currentSnailData.Authority);
    Close();
  }

  private void OnUpgradeArmorClicked()
  {
    if (!CheckForEnoughSlime(55))
    {
      return;
    }
    AnchorService.Instance.SnailAction(!Web3.Rpc.NodeAddress.AbsoluteUri.Contains("localhost"), () =>
    {
      Debug.Log("Success snail interact");
    }, 2, currentSnailData.Authority);
    Close();
  }

  private void OnUpgradeWeaponClicked()
  {
    if (!CheckForEnoughSlime(55))
    {
      return;
    }
    AnchorService.Instance.SnailAction(!Web3.Rpc.NodeAddress.AbsoluteUri.Contains("localhost"), () =>
    {
      Debug.Log("Success snail interact");
    }, 3, currentSnailData.Authority);
    Close();
  }

  private void Close()
  {
    Root.gameObject.SetActive(false);
  }

  public void Open(bool open, SnailData snailData)
  {
    currentSnailData = snailData;
    Root.gameObject.SetActive(open);
    YourSnailRoot.gameObject.SetActive(snailData.Authority == Web3.Account.PublicKey);
  }

  private void Update()
  {
    if (currentSnailData == null)
    {
      return;
    }
    CurrentPositionText.text = "Current position = " + AnchorService.Instance.CalculateCurrentPosition(currentSnailData).ToString() + "mm";
  }
}
