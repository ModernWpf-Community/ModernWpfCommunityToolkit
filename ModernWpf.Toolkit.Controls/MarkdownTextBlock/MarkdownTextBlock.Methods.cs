// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ModernWpf.Toolkit.Controls.Markdown.Render;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Toolkit.Parsers.Markdown;
using ColorCode;
using ModernWpf.Controls;
using ModernWpf.Toolkit.Controls.Markdown.Inlines;

namespace ModernWpf.Toolkit.Controls
{
    /// <summary>
    /// An efficient and extensible control that can parse and render markdown.
    /// </summary>
    public partial class MarkdownTextBlock
    {
        /// <summary>
        /// Sets the Markdown Renderer for Rendering the UI.
        /// </summary>
        /// <typeparam name="T">The Inherited Markdown Render</typeparam>
        public void SetRenderer<T>()
            where T : MarkdownRenderer
        {
            renderertype = typeof(T);
        }

        /// <summary>
        /// Called to preform a render of the current Markdown.
        /// </summary>
        private void RenderMarkdown()
        {
            if (_rootElement == null)
            {
                return;
            }

            if (_flowDocumentScrollViewer == null)
            {
                _flowDocumentScrollViewer = new FlowDocumentScrollViewer()
                {
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto
                };
                DataObject.AddCopyingHandler(_flowDocumentScrollViewer, OnCopy);
                _rootElement.Child = _flowDocumentScrollViewer;
            }

            UnhookListeners();
            _listeningHyperlinks.Clear();

            var markdownRenderedArgs = new MarkdownRenderedEventArgs(null);

            if (string.IsNullOrWhiteSpace(Text))
            {
                _flowDocumentScrollViewer.Document = null;
            }
            else
            {
                try
                {
                    MarkdownDocument markdown = new MarkdownDocument();
                    foreach (string str in SchemeList.Split(',').ToList())
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            MarkdownDocument.KnownSchemes.Add(str);
                        }
                    }

                    markdown.Parse(Text);

                    if (!(Activator.CreateInstance(renderertype, markdown, this, this, this, this) is MarkdownRenderer renderer))
                    {
                        throw new Exception("Markdown Renderer was not of the correct type.");
                    }

                    renderer.FontFamily = FontFamily;
                    renderer.FontSize = FontSize;
                    renderer.FontStretch = FontStretch;
                    renderer.FontStyle = FontStyle;
                    renderer.FontWeight = FontWeight;
                    renderer.Foreground = Foreground;
                    renderer.Padding = Padding;
                    renderer.CodeBackground = CodeBackground;
                    renderer.CodeBorderBrush = CodeBorderBrush;
                    renderer.CodeBorderThickness = CodeBorderThickness;
                    renderer.InlineCodeBorderThickness = InlineCodeBorderThickness;
                    renderer.InlineCodeBackground = InlineCodeBackground;
                    renderer.InlineCodeBorderBrush = InlineCodeBorderBrush;
                    renderer.InlineCodePadding = InlineCodePadding;
                    renderer.InlineCodeFontFamily = InlineCodeFontFamily;
                    renderer.InlineCodeForeground = InlineCodeForeground;
                    renderer.CodeForeground = CodeForeground;
                    renderer.CodeFontFamily = CodeFontFamily;
                    renderer.CodePadding = CodePadding;
                    renderer.CodeMargin = CodeMargin;
                    renderer.EmojiFontFamily = EmojiFontFamily;
                    renderer.Header1FontSize = Header1FontSize;
                    renderer.Header1FontWeight = Header1FontWeight;
                    renderer.Header1Margin = Header1Margin;
                    renderer.Header1Foreground = Header1Foreground;
                    renderer.Header2FontSize = Header2FontSize;
                    renderer.Header2FontWeight = Header2FontWeight;
                    renderer.Header2Margin = Header2Margin;
                    renderer.Header2Foreground = Header2Foreground;
                    renderer.Header3FontSize = Header3FontSize;
                    renderer.Header3FontWeight = Header3FontWeight;
                    renderer.Header3Margin = Header3Margin;
                    renderer.Header3Foreground = Header3Foreground;
                    renderer.Header4FontSize = Header4FontSize;
                    renderer.Header4FontWeight = Header4FontWeight;
                    renderer.Header4Margin = Header4Margin;
                    renderer.Header4Foreground = Header4Foreground;
                    renderer.Header5FontSize = Header5FontSize;
                    renderer.Header5FontWeight = Header5FontWeight;
                    renderer.Header5Margin = Header5Margin;
                    renderer.Header5Foreground = Header5Foreground;
                    renderer.Header6FontSize = Header6FontSize;
                    renderer.Header6FontWeight = Header6FontWeight;
                    renderer.Header6Margin = Header6Margin;
                    renderer.Header6Foreground = Header6Foreground;
                    renderer.HorizontalRuleBrush = HorizontalRuleBrush;
                    renderer.HorizontalRuleMargin = HorizontalRuleMargin;
                    renderer.HorizontalRuleThickness = HorizontalRuleThickness;
                    renderer.ListMargin = ListMargin;
                    renderer.ListBulletSpacing = ListBulletSpacing;
                    renderer.ParagraphMargin = ParagraphMargin;
                    renderer.ParagraphLineHeight = ParagraphLineHeight;
                    renderer.QuoteBackground = QuoteBackground;
                    renderer.QuoteBorderBrush = QuoteBorderBrush;
                    renderer.QuoteBorderThickness = QuoteBorderThickness;
                    renderer.QuoteForeground = QuoteForeground;
                    renderer.QuoteMargin = QuoteMargin;
                    renderer.QuotePadding = QuotePadding;
                    renderer.TableBorderBrush = TableBorderBrush;
                    renderer.TableBorderThickness = TableBorderThickness;
                    renderer.YamlBorderBrush = YamlBorderBrush;
                    renderer.YamlBorderThickness = YamlBorderThickness;
                    renderer.TableCellPadding = TableCellPadding;
                    renderer.TableMargin = TableMargin;
                    renderer.LinkForeground = LinkForeground;
                    renderer.ImageStretch = ImageStretch;
                    renderer.ImageMaxHeight = ImageMaxHeight;
                    renderer.ImageMaxWidth = ImageMaxWidth;
                    renderer.FlowDirection = FlowDirection;

                    _flowDocumentScrollViewer.IsSelectionEnabled = IsTextSelectionEnabled;
                    _flowDocumentScrollViewer.Document = renderer.Render();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error while parsing and rendering: " + ex.Message);
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }

                    markdownRenderedArgs = new MarkdownRenderedEventArgs(ex);
                }
            }

            MarkdownRendered?.Invoke(this, markdownRenderedArgs);
        }

        private void HookListeners()
        {
            foreach (object link in _listeningHyperlinks)
            {
                if (link is Hyperlink hyperlink)
                {
                    hyperlink.Click -= Hyperlink_Click;
                    hyperlink.Click += Hyperlink_Click;
                }
                else if (link is HyperlinkButton imagelink)
                {
                    imagelink.Click -= NewImagelink_Click;
                    imagelink.Click += NewImagelink_Click;
                }
            }
        }

        private void UnhookListeners()
        {
            foreach (object link in _listeningHyperlinks)
            {
                if (link is Hyperlink hyperlink)
                {
                    hyperlink.Click -= Hyperlink_Click;
                }
                else if (link is HyperlinkButton imagelink)
                {
                    imagelink.Click -= NewImagelink_Click;
                }
            }
        }

        /// <summary>
        /// Called when the render has a link we need to listen to.
        /// </summary>
        public void RegisterNewHyperLink(Hyperlink newHyperlink, string linkUrl)
        {
            newHyperlink.Click += Hyperlink_Click;

            newHyperlink.SetValue(HyperlinkUrlProperty, linkUrl);

            _listeningHyperlinks.Add(newHyperlink);
        }

        /// <summary>
        /// Called when the render has a link we need to listen to.
        /// </summary>
        public void RegisterNewHyperLink(HyperlinkButton newImagelink, string linkUrl, bool isHyperLink)
        {
            newImagelink.Click += NewImagelink_Click;

            newImagelink.SetValue(HyperlinkUrlProperty, linkUrl);

            newImagelink.SetValue(IsHyperlinkProperty, isHyperLink);

            _listeningHyperlinks.Add(newImagelink);
        }

        /// <summary>
        /// Called when the renderer needs to display a image.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        async Task<ImageSource> IImageResolver.ResolveImageAsync(string url, string tooltip)
        {

            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                if (!string.IsNullOrEmpty(UriPrefix))
                {
                    url = string.Format("{0}{1}", UriPrefix, url);
                }
            }

            var eventArgs = new ImageResolvingEventArgs(url, tooltip);
            ImageResolving?.Invoke(this, eventArgs);

            await eventArgs.WaitForDeferrals();

            try
            {
                return eventArgs.Handled
                    ? eventArgs.Image
                    : GetImageSource(new Uri(url, UriKind.RelativeOrAbsolute));
            }
            catch (Exception)
            {
                return null;
            }

            static ImageSource GetImageSource(Uri imageUrl)
            {
                return new BitmapImage(imageUrl);
            }
        }

        /// <summary>
        /// Called when the renderer needs to display a emoji inline.
        /// </summary>
        /// <returns>A <see cref="Inline"/> with the rendered emoji.</returns>
        Inline IEmojiInlineResolver.ResolveEmojiInline(string emojiString)
        {
            var eventArgs = new EmojiInlineResolvingEventArgs(emojiString);
            EmojiInlineResolving?.Invoke(this, eventArgs);

            return eventArgs.Handled ? eventArgs.EmojiInline : null;
        }

        /// <summary>
        /// Called when a Code Block is being rendered.
        /// </summary>
        /// <returns>Parsing was handled Successfully</returns>
        bool ICodeBlockResolver.ParseSyntax(InlineCollection inlineCollection, string text, string codeLanguage)
        {
            var eventArgs = new CodeBlockResolvingEventArgs(inlineCollection, text, codeLanguage);
            CodeBlockResolving?.Invoke(this, eventArgs);

            try
            {
                var result = eventArgs.Handled;
                if (UseSyntaxHighlighting && !result && codeLanguage != null)
                {
                    var language = Languages.FindById(codeLanguage);
                    if (language != null)
                    {
                        CodeBlockFormatter formatter;
                        if (CodeStyling != null)
                        {
                            formatter = new CodeBlockFormatter(CodeStyling);
                        }
                        else
                        {
                            var theme = ThemeManager.GetActualTheme(this);

                            formatter = new CodeBlockFormatter(theme);
                        }

                        formatter.FormatInlines(text, language, inlineCollection);
                        return true;
                    }
                }

                return result;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Called when a link needs to be handled
        /// </summary>
        internal async void LinkHandled(string url, bool isHyperlink)
        {
            // Links that are nested within superscript elements cause the Click event to fire multiple times.
            // e.g. this markdown "[^bot](http://www.reddit.com/r/youtubefactsbot/wiki/index)"
            // Therefore we detect and ignore multiple clicks.
            if (multiClickDetectionTriggered)
            {
                return;
            }

            multiClickDetectionTriggered = true;
            await Dispatcher.InvokeAsync(() => multiClickDetectionTriggered = false);

            if (url == null)
            {
                return;
            }

            var eventArgs = new LinkClickedEventArgs(url);
            if (isHyperlink)
            {
                LinkClicked?.Invoke(this, eventArgs);
            }
            else
            {
                ImageClicked?.Invoke(this, eventArgs);
            }
        }

        /// <summary>
        /// Called when the document needs to be copied
        /// </summary>
        private void OnCopy(object sender, DataObjectCopyingEventArgs e)
        {
            string clipboard = "";
            TextPointer next;
            for (TextPointer p = _flowDocumentScrollViewer.Selection.Start;
                 p != null && p.CompareTo(_flowDocumentScrollViewer.Selection.End) < 0;
                 p = next)
            {
                next = p.GetNextInsertionPosition(LogicalDirection.Forward);
                if (next == null)
                    break;

                var uIElementInline = (next.Parent as Inline)?.PreviousInline as IUIElementInline;
                if (uIElementInline == null && next.Parent != p.Parent)
                    uIElementInline = (p.Parent as Inline)?.NextInline as IUIElementInline;
                if (uIElementInline != null && (p.Parent as Inline)?.PreviousInline != uIElementInline)
                    clipboard += uIElementInline?.GetUIContentString();
                else
                    clipboard += new TextRange(p, next).Text;
            }

            Clipboard.SetText(clipboard);
            e.Handled = true;
            e.CancelCommand();
        }
    }
}
