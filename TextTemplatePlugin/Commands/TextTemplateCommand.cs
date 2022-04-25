namespace Loupedeck.TextTemplatePlugin.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Loupedeck.TextTemplatePlugin.Services;

    public class TextTemplateCommand : PluginDynamicCommand
    {
        public static readonly TextTemplateService textTemplateService = new Services.TextTemplateService();

        public TextTemplateCommand() : base("Templates", "Select text template", string.Empty)
        {
            this.MakeProfileAction("list;Select template: ");
        }

        protected override Boolean OnLoad() => base.OnLoad();


        protected override PluginActionParameter[] GetParameters() =>
            TextTemplateService.Templates
            .Select(t => new PluginActionParameter(t.Value.Name, t.Value.Name, String.Empty))
            .ToArray();

        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            return base.GetCommandDisplayName(actionParameter, imageSize);
        }

        protected override void RunCommand(String actionParameter)
        {
            var currentTemplate = TextTemplateService.Templates.FirstOrDefault(t => t.Key == actionParameter);

            var nativeMethods = this.NativeApi.GetNativeMethods();
            var textToSend = currentTemplate.Value.Compile();
            var textToSendByLine = textToSend.Split('\n');

            textToSendByLine.Select((l, i) => new { line = l, index = i }).ToList().ForEach(item =>
            {
                var count = item.index + 1;
                nativeMethods.SendString(item.line);
                if (count != textToSendByLine.Length) nativeMethods.SendKeyboardShortcut(VirtualKeyCode.Return);                
            });
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            return base.GetCommandImage(actionParameter, imageSize);
        }
    }
}
