// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Media;

namespace ModernWpf.Toolkit.UI.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="ScaleTransform"/>.
    /// </summary>
    public static class ScaleTransformExtensions
    {
        /// <summary>
        /// Gets the matrix that represents this transform.
        /// Implements WPF's SkewTransform.Value.
        /// </summary>
        /// <param name="transform">Extended SkewTransform.</param>
        /// <returns>Matrix representing transform.</returns>
        public static Matrix GetMatrix(this ScaleTransform transform)
        {
            transform.
            var I = Matrix.Identity;
            I.ScaleAt(transform.ScaleX, transform.ScaleY, transform.CenterX, transform.CenterY);
            return I;
        }
    }
}
