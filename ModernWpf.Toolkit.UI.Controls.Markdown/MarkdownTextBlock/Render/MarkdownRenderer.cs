// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Parsers.Core;
using Microsoft.Toolkit.Parsers.Markdown;
using Microsoft.Toolkit.Parsers.Markdown.Inlines;
using Microsoft.Toolkit.Parsers.Markdown.Render;
using System;
using System.Windows;
using System.Windows.Documents;

namespace ModernWpf.Toolkit.UI.Controls.Markdown.Render
{
    /// <summary>
    /// Generates UI for the WPF Markdown Textblock.
    /// </summary>
    public partial class MarkdownRenderer : MarkdownRendererBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownRenderer"/> class.
        /// </summary>
        /// <param name="document">The Document to Render.</param>
        /// <param name="linkRegister">The LinkRegister, <see cref="MarkdownTextBlock"/> will use itself.</param>
        /// <param name="imageResolver">The Image Resolver, <see cref="MarkdownTextBlock"/> will use itself.</param>
        /// <param name="codeBlockResolver">The Code Block Resolver, <see cref="MarkdownTextBlock"/> will use itself.</param>
        /// <param name="emojiInlineResolver">The Emoji Inline Resolver, <see cref="MarkdownTextBlock"/> will use itself.</param>
        public MarkdownRenderer(MarkdownDocument document, ILinkRegister linkRegister, IImageResolver imageResolver, ICodeBlockResolver codeBlockResolver, IEmojiInlineResolver emojiInlineResolver)
            : base(document)
        {
            LinkRegister = linkRegister;
            ImageResolver = imageResolver;
            CodeBlockResolver = codeBlockResolver;
            EmojiInlineResolver = emojiInlineResolver;
            DefaultEmojiFont = SystemFonts.MessageFontFamily;
        }

        /// <summary>
        /// Called externally to render markdown to a text block.
        /// </summary>
        /// <returns> A FlowDocument. </returns>
        public FlowDocument Render()
        {
            var document = new FlowDocument()
            { 
                PagePadding = Padding,
                FontFamily = FontFamily,
                FontSize = FontSize,
                FontStretch = FontStretch,
                FontStyle = FontStyle,
                FontWeight = FontWeight,
                Foreground = Foreground,
                FlowDirection = FlowDirection,
                TextAlignment = TextAlignment.Left
            };

            Render(new BlockCollectionRenderContext(document.Blocks) { Foreground = Foreground });

            return document;
        }

        /// <summary>
        /// Creates a new TextBlock, with default settings.
        /// </summary>
        /// <returns>The created TextBlock</returns>
        internal SelectableTextBlock CreateTextBlock(RenderContext context)
        {
            var result = new SelectableTextBlock
            {
                FontFamily = FontFamily,
                FontSize = FontSize,
                FontStretch = FontStretch,
                FontStyle = FontStyle,
                FontWeight = FontWeight,
                Foreground = context.Foreground,
                FlowDirection = FlowDirection
            };
            return result;
        }

        /// <summary>
        /// Performs an action against any runs that occur within the given span.
        /// </summary>
        protected void AlterChildRuns(Span parentSpan, Action<Span, Run> action)
        {
            foreach (var inlineElement in parentSpan.Inlines)
            {
                if (inlineElement is Span span)
                {
                    AlterChildRuns(span, action);
                }
                else if (inlineElement is Run)
                {
                    action(parentSpan, (Run)inlineElement);
                }
            }
        }

        /// <summary>
        /// Checks if all text elements inside the given container are superscript.
        /// </summary>
        /// <returns> <c>true</c> if all text is superscript (level 1); <c>false</c> otherwise. </returns>
        private bool AllTextIsSuperscript(IInlineContainer container, int superscriptLevel = 0)
        {
            foreach (var inline in container.Inlines)
            {
                if (inline is SuperscriptTextInline textInline)
                {
                    if (AllTextIsSuperscript(textInline, superscriptLevel + 1) == false)
                    {
                        return false;
                    }
                }
                else if (inline is IInlineContainer)
                {
                    if (AllTextIsSuperscript((IInlineContainer)inline, superscriptLevel) == false)
                    {
                        return false;
                    }
                }
                else if (inline is IInlineLeaf && !ParseHelpers.IsMarkdownBlankOrWhiteSpace(((IInlineLeaf)inline).Text))
                {
                    if (superscriptLevel != 1)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Removes all superscript elements from the given container.
        /// </summary>
        private void RemoveSuperscriptRuns(IInlineContainer container, bool insertCaret)
        {
            for (int i = 0; i < container.Inlines.Count; i++)
            {
                var inline = container.Inlines[i];
                if (inline is SuperscriptTextInline textInline)
                {
                    RemoveSuperscriptRuns(textInline, insertCaret);

                    container.Inlines.RemoveAt(i);
                    if (insertCaret)
                    {
                        container.Inlines.Insert(i++, new TextRunInline { Text = "^" });
                    }

                    foreach (var superscriptInline in textInline.Inlines)
                    {
                        container.Inlines.Insert(i++, superscriptInline);
                    }

                    i--;
                }
                else if (inline is IInlineContainer)
                {
                    RemoveSuperscriptRuns((IInlineContainer)inline, insertCaret);
                }
            }
        }
    }
}
