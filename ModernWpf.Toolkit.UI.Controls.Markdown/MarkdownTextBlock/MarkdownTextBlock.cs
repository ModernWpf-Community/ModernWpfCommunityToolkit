// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ModernWpf.Toolkit.UI.Controls.Markdown.Render;
using ModernWpf.Toolkit.UI.Extensions;
using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Toolkit.UI.Controls
{
    /// <summary>
    /// An efficient and extensible control that can parse and render markdown.
    /// </summary>
    public partial class MarkdownTextBlock : Control, ILinkRegister, IImageResolver, ICodeBlockResolver, IEmojiInlineResolver
    {
        private long _fontSizePropertyToken;
        private long _flowDirectionPropertyToken;
        private long _backgroundPropertyToken;
        private long _borderBrushPropertyToken;
        private long _borderThicknessPropertyToken;
        private long _fontFamilyPropertyToken;
        private long _fontStretchPropertyToken;
        private long _fontStylePropertyToken;
        private long _fontWeightPropertyToken;
        private long _foregroundPropertyToken;
        private long _paddingPropertyToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownTextBlock"/> class.
        /// </summary>
        public MarkdownTextBlock()
        {
            DefaultStyleKey = typeof(MarkdownTextBlock);
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            HookListeners();
            ThemeManager.AddActualThemeChangedHandler(this, OnActualThemeChanged);

            // Register for property callbacks that are owned by our parent class.
            _fontSizePropertyToken = this.RegisterPropertyChangedCallback(FontSizeProperty, OnPropertyChanged);
            _flowDirectionPropertyToken = this.RegisterPropertyChangedCallback(FlowDirectionProperty, OnPropertyChanged);
            _backgroundPropertyToken = this.RegisterPropertyChangedCallback(BackgroundProperty, OnPropertyChanged);
            _borderBrushPropertyToken = this.RegisterPropertyChangedCallback(BorderBrushProperty, OnPropertyChanged);
            _borderThicknessPropertyToken = this.RegisterPropertyChangedCallback(BorderThicknessProperty, OnPropertyChanged);
            _fontFamilyPropertyToken = this.RegisterPropertyChangedCallback(FontFamilyProperty, OnPropertyChanged);
            _fontStretchPropertyToken = this.RegisterPropertyChangedCallback(FontStretchProperty, OnPropertyChanged);
            _fontStylePropertyToken = this.RegisterPropertyChangedCallback(FontStyleProperty, OnPropertyChanged);
            _fontWeightPropertyToken = this.RegisterPropertyChangedCallback(FontWeightProperty, OnPropertyChanged);
            _foregroundPropertyToken = this.RegisterPropertyChangedCallback(ForegroundProperty, OnPropertyChanged);
            _paddingPropertyToken = this.RegisterPropertyChangedCallback(PaddingProperty, OnPropertyChanged);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ThemeManager.RemoveActualThemeChangedHandler(this, OnActualThemeChanged);

            // Unregister property callbacks
            this.UnregisterPropertyChangedCallback(FontSizeProperty, _fontSizePropertyToken);
            this.UnregisterPropertyChangedCallback(FlowDirectionProperty, _flowDirectionPropertyToken);
            this.UnregisterPropertyChangedCallback(BackgroundProperty, _backgroundPropertyToken);
            this.UnregisterPropertyChangedCallback(BorderBrushProperty, _borderBrushPropertyToken);
            this.UnregisterPropertyChangedCallback(BorderThicknessProperty, _borderThicknessPropertyToken);
            this.UnregisterPropertyChangedCallback(FontFamilyProperty, _fontFamilyPropertyToken);
            this.UnregisterPropertyChangedCallback(FontStretchProperty, _fontStretchPropertyToken);
            this.UnregisterPropertyChangedCallback(FontStyleProperty, _fontStylePropertyToken);
            this.UnregisterPropertyChangedCallback(FontWeightProperty, _fontWeightPropertyToken);
            this.UnregisterPropertyChangedCallback(ForegroundProperty, _foregroundPropertyToken);
            this.UnregisterPropertyChangedCallback(PaddingProperty, _paddingPropertyToken);
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
