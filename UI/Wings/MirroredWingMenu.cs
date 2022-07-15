using System;
using MerinoClient.Core.VRChat;
using UnityEngine;

namespace MerinoClient.Core.UI.Wings
{
    public class MirroredWingMenu
    {
        private WingMenu _leftMenu;
        private WingMenu _rightMenu;

        public bool Active
        {
            get => _leftMenu.Active && _rightMenu.Active;
            set
            {
                _leftMenu.Active = value;
                _rightMenu.Active = value;
            }
        }

        public MirroredWingMenu(string text, string tooltip, Transform leftParent, Transform rightParent, Sprite sprite = null, bool arrow = true, bool background = true, bool separator = false)
        {
            _leftMenu = new WingMenu(text);
            _rightMenu = new WingMenu(text, false);

            WingButton.Create(text, tooltip, _leftMenu.Open, leftParent, sprite, arrow, background, separator);
            WingButton.Create(text, tooltip, _rightMenu.Open, rightParent, sprite, arrow, background, separator);
        }

        public static MirroredWingMenu Create(string text, string tooltip, Sprite sprite = null, bool arrow = true,
            bool background = true, bool separator = false)
        {
            return new MirroredWingMenu(text, tooltip,
                QuickMenuEx.LeftWing.field_Public_RectTransform_0.Find("WingMenu/ScrollRect/Viewport/VerticalLayoutGroup"),
                QuickMenuEx.RightWing.field_Public_RectTransform_0.Find("WingMenu/ScrollRect/Viewport/VerticalLayoutGroup"),
                sprite, arrow, background, separator);
        }

        public MirroredWingButton AddButton(string text, string tooltip, Action onClick, Sprite sprite = null, bool arrow = true, bool background = true,
            bool separator = false)
        {
            if (_leftMenu == null || _rightMenu == null)
            {
                throw new NullReferenceException("This wing menu has been destroyed.");
            }

            return new MirroredWingButton(text, tooltip, onClick, _leftMenu.Container, _rightMenu.Container, sprite, arrow, background, separator);
        }

        public MirroredWingToggle AddToggle(string text, string tooltip, Action<bool> onToggle, bool defaultValue)
        {
            if (_leftMenu == null || _rightMenu == null)
            {
                throw new NullReferenceException("This wing menu has been destroyed.");
            }

            return new MirroredWingToggle(text, tooltip, onToggle, _leftMenu.Container, _rightMenu.Container,
                defaultValue);
        }

        public MirroredWingMenu AddSubMenu(string text, string tooltip, Sprite sprite = null, bool arrow = true,
            bool background = true, bool separator = false)
        {
            if (_leftMenu == null || _rightMenu == null)
            {
                throw new NullReferenceException("This wing menu has been destroyed.");
            }

            return new MirroredWingMenu(text, tooltip, _leftMenu.Container, _rightMenu.Container, sprite, arrow,
                background, separator);
        }

        public void Destroy()
        {
            _leftMenu.Destroy();
            _rightMenu.Destroy();

            _leftMenu = null;
            _rightMenu = null;
        }
    }
}