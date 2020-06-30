using System;
using System.Windows.Documents;

namespace ModernWpf.Toolkit.Controls
{
    /// <summary>
    /// Arguments for the <see cref="MarkdownTextBlock.EmojiInlineResolving"/> event which is called when a emoji needs to be resolved to a <see cref="Inline"/>.
    /// </summary>
    public class EmojiInlineResolvingEventArgs : EventArgs
    {
        internal EmojiInlineResolvingEventArgs(string emojiString)
        {
            EmojiString = emojiString;
        }

        /// <summary>
        /// Gets the emoji string in the markdown document.
        /// </summary>
        public string EmojiString { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this event was handled successfully.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Gets or sets the emoji inline to display in the <see cref="MarkdownTextBlock"/>.
        /// </summary>
        public Inline EmojiInline { get; set; }
    }
}
