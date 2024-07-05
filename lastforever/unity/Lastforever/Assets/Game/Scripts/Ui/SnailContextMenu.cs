
using Frictionless;
using Lastforever.Types;
using Solana.Unity.SDK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class SnailContextMenu : MonoBehaviour
{
  public GameObject Root;
  public Button CloseButton;
  public Button DelayButton;
  public Button SpeedupButton;
  public Button UpgradeArmor;
  public TextMeshProUGUI CurrentPositionText;

  private SnailData currentSnailData;

  private void Awake()
  {
    ServiceFactory.RegisterSingleton(this);
    CloseButton.onClick.AddListener(Close);
    DelayButton.onClick.AddListener(OnDelayClicked);
    SpeedupButton.onClick.AddListener(OnSpeedupClicked);
    UpgradeArmor.onClick.AddListener(OnUpgradeArmorClicked);
  }

  private void OnDelayClicked()
  {
    AnchorService.Instance.SnailAction(!Web3.Rpc.NodeAddress.AbsoluteUri.Contains("localhost"), () =>
    {
      Debug.Log("Success snail interact");
    }, 0);
    Close();
  }

  private void OnSpeedupClicked()
  {
    AnchorService.Instance.SnailAction(!Web3.Rpc.NodeAddress.AbsoluteUri.Contains("localhost"), () =>
    {
      Debug.Log("Success snail interact");
    }, 1);
    Close();
  }

  private void OnUpgradeArmorClicked()
  {
    AnchorService.Instance.SnailAction(!Web3.Rpc.NodeAddress.AbsoluteUri.Contains("localhost"), () =>
    {
      Debug.Log("Success snail interact");
    }, 2);
    Close();
  }

  private void OnUpgradeWeaponClicked()
  {
    AnchorService.Instance.SnailAction(!Web3.Rpc.NodeAddress.AbsoluteUri.Contains("localhost"), () =>
    {
      Debug.Log("Success snail interact");
    }, 3);
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
