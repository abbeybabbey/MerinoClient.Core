using System;
using System.Linq;
using MelonLoader;
using MerinoClient.Core.Managers;

namespace MerinoClient.Core
{
    public class ConfigValue<T>
    {
        public event Action OnValueChanged;

        private readonly MelonPreferences_Entry<T> _entry;

        public T Value => _entry.Value;

        public T DefaultValue => _entry.DefaultValue;

        public string DisplayName => _entry.DisplayName;

        public string Name => _entry.Identifier;

        public ConfigValue(string name, T defaultValue, string displayName = null, MelonPreferences_Category customCategory = null, string description = null,  bool isHidden = true)
        {
            var category = MelonPreferences.CreateCategory(customCategory == null ? ConfigManager.Instance.CategoryName : customCategory.Identifier);

            var entryName = string.Concat(name.Where(c => char.IsLetter(c) || char.IsNumber(c)));
            _entry = category.GetEntry<T>(entryName) ?? category.CreateEntry(entryName, defaultValue, displayName, description, isHidden);
            _entry.OnValueChangedUntyped += () => OnValueChanged?.Invoke();
        }

        public static implicit operator T(ConfigValue<T> conf)
        {
            return conf._entry.Value;
        }

        public void SetValue(T value)
        {
            _entry.Value = value;
            MelonPreferences.Save();
        }

        public void SoftSetValue(T value)
        {
            //TODO: make a better solution for MirroredWingToggle not saving preferences every-time it's made
            _entry.Value = value;
        }

        public override string ToString()
        {
            return _entry.Value.ToString();
        }
    }
}
