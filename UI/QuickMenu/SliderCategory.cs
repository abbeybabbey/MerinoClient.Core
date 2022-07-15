using System;
using MerinoClient.Core.VRChat;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MerinoClient.Core.UI.QuickMenu
{
    public class ReMenuSliderContainer : UiElement
    {
        private static GameObject _containerPrefab;

        private static GameObject ContainerPrefab
        {                       
            get
            {
                if (_containerPrefab == null)
                {
                    _containerPrefab = QuickMenuEx.Instance.field_Public_Transform_0
                        .Find("Window/QMParent/Menu_AudioSettings/Content").gameObject;
                }
                return _containerPrefab;
            }
        }

        public ReMenuSliderContainer(string name, Transform parent = null) : base(ContainerPrefab, parent == null ? ContainerPrefab.transform.parent : parent, $"Sliders_{name}")
        {
            foreach (var obj in RectTransform)
            {
                var control = obj.Cast<Transform>();
                if (control == null)
                {
                    continue;
                }
                Object.Destroy(control.gameObject);
            }

            var vlg = GameObject.GetComponent<VerticalLayoutGroup>();
            vlg.m_Padding = new RectOffset(64, 64, 0, 0);
        }

        public ReMenuSliderContainer(Transform transform) : base(transform)
        {
        }
    }

    public class MenuSliderCategory
    {
        public readonly MenuHeader Header;
        private readonly ReMenuSliderContainer _sliderContainer;

        public string Title
        {
            get => Header.Title;
            set => Header.Title = value;
        }

        public bool Active
        {
            get => _sliderContainer.Active;
            set
            {
                Header.Active = value;
                _sliderContainer.Active = value;
            }
        }

        public MenuSliderCategory(string title, Transform parent = null, bool collapsible = true)
        {
            if (collapsible)
            {
                var header = new MenuHeaderCollapsible(title, parent);
                header.OnToggle += b => _sliderContainer!.GameObject.SetActive(b);
                Header = header;
            }

            else
            {
                var header = new MenuHeader(title, parent);
                Header = header;
            }
            _sliderContainer = new ReMenuSliderContainer(title, parent);

        }

        public MenuSliderCategory(MenuHeader headerElement, ReMenuSliderContainer container)
        {
            Header = headerElement;
            _sliderContainer = container;
        }

        public MenuSlider AddSlider(string text, string tooltip, Action<float> onSlide, float defaultValue = 0, float minValue = 0, float maxValue = 10)
        {
            var slider = new MenuSlider(text, tooltip, onSlide, _sliderContainer.RectTransform, defaultValue, minValue, maxValue);
            return slider;
        }

        public MenuSlider AddSlider(string text, string tooltip, ConfigValue<float> configValue, float defaultValue = 0, float minValue = 0, float maxValue = 10)
        {
            var slider = new MenuSlider(text, tooltip, configValue.SetValue, _sliderContainer.RectTransform, configValue, minValue, maxValue);
            return slider;
        }

    }
}
