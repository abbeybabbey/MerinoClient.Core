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

namespace MerinoClient.Core.UI.QuickMenu
{
    public class CategoryPage : UiElement
    {
        private static GameObject _menuPrefab;
        private static GameObject MenuPrefab
        {
            get
            {
                if (_menuPrefab == null)
                {
                    _menuPrefab = QuickMenuEx.Instance.field_Public_Transform_0.Find("Window/QMParent/Menu_Dashboard").gameObject;
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

        public CategoryPage(string text, bool isRoot = false) : base(MenuPrefab, QuickMenuEx.MenuParent, $"Menu_{text}", false)
        {
            if (!_fixedLaunchpad)
            {
                FixLaunchpadScrolling();
                _fixedLaunchpad = true;

                // We just instantiated a possibly non-scrollable ui page. Let's fix it for our new one just in case.
                var scrollRect = RectTransform.GetComponentInChildren<ScrollRect>();
                scrollRect.content.GetComponent<VerticalLayoutGroup>().childControlHeight = true;
                scrollRect.enabled = true;
                scrollRect.verticalScrollbar = scrollRect.transform.Find("Scrollbar").GetComponent<Scrollbar>();
                scrollRect.viewport.GetComponent<RectMask2D>().enabled = true;
            }

            Object.DestroyImmediate(GameObject.GetComponent<LaunchPadQMMenu>());

            RectTransform.SetSiblingIndex(SiblingIndex);

            _isRoot = isRoot;
            var headerTransform = RectTransform.GetChild(0);
            Object.DestroyImmediate(headerTransform.Find("RightItemContainer/Button_QM_Expand").gameObject);

            var titleText = headerTransform.GetComponentInChildren<TextMeshProUGUI>();
            titleText.text = text;
            titleText.richText = true;


            if (!_isRoot)
            {
                var backButton = headerTransform.GetComponentInChildren<Button>(true);
                backButton.gameObject.SetActive(true);
            }

            _container = RectTransform.GetComponentInChildren<ScrollRect>().content;
            foreach (var obj in _container)
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
            UiPage.field_Public_String_0 = $"QuickMenuReMod{GetCleanName(text)}";
            UiPage.field_Private_Boolean_1 = true;
            UiPage.field_Protected_MenuStateController_0 = QuickMenuEx.MenuStateCtrl;
            UiPage.field_Private_List_1_UIPage_0 = new Il2CppSystem.Collections.Generic.List<UIPage>();
            UiPage.field_Private_List_1_UIPage_0.Add(UiPage);

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

        public CategoryPage(Transform transform) : base(transform)
        {
            UiPage = GameObject.GetComponent<UIPage>();
            _isRoot = QuickMenuEx.MenuStateCtrl.field_Public_ArrayOf_UIPage_0.Contains(UiPage);
            _container = RectTransform.GetComponentInChildren<ScrollRect>().content;
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

        public MenuCategory AddCategory(string title, bool collapsible = true)
        {
            return GetCategory(title) ?? new MenuCategory(title, _container, collapsible);
        }

        public MenuCategory AddCategory(string title)
        {
            return GetCategory(title) ?? new MenuCategory(title, _container);
        }

        public MenuCategory GetCategory(string name)
        {
            var headerTransform = _container.Find($"Header_{GetCleanName(name)}");
            if (headerTransform == null) return null;

            var header = new MenuHeader(headerTransform);
            var buttonContainer = new MenuButtonContainer(_container.Find($"Buttons_{GetCleanName(name)}"));
            return new MenuCategory(header, buttonContainer);
        }
        
        public MenuSliderCategory AddSliderCategory(string title)
        {
            return AddSliderCategory(title, true);
        }

        public MenuSliderCategory AddSliderCategory(string title, bool collapsible = true)
        {
            return GetSliderCategory(title) ?? new MenuSliderCategory(title, _container, collapsible);
        }

        public MenuSliderCategory GetSliderCategory(string name)
        {
            var headerTransform = _container.Find($"Header_{GetCleanName(name)}");
            if (headerTransform == null) return null;

            var header = new MenuHeader(headerTransform);
            var sliderContainer = new ReMenuSliderContainer(_container.Find($"Buttons_{GetCleanName(name)}"));
            return new MenuSliderCategory(header, sliderContainer);
        }

        public static CategoryPage Create(string text, bool isRoot)
        {
            return new CategoryPage(text, isRoot);
        }

        private static bool _fixedLaunchpad;
        private static void FixLaunchpadScrolling()
        {
            var dashboard = QuickMenuEx.Instance.field_Public_Transform_0.Find("Window/QMParent/Menu_Dashboard").GetComponent<UIPage>();
            var scrollRect = dashboard.GetComponentInChildren<ScrollRect>();

            scrollRect.content.GetComponent<VerticalLayoutGroup>().childControlHeight = true;
            scrollRect.enabled = true;
            scrollRect.verticalScrollbar = scrollRect.transform.Find("Scrollbar").GetComponent<Scrollbar>(); ;
            scrollRect.viewport.GetComponent<RectMask2D>().enabled = true;
        }
    }
}
