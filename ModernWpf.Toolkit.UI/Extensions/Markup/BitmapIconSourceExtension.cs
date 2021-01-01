// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ModernWpf.Controls;
using System;
using System.Windows.Markup;

namespace ModernWpf.Toolkit.UI.Extensions
{
    /// <summary>
    /// Custom <see cref="MarkupExtension"/> which can provide <see cref="BitmapIconSource"/> values.
    /// </summary>
    [MarkupExtensionReturnType(typeof(BitmapIconSource))]
    public sealed class BitmapIconSourceExtension : MarkupExtension
    {
        /// <summary>
        /// Gets or sets the <see cref="Uri"/> representing the image to display.
        /// </summary>
        public Uri Source { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display the icon as monochrome.
        /// </summary>
        public bool ShowAsMonochrome { get; set; }

        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new BitmapIconSource
            {
                ShowAsMonochrome = ShowAsMonochrome,
                UriSource = Source
            };
        }
    }
}
