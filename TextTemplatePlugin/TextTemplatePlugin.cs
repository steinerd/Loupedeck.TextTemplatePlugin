namespace Loupedeck.TextTemplatePlugin
{
    using System;
    using System.IO;

    public class TextTemplatePlugin : Plugin
    {
        internal static TextTemplatePlugin Instance { get; private set; }

        internal const string DEFAULT_PATH = @".loupedeck\text-templates";
        internal static string UserProfilePath { get => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); }

        public override Boolean HasNoApplication => true;
        public override Boolean UsesApplicationApiOnly => true;

        public TextTemplatePlugin()
        {
            Instance = this;
        }

        public override void Load() => this.LoadResources();

        public override void Unload() { }

        private void OnApplicationStarted(Object sender, EventArgs e){}

        private void OnApplicationStopped(Object sender, EventArgs e){}

        public override void RunCommand(String commandName, String parameter){}

        public override void ApplyAdjustment(String adjustmentName, String parameter, Int32 diff){}

        private void LoadResources()
        {
            this.Info.Icon16x16 = EmbeddedResources.ReadImage("Loupedeck.TextTemplatePlugin.Resources.16.png");
            this.Info.Icon32x32 = EmbeddedResources.ReadImage("Loupedeck.TextTemplatePlugin.Resources.32.png");
            this.Info.Icon48x48 = EmbeddedResources.ReadImage("Loupedeck.TextTemplatePlugin.Resources.48.png");
            this.Info.Icon256x256 = EmbeddedResources.ReadImage("Loupedeck.TextTemplatePlugin.Resources.256.png");

            Directory.CreateDirectory(Path.Combine(UserProfilePath, DEFAULT_PATH));
        }
    }
}
