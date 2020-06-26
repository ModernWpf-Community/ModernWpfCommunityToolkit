using System.Windows.Documents;

namespace ModernWpf.Toolkit.Controls.Markdown.Render
{
    /// <summary>
    /// An interface used to resolve emoji inlines.
    /// </summary>
    public interface IEmojiInlineResolver
    {
        /// <summary>
        /// Resolves an Emoji inline from a emoji sting.
        /// </summary>
        /// <param name="emoji">Emoji string to Resolve.</param>
        /// <returns>Image</returns>
        Inline ResolveEmojiInline(string emoji);
    }
}
