using System;
using Lastforever.Types;
using UnityEngine;
using UnityEngine.UI;

public class SnailView : MonoBehaviour
{

  public float Y_PositionFrom;
  public float Y_PositionTo;

  public Button Button;

  public SnailData SnailData;
  private Action<SnailView> onClick;

  private void Awake()
  {
    Button.onClick.AddListener(OnButtonClicked);
  }

  private void OnButtonClicked()
  {
    onClick?.Invoke(this);
  }

  public void Init(SnailData data, Action<SnailView> onClick)
  {
    this.onClick = onClick;
    SnailData = data;
    UpdatePosition(data);
  }

  private void UpdatePosition(SnailData data)
  {
    var y_newPos = AnchorService.Instance.CalculateCurrentPosition(data);
    transform.localPosition = new Vector3(0, y_newPos, 0);
  }

  private void Update()
  {
    if (SnailData == null)
    {
      return;
    }
    UpdatePosition(SnailData);
  }
}
