using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ControllerLayout : MonoBehaviour
{
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private ControllerLayout _keyboardLayout;

    private void OnEnable()
    {
        if (_nextButton) _nextButton.Select();
        else _backButton.Select();

        _backButton.onClick.AddListener(() =>
        {
            _backButton.onClick.RemoveAllListeners();
            gameObject.SetActive(false);
        });
    }
    
    private void OnDestroy()
    {
        _backButton.onClick.RemoveAllListeners();
        
    }

    public void SetBackButton(UnityAction callback)
    {
        _backButton.onClick.AddListener(callback);
    }

    public void GoNextButton()
    {
        void Back()
        {
            gameObject.SetActive(true);
        }
        _keyboardLayout.gameObject.SetActive(true);
        _keyboardLayout.SetBackButton(Back);
        gameObject.SetActive(false);
    }
}
