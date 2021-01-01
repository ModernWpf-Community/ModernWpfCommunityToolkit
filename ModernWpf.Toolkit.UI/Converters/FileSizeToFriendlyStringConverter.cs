// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace ModernWpf.Toolkit.UI.Converters
{
    /// <summary>
    /// Converts a file size in bytes to a more human-readable friendly format using <see cref="Toolkit.Converters.ToFileSizeString(long)"/>
    /// </summary>
    public class FileSizeToFriendlyStringConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long size)
            {
                return Toolkit.Converters.ToFileSizeString(size);
            }

            return string.Empty;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
