using Frictionless;
using Lastforever.Accounts;
using Solana.Unity.SDK;
using Solana.Unity.Wallet;
using UnityEngine;

public class SnailController : MonoBehaviour
{

    public SnailView SnailViewPrefab;
    public GameObject SnailsRoot;

    void Start()
    {
      AnchorService.OnGameDataChanged += OnGameDataChanged;
      Web3.OnLogin += OnLogin;
    }

    private void OnDestroy()
    {
      AnchorService.OnGameDataChanged -= OnGameDataChanged;
      Web3.OnLogin -= OnLogin;
    }

    private void OnLogin(Account obj)
    {
      OnGameDataChanged(AnchorService.Instance.CurrentGameData);
    }

    private void OnGameDataChanged(GameData gameData)
    {
      foreach (Transform transform in SnailsRoot.transform)
      {
        Destroy(transform.gameObject);
      }

      foreach (var snailData in gameData.Snails)
      {
        var newSnail = Instantiate(SnailViewPrefab, SnailsRoot.transform);
        newSnail.Init(snailData, onClick);
      }
    }

    private void onClick(SnailView snailView)
    {
      ServiceFactory.Resolve<SnailContextMenu>().Open(true, snailView.SnailData);
    }
}
