// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ModernWpf.Controls;
using System.Windows.Documents;

namespace ModernWpf.Toolkit.Controls.Markdown.Render
{
    /// <summary>
    /// An interface used to handle links in the markdown.
    /// </summary>
    public interface ILinkRegister
    {
        /// <summary>
        /// Registers a Hyperlink with a LinkUrl.
        /// </summary>
        /// <param name="newHyperlink">Hyperlink to Register.</param>
        /// <param name="linkUrl">Url to Register.</param>
        void RegisterNewHyperLink(Hyperlink newHyperlink, string linkUrl);

        /// <summary>
        /// Registers a Hyperlink with a LinkUrl.
        /// </summary>
        /// <param name="newImagelink">ImageLink to Register.</param>
        /// <param name="linkUrl">Url to Register.</param>
        /// <param name="isHyperLink">Is Image an IsHyperlink.</param>
        void RegisterNewHyperLink(HyperlinkButton newImagelink, string linkUrl, bool isHyperLink);
    }
}
