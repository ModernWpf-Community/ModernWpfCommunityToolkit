using Emoji.Wpf;
using ModernWpf.Toolkit.UI.Controls.Markdown.Inlines;

namespace ModernWpf.Toolkit.SampleApp
{
    public class EmojiUIElementInline : EmojiInline, IUIElementInline
    {
        public string GetUIContentString() => Text;
    }
}
