using System.Collections;
using System.Collections.Generic;
using Frictionless;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ErrorPopup : MonoBehaviour
{
  public GameObject Root;
  public Button CloseButton;

  public TextMeshProUGUI Text;
    void Start()
    {
      ServiceFactory.RegisterSingleton(this);
      CloseButton.onClick.AddListener(OnCloseClicked);
      Root.gameObject.SetActive(false);
    }

    public void Open(string text)
    {
      Text.text = text;
      Root.gameObject.SetActive(true);
    }

    private void OnCloseClicked()
    {
      Root.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
