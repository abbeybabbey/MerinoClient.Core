﻿using System;
using MerinoClient.Core.VRChat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.DataModel.Core;
using VRC.UI.Core.Styles;
using VRC.UI.Elements;
using VRC.UI.Elements.Controls;
using Object = UnityEngine.Object;

namespace MerinoClient.Core.UI.QuickMenu
{
    public class RadioToggle : UiElement
    {

        private static GameObject _togglePrefab;
        
        private static GameObject TogglePrefab
        {
            get
            {
                if (_togglePrefab == null)
                {
                    var audioMenuSource = QuickMenuEx.Instance.field_Public_Transform_0.Find("Window/QMParent/Menu_ChangeAudioInputDevice").gameObject;
                    var deviceMenu = audioMenuSource.GetComponent<AudioInputDeviceMenu>();
                    _togglePrefab = deviceMenu.field_Public_GameObject_0;
                }
                return _togglePrefab;
            }
        }

        public bool IsOn;
        public Action<RadioToggle, bool> ToggleStateUpdated;
        public System.Object ToggleData;

        private Toggle _toggle;
        private Graphic _checkmark;
        private readonly TMP_Text _text;
        private StyleElement _style;

        public RadioToggle(Transform parent, string name, string text, System.Object obj, bool defaultState = false) : base(TogglePrefab, parent, $"ReRadioToggle_{GetCleanName(name)}")
        {
            Object.DestroyImmediate(RectTransform.GetComponent<AudioDeviceButton>());
            Object.DestroyImmediate(RectTransform.GetComponent<RadioButtonSelector>());
            Object.DestroyImmediate(RectTransform.GetComponent<DataContext>());
            Object.DestroyImmediate(RectTransform.GetComponentInChildren<ListCountBinding>());
            
            var button = RectTransform.GetComponent<Button>();
            _toggle = RectTransform.GetComponentInChildren<Toggle>(true);
            _checkmark = _toggle.graphic;
            _text = RectTransform.GetComponentInChildren<TMP_Text>(true);
            _style = RectTransform.GetComponent<StyleElement>();

            _text.text = text;
            ToggleData = obj;
            
            SetToggle(defaultState);
            
            button.onClick.AddListener(new Action(ToggleOn));
        }

        public void SetToggle(bool state)
        {
            IsOn = state;
            _checkmark.gameObject.active = IsOn;
            _toggle.Set(IsOn);
        }

        private void ToggleOn()
        {
            if(IsOn) return;
            
            IsOn = true;
            _checkmark.gameObject.active = IsOn;
            _toggle.Set(IsOn);
            ToggleStateUpdated?.Invoke(this, IsOn);
        }
    }
}