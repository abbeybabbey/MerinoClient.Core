using System;
using UnityEngine;

namespace MerinoClient.Core.UI.QuickMenu;

public interface IButtonPage
{
    MenuButton AddButton(string text, string tooltip, Action onClick, Sprite sprite = null);
    MenuButton AddSpacer(Sprite sprite = null);
    MenuPage AddMenuPage(string text, string tooltip = "", Sprite sprite = null);
    CategoryPage AddCategoryPage(string text, string tooltip = "", Sprite sprite = null);
    MenuToggle AddToggle(string text, string tooltip, Action<bool> onToggle, bool defaultValue = false);
    MenuToggle AddToggle(string text, string tooltip, ConfigValue<bool> configValue);
    MenuToggle AddToggle(string text, string tooltip, Action<bool> onToggle, bool defaultValue, Sprite iconOn, Sprite iconOff);
    MenuToggle AddToggle(string text, string tooltip, ConfigValue<bool> configValue, Sprite iconOn, Sprite iconOff);
    MenuPage GetMenuPage(string name);
    CategoryPage GetCategoryPage(string name);
    void AddCategoryPage(string text, string tooltip, Action<CategoryPage> onPageBuilt, Sprite sprite = null);
    void AddMenuPage(string text, string tooltip, Action<MenuPage> onPageBuilt, Sprite sprite = null);
}