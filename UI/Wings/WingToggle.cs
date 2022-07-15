using System;
using MerinoClient.Core.VRChat;
using UnityEngine;

namespace MerinoClient.Core.UI.Wings
{
    public class WingToggle
    {
        private readonly WingButton _button;
        private readonly Action<bool> _onToggle;

        private bool _state;
        
        public bool Interactable
        {
            get => _button.Interactable;
            set => _button.Interactable = value;
        }
        
        public WingToggle(string text, string tooltip, Action<bool> onToggle, Transform parent, bool defaultValue = false)
        {
            _onToggle = onToggle;
            _button = new WingButton(text, tooltip, () =>
            {
                Toggle(!_state);
            }, parent, GetCurrentIcon(), false);
            Toggle(defaultValue);
        }

        private Sprite GetCurrentIcon()
        {
            return _state ? QuickMenuEx.OnIconSprite : QuickMenuEx.OffIconSprite;
        }

        public void Toggle(bool b, bool callback = true)
        {
            if (_state == b) return;

            _state = b;
            _button.Sprite = GetCurrentIcon();
            if (callback)
            {
                _onToggle(_state);
            }
        }
    }
}
