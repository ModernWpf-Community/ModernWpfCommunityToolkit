// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ModernWpf.Toolkit.Controls.Markdown.Render;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Toolkit.Controls
{
    /// <summary>
    /// An efficient and extensible control that can parse and render markdown.
    /// </summary>
    public partial class MarkdownTextBlock : Control, ILinkRegister, IImageResolver, ICodeBlockResolver, IEmojiInlineResolver
    {
        static MarkdownTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MarkdownTextBlock), new FrameworkPropertyMetadata(typeof(MarkdownTextBlock)));
            FontSizeProperty.OverrideMetadata(typeof(MarkdownTextBlock), new FrameworkPropertyMetadata(OnPropertyChangedStatic));
            FlowDirectionProperty.OverrideMetadata(typeof(MarkdownTextBlock), new FrameworkPropertyMetadata(OnPropertyChangedStatic));
            BackgroundProperty.OverrideMetadata(typeof(MarkdownTextBlock), new FrameworkPropertyMetadata(OnPropertyChangedStatic));
            BorderBrushProperty.OverrideMetadata(typeof(MarkdownTextBlock), new FrameworkPropertyMetadata(OnPropertyChangedStatic));
            BorderThicknessProperty.OverrideMetadata(typeof(MarkdownTextBlock), new FrameworkPropertyMetadata(OnPropertyChangedStatic));
            FontFamilyProperty.OverrideMetadata(typeof(MarkdownTextBlock), new FrameworkPropertyMetadata(OnPropertyChangedStatic));
            FontStretchProperty.OverrideMetadata(typeof(MarkdownTextBlock), new FrameworkPropertyMetadata(OnPropertyChangedStatic));
            FontStyleProperty.OverrideMetadata(typeof(MarkdownTextBlock), new FrameworkPropertyMetadata(OnPropertyChangedStatic));
            FontWeightProperty.OverrideMetadata(typeof(MarkdownTextBlock), new FrameworkPropertyMetadata(OnPropertyChangedStatic));
            ForegroundProperty.OverrideMetadata(typeof(MarkdownTextBlock), new FrameworkPropertyMetadata(OnPropertyChangedStatic));
            PaddingProperty.OverrideMetadata(typeof(MarkdownTextBlock), new FrameworkPropertyMetadata(OnPropertyChangedStatic));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownTextBlock"/> class.
        /// </summary>
        public MarkdownTextBlock()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            HookListeners();
            ThemeManager.AddActualThemeChangedHandler(this, OnActualThemeChanged);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ThemeManager.RemoveActualThemeChangedHandler(this, OnActualThemeChanged);
        }

        private void OnActualThemeChanged(object sender, RoutedEventArgs e)
        {
            RenderMarkdown();
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            _rootElement = GetTemplateChild("RootElement") as Border;

            RenderMarkdown();
        }
    }
}