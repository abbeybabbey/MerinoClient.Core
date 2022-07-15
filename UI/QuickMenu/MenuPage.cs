using System;
using System.Linq;
using MerinoClient.Core.Unity;
using MerinoClient.Core.VRChat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;
using VRC.UI.Elements.Menus;
using Object = UnityEngine.Object;
// ReSharper disable MethodOverloadWithOptionalParameter

namespace MerinoClient.Core.UI.QuickMenu
{
    public class MenuPage : UiElement, IButtonPage
    {
        private static GameObject _menuPrefab;

        private static GameObject MenuPrefab
        {
            get
            {
                if (_menuPrefab == null)
                {
                    _menuPrefab = QuickMenuEx.Instance.field_Public_Transform_0.Find("Window/QMParent/Menu_DevTools").gameObject;
                }
                return _menuPrefab;
            }
        }

        private static int SiblingIndex => QuickMenuEx.Instance.field_Public_Transform_0.Find("Window/QMParent/Modal_AddMessage").GetSiblingIndex();

        public event Action OnOpen;
        public event Action OnClose;
        private readonly bool _isRoot;

        private readonly Transform _container;

        public UIPage UiPage { get; }

        public MenuPage(string text, bool isRoot = false) : base(MenuPrefab, QuickMenuEx.MenuParent, $"Menu_{text}", false)
        {
            Object.DestroyImmediate(GameObject.GetComponent<DevMenu>());

            RectTransform.SetSiblingIndex(SiblingIndex);

            var menuName = GetCleanName(text);
            _isRoot = isRoot;
            var headerTransform = RectTransform.GetChild(0);
            var titleText = headerTransform.GetComponentInChildren<TextMeshProUGUI>();
            titleText.text = text;
            titleText.richText = true;

            if (!_isRoot)
            {
                var backButton = headerTransform.GetComponentInChildren<Button>(true);
                backButton.gameObject.SetActive(true);
            }

            headerTransform.name = $"Header_{menuName}";

            var buttonContainer = RectTransform.Find("Scrollrect/Viewport/VerticalLayoutGroup/Buttons");
            foreach (var obj in buttonContainer)
            {
                var control = obj.Cast<Transform>();
                if (control == null)
                {
                    continue;
                }
                Object.Destroy(control.gameObject);
            }

            // Set up UIPage
            UiPage = GameObject.AddComponent<UIPage>();
            UiPage.field_Public_String_0 = $"QuickMenuReMod{menuName}";
            UiPage.field_Private_Boolean_1 = true;
            UiPage.field_Protected_MenuStateController_0 = QuickMenuEx.MenuStateCtrl;
            UiPage.field_Private_List_1_UIPage_0 = new Il2CppSystem.Collections.Generic.List<UIPage>();
            UiPage.field_Private_List_1_UIPage_0.Add(UiPage);

            // Get scroll stuff
            var scrollRect = RectTransform.Find("Scrollrect").GetComponent<ScrollRect>();
            _container = scrollRect.content;

            // copy properties of old grid layout
            var gridLayoutGroup = _container.Find("Buttons").GetComponent<GridLayoutGroup>();

            Object.DestroyImmediate(_container.GetComponent<VerticalLayoutGroup>());
            var glp = _container.gameObject.AddComponent<GridLayoutGroup>();
            glp.spacing = gridLayoutGroup.spacing;
            glp.cellSize = gridLayoutGroup.cellSize;
            glp.constraint = gridLayoutGroup.constraint;
            glp.constraintCount = gridLayoutGroup.constraintCount;
            glp.startAxis = gridLayoutGroup.startAxis;
            glp.startCorner = gridLayoutGroup.startCorner;
            glp.childAlignment = TextAnchor.UpperLeft;
            glp.padding = gridLayoutGroup.padding;
            glp.padding.top = 8;
            glp.padding.left = 64;

            // delete components we're not using
            Object.DestroyImmediate(_container.Find("Buttons").gameObject);
            Object.DestroyImmediate(_container.Find("Spacer_8pt").gameObject);

            // Fix scrolling
            var scrollbar = scrollRect.transform.Find("Scrollbar");
            scrollbar.gameObject.SetActive(true);

            scrollRect.enabled = true;
            scrollRect.verticalScrollbar = scrollbar.GetComponent<Scrollbar>();
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
            scrollRect.viewport.GetComponent<RectMask2D>().enabled = true;

            QuickMenuEx.MenuStateCtrl.field_Private_Dictionary_2_String_UIPage_0.Add(UiPage.field_Public_String_0, UiPage);

            if (isRoot)
            {
                var rootPages = QuickMenuEx.MenuStateCtrl.field_Public_ArrayOf_UIPage_0.ToList();
                rootPages.Add(UiPage);
                QuickMenuEx.MenuStateCtrl.field_Public_ArrayOf_UIPage_0 = rootPages.ToArray();
            }

            EnableDisableListener.RegisterSafe();
            var listener = GameObject.AddComponent<EnableDisableListener>();
            listener.OnEnableEvent += () => OnOpen?.Invoke();
            listener.OnDisableEvent += () => OnClose?.Invoke();
        }

        public MenuPage(Transform transform) : base(transform)
        {
            UiPage = GameObject.GetComponent<UIPage>();
            _isRoot = QuickMenuEx.MenuStateCtrl.field_Public_ArrayOf_UIPage_0.Contains(UiPage);
            var scrollRect = RectTransform.Find("Scrollrect").GetComponent<ScrollRect>();
            _container = scrollRect.content;
        }

        public void Open()
        {
            if (_isRoot)
            {
                QuickMenuEx.MenuStateCtrl.SwitchToRootPage(UiPage.field_Public_String_0);
            }
            else
            {
                QuickMenuEx.MenuStateCtrl.PushPage(UiPage.field_Public_String_0);
            }

            OnOpen?.Invoke();
        }

        public MenuButton AddButton(string text, string tooltip, Action onClick, Sprite sprite = null)
        {
            return new MenuButton(text, tooltip, onClick, _container, sprite);
        }

        public MenuButton AddSpacer(Sprite sprite = null)
        {
            var spacer = AddButton(string.Empty, string.Empty, null, sprite);
            spacer.GameObject.name = "Button_Spacer";
            spacer.Background.gameObject.SetActive(false);
            return spacer;
        }

        public MenuToggle AddToggle(string text, string tooltip, Action<bool> onToggle, bool defaultValue = false)
            => AddToggle(text, tooltip, onToggle, defaultValue, null);
        public MenuToggle AddToggle(string text, string tooltip, ConfigValue<bool> configValue)
            => AddToggle(text, tooltip, configValue, null);
        public MenuToggle AddToggle(string text, string tooltip, Action<bool> onToggle, bool defaultValue = false, Sprite iconOn = null, Sprite iconOff = null)
        {
            return new MenuToggle(text, tooltip, onToggle, _container, defaultValue, iconOn, iconOff);
        }
        public MenuToggle AddToggle(string text, string tooltip, ConfigValue<bool> configValue, Sprite iconOn = null, Sprite iconOff = null)
        {
            return new MenuToggle(text, tooltip, configValue.SetValue, _container, configValue, iconOn, iconOff);
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

        public MenuPage GetMenuPage(string name)
        {
            var transform = QuickMenuEx.MenuParent.Find(GetCleanName($"Menu_{name}"));
            return transform == null ? null : new MenuPage(transform);
        }

        public CategoryPage GetCategoryPage(string name)
        {
            var transform = QuickMenuEx.MenuParent.Find(GetCleanName($"Menu_{name}"));
            return transform == null ? null : new CategoryPage(transform);
        }

        public static MenuPage Create(string text, bool isRoot)
        {
            return new MenuPage(text, isRoot);
        }
    }
}
