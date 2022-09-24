using Microsoft.Xna.Framework.Input;
using System;

namespace Celeste.Mod.SSMQoLMod
{
    public class SSMQoLSettings : EverestModuleSettings
    {
        public bool FastLookout { get; set; } = true;

        public int FastLookoutMultiplier { get; set; } = 4;

        [DefaultButtonBinding(Buttons.LeftTrigger, Keys.LeftShift)]
        public ButtonBinding FastLookoutButton { get; set; }

        [SettingSubText("MODOPTIONS_SSMQOL_SKIPPABLEPOSTCARDS_SUBTEXT")]
        public bool SkippablePostcards { get; set; } = true;

        [SettingIgnore]
        public bool SkipCSidePostcardWait { get; set; } = true;

        [SettingSubText("MODOPTIONS_SSMQOL_SKIPPABLEBSIDEINTRO_SUBTEXT")]
        public bool SkippableBSideIntro { get; set; } = true;

        public bool DisableLowStaminaFlashing { get; set; } = false;

        [SettingSubText("MODOPTIONS_SSMQOL_KEEPTOGGLEGRABONDEATH_SUBTEXT")]
        public bool KeepToggleGrabOnDeath { get; set; } = false;

        public void CreateFastLookoutEntry(TextMenu menu, bool inGame)
        {
            TextMenu.OnOff entry = new TextMenu.OnOff(Dialog.Clean("MODOPTIONS_SSMQOL_FASTLOOKOUT"), FastLookout);
            menu.Add(entry);
            entry.AddDescription(menu, Dialog.Clean("MODOPTIONS_SSMQOL_FASTLOOKOUT_SUBTEXT"));
            entry.Change(value =>
            {
                FastLookout = value;
                // find multiplier option and disable it if this option is set to false
                foreach (TextMenu.Item item in menu.Items)
                {
                    if ((item as TextMenu.Slider)?.Label == Dialog.Clean("MODOPTIONS_SSMQOL_FASTLOOKOUTMULTIPLIER"))
                    {
                        item.Disabled = !value;
                        break;
                    }
                }
            });
        }

        public void CreateFastLookoutMultiplierEntry(TextMenu menu, bool inGame)
        {
            TextMenu.Slider entry = new TextMenu.Slider(
                Dialog.Clean("MODOPTIONS_SSMQOL_FASTLOOKOUTMULTIPLIER"), i => $"{i}x", 2, 10, FastLookoutMultiplier);
            menu.Add(entry);
            entry.Disabled = !FastLookout;
            entry.Change(value => FastLookoutMultiplier = value);
        }
    }
}
