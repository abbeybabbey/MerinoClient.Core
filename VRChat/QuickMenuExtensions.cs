﻿using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI;
using VRC.UI.Elements;
using VRC.UI.Elements.Controls;
using VRC.UI.Elements.Menus;

namespace MerinoClient.Core.VRChat
{
    public static class QuickMenuExtensions
    {
        private delegate void ShowModalAlertDelegate(ModalAlert modalAlert, string text);
        private static ShowModalAlertDelegate _showModalAlert;
        private static ShowModalAlertDelegate ShowModalAlertFn
        {
            get
            {
                if (_showModalAlert != null)
                    return _showModalAlert;

                var showModalAlertFn = typeof(ModalAlert).GetMethods().First(method =>
                    method.Name.StartsWith("Method_Public_Void_String_") && method.GetParameters().Length == 1 &&
                    XrefUtils.CheckUsedBy(method, "Method_Public_Virtual_Final_New_Void_String_"));

                _showModalAlert =
                    (ShowModalAlertDelegate)Delegate.CreateDelegate(typeof(ShowModalAlertDelegate), showModalAlertFn);

                return _showModalAlert;
            }
        }

        private delegate void ShowConfirmDialogDelegate(UIMenu uiMenu, string title, string body, Il2CppSystem.Action acceptAction, Il2CppSystem.Action declineAction = null, string confirmText = "Yes", string declineText = "No");
        private static ShowConfirmDialogDelegate _showConfirmDialogDelegate;
        private static ShowConfirmDialogDelegate ShowConfirmDialogFn
        {
            get
            {
                if (_showConfirmDialogDelegate != null)
                    return _showConfirmDialogDelegate;

                var showConfirmDialogFn = typeof(UIMenu).GetMethods().FirstOrDefault(m =>
                {
                    if (!m.Name.Contains("Public_Void_String_String_Action_Action_String_String_"))
                        return false;
                    
                    return XrefUtils.CheckMethod(m, "ConfirmDialog");
                });

                _showConfirmDialogDelegate = (ShowConfirmDialogDelegate)Delegate.CreateDelegate(typeof(ShowConfirmDialogDelegate), showConfirmDialogFn);

                return _showConfirmDialogDelegate;
            }
        }

        public static bool IsActive(this VRC.UI.Elements.QuickMenu quickMenu)
        {
            return quickMenu.gameObject.activeSelf;
        }

        private delegate void ShowConfirmDialogWithCancelDelegate(UIMenu uiMenu, string title, string body, string yesLabel, string noLabel, string cancelLabel, Il2CppSystem.Action onYes, Il2CppSystem.Action onNo, Il2CppSystem.Action onCancel);
        private static ShowConfirmDialogWithCancelDelegate _showConfirmDialogWithCancelDelegate;
        private static ShowConfirmDialogWithCancelDelegate ShowConfirmDialogWithCancelFn
        {
            get
            {
                if (_showConfirmDialogWithCancelDelegate != null)
                    return _showConfirmDialogWithCancelDelegate;

                var showConfirmDialogWithCancelFn = typeof(UIMenu).GetMethods().FirstOrDefault(m =>
                {
                    if (!m.Name.Contains("Method_Public_Void_String_String_String_String_String_Action_Action_Action_"))
                        return false;
                    
                    return XrefUtils.CheckMethod(m, "ConfirmDialog");
                });

                _showConfirmDialogWithCancelDelegate = (ShowConfirmDialogWithCancelDelegate)Delegate.CreateDelegate(typeof(ShowConfirmDialogWithCancelDelegate), showConfirmDialogWithCancelFn);
                return _showConfirmDialogWithCancelDelegate;
            }
        }

        private delegate void ShowAlertDialogDelegate(UIMenu uiMenu, string title, string body, Il2CppSystem.Action onClose, string closeText = "Close", bool unknown = false);
        private static ShowAlertDialogDelegate _showAlertDialogDelegate;
        private static ShowAlertDialogDelegate ShowAlertDialogFn
        {
            get
            {
                if (_showAlertDialogDelegate != null)
                    return _showAlertDialogDelegate;
                
                var showAlertDialogFn = typeof(UIMenu).GetMethods().FirstOrDefault(m =>
                {
                    if (!m.Name.Contains("Method_Public_Void_String_String_Action_String_Boolean_PDM"))
                        return false;
                    
                    return XrefUtils.CheckMethod(m, "ConfirmDialog");
                });

                _showAlertDialogDelegate = (ShowAlertDialogDelegate)Delegate.CreateDelegate(typeof(ShowAlertDialogDelegate), showAlertDialogFn);
                return _showAlertDialogDelegate;
            }
        }

        public static void ShowConfirmDialog(this UIMenu uiMenu, string title, string body, Action onYes, Action onNo = null)
        {
            ShowConfirmDialog(uiMenu, title, body, "Yes", "No", onYes, onNo);
        }
        
        public static void ShowConfirmDialog(this UIMenu uiMenu, string title, string body, string confirmText, string declineText, Action onYes, Action onNo=null)
        {
            ShowConfirmDialogFn.Invoke(uiMenu, title, body, onYes, onNo, confirmText, declineText);
        }

        public static void ShowConfirmDialogWithCancel(this UIMenu uiMenu, string title, string body, string yesLabel, string noLabel, string cancelLabel, Action onYes, Action onNo, Action onCancel)
        {
            ShowConfirmDialogWithCancelFn.Invoke(uiMenu, title, body, yesLabel, noLabel, cancelLabel, onYes, onNo, onCancel);
        }

        public static void ShowAlertDialog(this UIMenu uiMenu, string title, string body, Action onClose = null)
        {
            ShowAlertDialog(uiMenu, title, body, "Close", onClose);
        }
        
        public static void ShowAlertDialog(this UIMenu uiMenu, string title, string body, string closeText, Action onClose = null)
        {
            ShowAlertDialogFn.Invoke(uiMenu, title, body, onClose, closeText);
        }

        public static void ShowModalAlert(this ModalAlert modalAlert, string alertText)
        {
            ShowModalAlertFn.Invoke(modalAlert, alertText);
        }
        
        private static MethodInfo _closeQuickMenuMethod;

        public static void CloseQuickMenu(this UIManagerImpl uiManager)
        {
            if (_closeQuickMenuMethod == null)
            {
                var goSetActive = typeof(GameObject).GetMethod(nameof(GameObject.SetActive));
                _closeQuickMenuMethod = typeof(UIManagerImpl).GetMethods()
                    .First(method => method.Name.StartsWith("Method_Public_Void_Boolean_") && XrefUtils.CheckUsing(method, goSetActive?.Name));
            }
            _closeQuickMenuMethod.Invoke(uiManager, new object[1] { true });
        }

        public static ModalAlert GetModalAlert(this UIMenu uiMenu)
        {
            return uiMenu.field_Private_ModalAlert_0;
        }

        public static void AddScrollBar(this SelectedUserMenuQM selectedUserMenuQm)
        {
            var transform = selectedUserMenuQm.transform;

            // Get scroll stuff
            var scrollRect = transform.Find("ScrollRect").GetComponent<ScrollRect>();
            scrollRect.enabled = true;

            // Fix scrolling
            var scrollbar = scrollRect.transform.Find("Scrollbar");
            scrollbar.gameObject.SetActive(true);

            scrollRect.enabled = true;
            scrollRect.verticalScrollbar = scrollbar.GetComponent<Scrollbar>();
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
            scrollRect.viewport.GetComponent<RectMask2D>().enabled = true;
        }
    }
}