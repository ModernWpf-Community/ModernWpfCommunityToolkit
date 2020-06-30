// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Parsers.Markdown;
using Microsoft.Toolkit.Parsers.Markdown.Inlines;
using Microsoft.Toolkit.Parsers.Markdown.Render;
using ModernWpf.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ModernWpf.Toolkit.Controls.Markdown.Render
{
    /// <summary>
    /// Inline UI Methods for WPF UI Creation.
    /// </summary>
    public partial class MarkdownRenderer
    {
        /// <summary>
        /// Renders emoji element.
        /// </summary>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="context"> Persistent state. </param>
        protected override void RenderEmoji(EmojiInline element, IRenderContext context)
        {
            if (!(context is InlineRenderContext localContext))
            {
                throw new RenderContextIncorrectException();
            }

            var inlineCollection = localContext.InlineCollection;

            Inline emoji = new Run
            {
                FontFamily = EmojiFontFamily ?? DefaultEmojiFont,
                Text = element.Text
            };

            inlineCollection.Add(emoji);

            var resolvedInline = EmojiInlineResolver.ResolveEmojiInline(element.Text);

            if (resolvedInline == null)
            {
                return;
            }

            inlineCollection.InsertAfter(emoji, resolvedInline);
            inlineCollection.Remove(emoji);

        }

        /// <summary>
        /// Renders a text run element.
        /// </summary>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="context"> Persistent state. </param>
        protected override void RenderTextRun(TextRunInline element, IRenderContext context)
        {
            InternalRenderTextRun(element, context);
        }

        private Run InternalRenderTextRun(TextRunInline element, IRenderContext context)
        {
            if (!(context is InlineRenderContext localContext))
            {
                throw new RenderContextIncorrectException();
            }

            var inlineCollection = localContext.InlineCollection;

            Run textRun = new Run
            {
                Text = CollapseWhitespace(context, element.Text)
            };

            inlineCollection.Add(textRun);
            return textRun;
        }

        /// <summary>
        /// Renders a bold run element.
        /// </summary>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="context"> Persistent state. </param>
        protected override void RenderBoldRun(BoldTextInline element, IRenderContext context)
        {
            if (!(context is InlineRenderContext localContext))
            {
                throw new RenderContextIncorrectException();
            }

            Span boldSpan = new Span
            {
                FontWeight = FontWeights.Bold
            };

            var childContext = new InlineRenderContext(boldSpan.Inlines, context)
            {
                Parent = boldSpan,
                WithinBold = true
            };

            RenderInlineChildren(element.Inlines, childContext);

            localContext.InlineCollection.Add(boldSpan);
        }

        /// <summary>
        /// Renders a link element
        /// </summary>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="context"> Persistent state. </param>
        protected override void RenderMarkdownLink(MarkdownLinkInline element, IRenderContext context)
        {
            if (!(context is InlineRenderContext localContext))
            {
                throw new RenderContextIncorrectException();
            }

            // HACK: Superscript is not allowed within a hyperlink.  But if we switch it around, so
            // that the superscript is outside the hyperlink, then it will render correctly.
            // This assumes that the entire hyperlink is to be rendered as superscript.
            if (AllTextIsSuperscript(element) == false)
            {
                var link = new Hyperlink();

                LinkRegister.RegisterNewHyperLink(link, element.Url);

                RemoveSuperscriptRuns(element, insertCaret: true);

                var childContext = new InlineRenderContext(link.Inlines, context)
                {
                    Parent = link,
                    WithinHyperlink = true
                };

                if (localContext.OverrideForeground)
                {
                    link.Foreground = localContext.Foreground;
                }
                else if (LinkForeground != null)
                {
                    link.Foreground = LinkForeground;
                }

                RenderInlineChildren(element.Inlines, childContext);
                context.TrimLeadingWhitespace = childContext.TrimLeadingWhitespace;

                ToolTipService.SetToolTip(link, element.Tooltip ?? element.Url);

                localContext.InlineCollection.Add(link);
            }
            else
            {
                // THE HACK IS ON!

                // Create a fake superscript element.
                var fakeSuperscript = new SuperscriptTextInline
                {
                    Inlines = new List<MarkdownInline>
                    {
                        element
                    }
                };

                RemoveSuperscriptRuns(element, insertCaret: false);

                RenderSuperscriptRun(fakeSuperscript, context);
            }
        }

        /// <summary>
        /// Renders a raw link element.
        /// </summary>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="context"> Persistent state. </param>
        protected override void RenderHyperlink(HyperlinkInline element, IRenderContext context)
        {
            if (!(context is InlineRenderContext localContext))
            {
                throw new RenderContextIncorrectException();
            }

            var link = new Hyperlink();

            LinkRegister.RegisterNewHyperLink(link, element.Url);

            var brush = localContext.Foreground;
            if (LinkForeground != null && !localContext.OverrideForeground)
            {
                brush = LinkForeground;
            }

            Run linkText = new Run
            {
                Text = CollapseWhitespace(context, element.Text),
                Foreground = brush
            };

            link.Inlines.Add(linkText);

            localContext.InlineCollection.Add(link);
        }

        /// <summary>
        /// Renders an image element.
        /// </summary>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="context"> Persistent state. </param>
        protected override async void RenderImage(ImageInline element, IRenderContext context)
        {
            if (!(context is InlineRenderContext localContext))
            {
                throw new RenderContextIncorrectException();
            }

            var inlineCollection = localContext.InlineCollection;

            var placeholder = InternalRenderTextRun(new TextRunInline { Text = element.Text, Type = MarkdownInlineType.TextRun }, context);
            var resolvedImage = await ImageResolver.ResolveImageAsync(element.RenderUrl, element.Tooltip);

            if (resolvedImage == null)
            {
                return;
            }

            var image = new Image
            {
                Source = resolvedImage,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Stretch = ImageStretch
            };

            HyperlinkButton hyperlinkButton = new HyperlinkButton()
            {
                Content = image
            };

            var viewbox = new Viewbox
            {
                Child = hyperlinkButton,
                StretchDirection = StretchDirection.DownOnly
            };

            var imageContainer = new InlineUIContainer() { Child = viewbox };

            bool ishyperlink = false;
            if (element.RenderUrl != element.Url)
            {
                ishyperlink = true;
            }

            LinkRegister.RegisterNewHyperLink(hyperlinkButton, element.Url, ishyperlink);

            if (ImageMaxHeight > 0)
            {
                viewbox.MaxHeight = ImageMaxHeight;
            }

            if (ImageMaxWidth > 0)
            {
                viewbox.MaxWidth = ImageMaxWidth;
            }

            if (element.ImageWidth > 0)
            {
                image.Width = element.ImageWidth;
                image.Stretch = Stretch.UniformToFill;
            }

            if (element.ImageHeight > 0)
            {
                if (element.ImageWidth == 0)
                {
                    image.Width = element.ImageHeight;
                }

                image.Height = element.ImageHeight;
                image.Stretch = Stretch.UniformToFill;
            }

            if (element.ImageHeight > 0 && element.ImageWidth > 0)
            {
                image.Stretch = Stretch.Fill;
            }

            ToolTipService.SetToolTip(image, element.Tooltip);

            // Try to add it to the current inlines
            // Could fail because some containers like Hyperlink cannot have inlined images
            try
            {
                inlineCollection.InsertAfter(placeholder, imageContainer);
                inlineCollection.Remove(placeholder);
            }
            catch { }
        }

        /// <summary>
        /// Renders a text run element.
        /// </summary>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="context"> Persistent state. </param>
        protected override void RenderItalicRun(ItalicTextInline element, IRenderContext context)
        {
            if (!(context is InlineRenderContext localContext))
            {
                throw new RenderContextIncorrectException();
            }

            Span italicSpan = new Span
            {
                FontStyle = FontStyles.Italic
            };

            var childContext = new InlineRenderContext(italicSpan.Inlines, context)
            {
                Parent = italicSpan,
                WithinItalics = true
            };

            RenderInlineChildren(element.Inlines, childContext);

            localContext.InlineCollection.Add(italicSpan);
        }

        /// <summary>
        /// Renders a strikethrough element.
        /// </summary>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="context"> Persistent state. </param>
        protected override void RenderStrikethroughRun(StrikethroughTextInline element, IRenderContext context)
        {
            if (!(context is InlineRenderContext localContext))
            {
                throw new RenderContextIncorrectException();
            }

            Span span = new Span
            {
                TextDecorations = TextDecorations.Strikethrough
            };

            var childContext = new InlineRenderContext(span.Inlines, context)
            {
                Parent = span
            };

            RenderInlineChildren(element.Inlines, childContext);

            localContext.InlineCollection.Add(span);
        }

        /// <summary>
        /// Renders a superscript element.
        /// </summary>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="context"> Persistent state. </param>
        protected override void RenderSuperscriptRun(SuperscriptTextInline element, IRenderContext context)
        {
            var localContext = context as InlineRenderContext;
            var parent = localContext?.Parent as TextElement;
            if (localContext == null && parent == null)
            {
                throw new RenderContextIncorrectException();
            }

            // Le <sigh>, InlineUIContainers are not allowed within hyperlinks.
            if (localContext.WithinHyperlink)
            {
                RenderInlineChildren(element.Inlines, context);
                return;
            }
            var span = new Span()
            {
                FontSize = parent.FontSize * 0.8,
                FontFamily = parent.FontFamily,
                FontStyle = parent.FontStyle,
                FontWeight = parent.FontWeight,
                BaselineAlignment = BaselineAlignment.TextTop
            };

            if (parent is Span parentspan)
            {
                span.BaselineAlignment = parentspan.BaselineAlignment switch
                {
                    BaselineAlignment.Bottom => BaselineAlignment.Subscript,
                    BaselineAlignment.Baseline => BaselineAlignment.Center,
                    BaselineAlignment.Center => BaselineAlignment.TextTop,
                    BaselineAlignment.TextTop => BaselineAlignment.Top,
                    BaselineAlignment.Subscript => BaselineAlignment.Baseline,
                    _ => BaselineAlignment.TextTop
                };
            }

            var childContext = new InlineRenderContext(span.Inlines, context)
            {
                Parent = span
            };

            RenderInlineChildren(element.Inlines, childContext);

            localContext.InlineCollection.Add(span);
        }

        /// <summary>
        /// Renders a subscript element.
        /// </summary>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="context"> Persistent state. </param>
        protected override void RenderSubscriptRun(SubscriptTextInline element, IRenderContext context)
        {
            var localContext = context as InlineRenderContext;
            var parent = localContext?.Parent as TextElement;
            if (localContext == null && parent == null)
            {
                throw new RenderContextIncorrectException();
            }

            var span = new Span
            {
                FontSize = parent.FontSize * 0.7,
                FontFamily = parent.FontFamily,
                FontStyle = parent.FontStyle,
                FontWeight = parent.FontWeight
            };

            span.BaselineAlignment = BaselineAlignment.Subscript;

            var childContext = new InlineRenderContext(span.Inlines, context)
            {
                Parent = span
            };

            RenderInlineChildren(element.Inlines, childContext);

            localContext.InlineCollection.Add(span);
        }

        /// <summary>
        /// Renders a code element
        /// </summary>
        /// <param name="element"> The parsed inline element to render. </param>
        /// <param name="context"> Persistent state. </param>
        protected override void RenderCodeRun(CodeInline element, IRenderContext context)
        {
            if (!(context is InlineRenderContext localContext))
            {
                throw new RenderContextIncorrectException();
            }

            var text = CreateTextBlock(localContext);
            text.Text = CollapseWhitespace(context, element.Text);
            text.FontFamily = InlineCodeFontFamily ?? FontFamily;
            text.Foreground = InlineCodeForeground ?? Foreground;

            if (localContext.WithinItalics)
            {
                text.FontStyle = FontStyles.Italic;
            }

            if (localContext.WithinBold)
            {
                text.FontWeight = FontWeights.Bold;
            }

            var border = new Border
            {
                BorderThickness = InlineCodeBorderThickness,
                BorderBrush = InlineCodeBorderBrush,
                Background = InlineCodeBackground,
                Child = text,
                Padding = InlineCodePadding,
                Margin = InlineCodeMargin
            };

            // Aligns content in InlineUI, see https://social.msdn.microsoft.com/Forums/silverlight/en-US/48b5e91e-efc5-4768-8eaf-f897849fcf0b/richtextbox-inlineuicontainer-vertical-alignment-issue?forum=silverlightarchieve
            border.RenderTransform = new TranslateTransform
            {
                Y = 4
            };

            var codeInline = new Inlines.CodeUIElementInline
            {
                Child = border,
                Text = text.Text
            };

            localContext.InlineCollection.Add(codeInline);
        }
    }
}
