// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Markup;

namespace ModernWpf.Toolkit.UI.Extensions
{
    /// <summary>
    /// A markup extension that returns a collection of values of a specific <see langword="enum"/>
    /// </summary>
    [MarkupExtensionReturnType(typeof(Array))]
    public sealed class EnumValuesExtension : MarkupExtension
    {
        /// <summary>
        /// Gets or sets the <see cref="System.Type"/> of the target <see langword="enum"/>
        /// </summary>
        public Type Type { get; set; }

        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider) => Enum.GetValues(Type);
    }
}