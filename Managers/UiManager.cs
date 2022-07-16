using MerinoClient.Core.UI.QuickMenu;
using MerinoClient.Core.VRChat;
using UnityEngine;

namespace MerinoClient.Core.Managers
{
    public class UiManager
    {
        public IButtonPage MainMenu { get; }
        public IButtonPage TargetMenu { get; }
        public IButtonPage TargetRemoteMenu { get; }

        public UiManager(string menuName, Sprite menuSprite, bool createTargetMenu = true, bool createRemoteTargetMenu = true)
        {
            MainMenu = new MenuPage(menuName, true);
            TabButton.Create(menuName, $"Open the {menuName} menu.", menuName, menuSprite);

            if (createTargetMenu)
            {
                var localMenu = new CategoryPage(QuickMenuEx.SelectedUserLocal.transform);
                TargetMenu = localMenu.AddCategory($"{menuName}");
            }

            if (createRemoteTargetMenu)
            {
                var localMenu = new CategoryPage(QuickMenuEx.SelectedUserRemote.transform);
                TargetRemoteMenu = localMenu.AddCategory($"{menuName}");
            }
        }
    }
}
