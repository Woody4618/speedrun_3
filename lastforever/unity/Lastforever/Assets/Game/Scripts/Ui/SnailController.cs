using Frictionless;
using Lastforever.Accounts;
using Solana.Unity.SDK;
using Solana.Unity.Wallet;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class SnailController : MonoBehaviour
{
    public Button ShowMySnailButton;
    public SnailView SnailViewPrefab;
    public GameObject SnailsRoot;
    public ScrollView ScrollView;
    public ScrollRect ScrollRect;
    public RectTransform ContentPanel;

    void Start()
    {
      AnchorService.OnGameDataChanged += OnGameDataChanged;
      Web3.OnLogin += OnLogin;
      ShowMySnailButton.onClick.AddListener(OnClickMySnailClicked);
    }

    private void OnClickMySnailClicked()
    {
      ScrollToMySnail();
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
        if (snailData.Authority == Web3.Account.PublicKey)
        {
          newSnail.transform.localScale = new Vector3(-1, 1, 1);
        }
        newSnail.Init(snailData, onClick);
      }

      ScrollToMySnail();
    }

    private void ScrollToMySnail()
    {
      if (string.IsNullOrEmpty(Web3.Account.PublicKey))
      {
        Debug.LogError("Public key is not set.");
        return;
      }

      int childCount = SnailsRoot.transform.childCount;

      if (childCount == 0)
      {
        Debug.LogWarning("No snails found.");
        return;
      }

      for (int i = 0; i < SnailsRoot.transform.childCount; i++)
      {
        var snailView = SnailsRoot.transform.GetChild(i).GetComponent<SnailView>();
        if (snailView != null && snailView.SnailData.Authority == Web3.Account.PublicKey)
        {
          //float targetPosition = (childCount > 1) ? (float)i / (childCount - 1) : 0.5f;
          //ScrollRect.verticalNormalizedPosition = Mathf.Clamp01(1 - targetPosition);

          SnapTo(snailView.transform as RectTransform);
          return;
        }
      }

      Debug.LogWarning("Snail with the given authority not found.");
    }

    public void SnapTo(RectTransform target)
    {
      Canvas.ForceUpdateCanvases();

      ContentPanel.anchoredPosition =
        (Vector2)ScrollRect.transform.InverseTransformPoint(ContentPanel.position)
        - (Vector2)ScrollRect.transform.InverseTransformPoint(target.position);
    }

    private void onClick(SnailView snailView)
    {
      ServiceFactory.Resolve<SnailContextMenu>().Open(true, snailView.SnailData);
    }
}
