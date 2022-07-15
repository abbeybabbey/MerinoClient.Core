using System;
using MerinoClient.Core.VRChat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;
using Object = UnityEngine.Object;

namespace MerinoClient.Core.UI.QuickMenu
{
    public class MenuHeader : UiElement
    {
        private static GameObject _headerPrefab;
        private static GameObject HeaderPrefab
        {
            get
            {
                if (_headerPrefab == null)
                {
                    _headerPrefab = QuickMenuEx.Instance.field_Public_Transform_0
                        .Find("Window/QMParent/Menu_Dashboard/ScrollRect").GetComponent<ScrollRect>().content
                        .Find("Header_QuickActions").gameObject;
                }
                return _headerPrefab;
            }
        }

        protected TextMeshProUGUI TextComponent;
        public string Title
        {
            get => TextComponent.text;
            set => TextComponent.text = value;
        }

        public MenuHeader(string title, Transform parent) : base(HeaderPrefab, (parent == null ? HeaderPrefab.transform.parent : parent), $"Header_{title}")
        {
            TextComponent = GameObject.GetComponentInChildren<TextMeshProUGUI>();
            TextComponent.text = title;
            TextComponent.richText = true;

            TextComponent.transform.parent.GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
        }

        public MenuHeader(Transform transform) : base(transform)
        {
            TextComponent = GameObject.GetComponentInChildren<TextMeshProUGUI>();
        }

        protected MenuHeader(GameObject original, Transform parent, Vector3 pos, string name, bool defaultState = true) : base(original, parent, pos, name, defaultState) { }
        protected MenuHeader(GameObject original, Transform parent, string name, bool defaultState = true) : base(original, parent, name, defaultState) { }
    }

    public class MenuHeaderCollapsible : MenuHeader
    {
        private static GameObject _headerPrefab;
        private static GameObject HeaderPrefab
        {
            get
            {
                if (_headerPrefab == null)
                {
                    _headerPrefab = QuickMenuEx.Instance.field_Public_Transform_0
                        .Find("Window/QMParent/Menu_Settings/Panel_QM_ScrollRect").GetComponent<ScrollRect>().content
                        .Find("QM_Foldout_UI_Elements").gameObject;
                }
                return _headerPrefab;
            }
        }

        public Action<bool> OnToggle;

        public MenuHeaderCollapsible(string title, Transform parent) : base(HeaderPrefab, (parent == null ? HeaderPrefab.transform.parent : parent), $"Header_{title}")
        {
            TextComponent = GameObject.GetComponentInChildren<TextMeshProUGUI>();
            TextComponent.text = title;
            TextComponent.richText = true;

            var foldout = GameObject.GetComponent<QMFoldout>();
            foldout.field_Private_String_0 = $"UI.ReMod.{GetCleanName(title)}";
            foldout.field_Private_Action_1_Boolean_0 = new Action<bool>(b => OnToggle?.Invoke(b));
        }

        public MenuHeaderCollapsible(Transform transform) : base(transform)
        {
            TextComponent = GameObject.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public class MenuButtonContainer : UiElement
    {
        private static GameObject _containerPrefab;
        private static GameObject ContainerPrefab
        {
            get
            {
                if (_containerPrefab == null)
                {
                    _containerPrefab = QuickMenuEx.Instance.field_Public_Transform_0
                        .Find("Window/QMParent/Menu_Dashboard/ScrollRect").GetComponent<ScrollRect>().content
                        .Find("Buttons_QuickActions").gameObject;
                }
                return _containerPrefab;
            }
        }

        public MenuButtonContainer(string name, Transform parent = null) : base(ContainerPrefab, parent == null ? ContainerPrefab.transform.parent : parent, $"Buttons_{name}")
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

            var gridLayout = GameObject.GetComponent<GridLayoutGroup>();

            gridLayout.childAlignment = TextAnchor.UpperLeft;
            gridLayout.padding.top = 8;
            gridLayout.padding.left = 64;
        }

        public MenuButtonContainer(Transform transform) : base(transform)
        {
        }
    }

    public class MenuCategory : IButtonPage
    {
        public readonly MenuHeader Header;
        private readonly MenuButtonContainer _buttonContainer;

        public string Title
        {
            get => Header.Title;
            set => Header.Title = value;
        }

        public bool Active
        {
            get => _buttonContainer.GameObject.activeInHierarchy;
            set
            {
                Header.Active = value;
                _buttonContainer.Active = value;
            }
        }

        public MenuCategory(string title, Transform parent = null, bool collapsible = true)
        {
            if (collapsible)
            {
                var header = new MenuHeaderCollapsible(title, parent);
                header.OnToggle += b => _buttonContainer!.GameObject.SetActive(b);
                Header = header;
            }

            else
            {
                var header = new MenuHeader(title, parent);
                Header = header;
            }
            _buttonContainer = new MenuButtonContainer(title, parent);

        }

        public MenuCategory(MenuHeader headerElement, MenuButtonContainer container)
        {
            Header = headerElement;
            _buttonContainer = container;
        }

        public MenuButton AddButton(string text, string tooltip, Action onClick, Sprite sprite = null)
        {
            var button = new MenuButton(text, tooltip, onClick, _buttonContainer.RectTransform, sprite);
            return button;
        }
        
        public MenuButton AddSpacer(Sprite sprite = null) {
            var spacer = AddButton(string.Empty, string.Empty, null, sprite);
            spacer.GameObject.name = "Button_Spacer";
            spacer.Background.gameObject.SetActive(false);
            return spacer;
        }

        public MenuToggle AddToggle(string text, string tooltip, Action<bool> onToggle, bool defaultValue = false) 
            => AddToggle(text, tooltip, onToggle, defaultValue, null, null);
        public MenuToggle AddToggle(string text, string tooltip, ConfigValue<bool> configValue)
            => AddToggle(text, tooltip, configValue, null, null);
        public MenuToggle AddToggle(string text, string tooltip, Action<bool> onToggle, bool defaultValue, Sprite iconOn, Sprite iconOff)
        {
            var toggle = new MenuToggle(text, tooltip, onToggle, _buttonContainer.RectTransform, defaultValue, iconOn, iconOff);
            return toggle;
        }
        public MenuToggle AddToggle(string text, string tooltip, ConfigValue<bool> configValue, Sprite iconOn, Sprite iconOff)
        {
            var toggle = new MenuToggle(text, tooltip, configValue.SetValue, _buttonContainer.RectTransform, configValue, iconOn, iconOff);
            return toggle;
        }

        public MenuPage AddMenuPage(string text, string tooltip = "", Sprite sprite = null)
        {
            var existingPage = GetMenuPage(text);
            if (existingPage != null)
            {
                return existingPage;
            }

            var menu = new MenuPage(text);
            AddButton(text, string.IsNullOrEmpty(tooltip) ? $"Open the {text} menu" : tooltip, menu.Open, sprite);
            return menu;
        }

        public CategoryPage AddCategoryPage(string text, string tooltip = "", Sprite sprite = null)
        {
            var existingPage = GetCategoryPage(text);
            if (existingPage != null)
            {
                return existingPage;
            }

            var menu = new CategoryPage(text);
            AddButton(text, string.IsNullOrEmpty(tooltip) ? $"Open the {text} menu" : tooltip, menu.Open, sprite);
            return menu;
        }

        public void AddMenuPage(string text, string tooltip, Action<MenuPage> onPageBuilt, Sprite sprite = null)
        {
            onPageBuilt(AddMenuPage(text, tooltip, sprite));
        }

        public void AddCategoryPage(string text, string tooltip, Action<CategoryPage> onPageBuilt, Sprite sprite = null)
        {
            onPageBuilt(AddCategoryPage(text, tooltip, sprite));
        }

        public RectTransform RectTransform => _buttonContainer.RectTransform;

        public MenuPage GetMenuPage(string name)
        {
            var transform = QuickMenuEx.MenuParent.Find(UiElement.GetCleanName($"Menu_{name}"));
            return transform == null ? null : new MenuPage(transform);
        }

        public CategoryPage GetCategoryPage(string name)
        {
            var transform = QuickMenuEx.MenuParent.Find(UiElement.GetCleanName($"Menu_{name}"));
            return transform == null ? null : new CategoryPage(transform);
        }
    }
}
