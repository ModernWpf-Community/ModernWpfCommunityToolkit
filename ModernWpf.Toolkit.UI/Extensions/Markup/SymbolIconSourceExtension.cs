﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ModernWpf.Controls;
using System;
using System.Windows.Markup;

namespace ModernWpf.Toolkit.UI.Extensions
{
    /// <summary>
    /// Custom <see cref="MarkupExtension"/> which can provide symbol-based <see cref="FontIconSource"/> values.
    /// </summary>
    [MarkupExtensionReturnType(typeof(FontIconSource))]
    public class SymbolIconSourceExtension : TextIconExtension
    {
        /// <summary>
        /// Gets or sets the <see cref="Windows.UI.Xaml.Controls.Symbol"/> value representing the icon to display.
        /// </summary>
        public Symbol Symbol { get; set; }

        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var fontIcon = new FontIconSource
            {
                Glyph = unchecked((char)Symbol).ToString(),
                FontFamily = SegoeMDL2AssetsFontFamily,
                FontWeight = FontWeight,
                FontStyle = FontStyle
            };

            if (FontSize > 0)
            {
                fontIcon.FontSize = FontSize;
            }

            if (Foreground != null)
            {
                fontIcon.Foreground = Foreground;
            }

            return fontIcon;
        }
    }
}
