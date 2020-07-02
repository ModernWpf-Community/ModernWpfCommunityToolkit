// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ModernWpf.Controls;
using System;
using System.Windows.Markup;

namespace ModernWpf.Toolkit.UI.Extensions
{
    /// <summary>
    /// Custom <see cref="MarkupExtension"/> which can provide symbol-baased <see cref="FontIcon"/> values.
    /// </summary>
    [MarkupExtensionReturnType(typeof(SymbolIcon))]
    public class SymbolIconExtension : TextIconExtension
    {
        /// <summary>
        /// Gets or sets the <see cref="Symbol"/> value representing the icon to display.
        /// </summary>
        public Symbol Symbol { get; set; }

        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var symbolIcon = new SymbolIcon(Symbol);

            if (Foreground != null)
            {
                symbolIcon.Foreground = Foreground;
            }

            return symbolIcon;
        }
    }
}
