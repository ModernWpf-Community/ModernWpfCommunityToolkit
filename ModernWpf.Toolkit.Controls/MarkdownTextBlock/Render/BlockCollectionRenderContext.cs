// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Parsers.Markdown.Render;
using System.Windows.Documents;

namespace ModernWpf.Toolkit.Controls.Markdown.Render
{
    /// <summary>
    /// The Context of the Current Document Rendering.
    /// </summary>
    public class BlockCollectionRenderContext : RenderContext
    {
        internal BlockCollectionRenderContext(BlockCollection blockCollection)
        {
            BlockCollection = blockCollection;
        }

        internal BlockCollectionRenderContext(BlockCollection blockCollection, IRenderContext context)
            : this(blockCollection)
        {
            TrimLeadingWhitespace = context.TrimLeadingWhitespace;
            Parent = context.Parent;

            if (context is RenderContext localcontext)
            {
                Foreground = localcontext.Foreground;
                OverrideForeground = localcontext.OverrideForeground;
            }
        }

        /// <summary>
        /// Gets or sets the list to add to.
        /// </summary>
        public BlockCollection BlockCollection { get; set; }
    }
}
