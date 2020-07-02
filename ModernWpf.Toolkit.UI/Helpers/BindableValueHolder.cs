// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Markup;

namespace ModernWpf.Toolkit.UI.Helpers
{
    /// <summary>
    /// Holds the value.
    /// Can be used to change several objects' properties at a time.
    /// </summary>
    [ContentProperty(nameof(Value))]
    public class BindableValueHolder : DependencyObject
    {
        /// <summary>
        /// Identifies the <see cref="Value"/> property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(BindableValueHolder), null);

        /// <summary>
        /// Gets or sets the held value.
        /// </summary>
        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
    }
}
