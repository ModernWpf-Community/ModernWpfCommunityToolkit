// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Parsers.Markdown;
using Microsoft.Toolkit.Parsers.Markdown.Blocks;
using Microsoft.Toolkit.Parsers.Markdown.Render;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace ModernWpf.Toolkit.UI.Controls.Markdown.Render
{
    /// <summary>
    /// Block UI Methods for WPF UI Creation.
    /// </summary>
    public partial class MarkdownRenderer
    {
        /// <summary>
        /// Renders a list of block elements.
        /// </summary>
        protected override void RenderBlocks(IEnumerable<MarkdownBlock> blockElements, IRenderContext context)
        {
            if (!(context is BlockCollectionRenderContext localContext))
            {
                throw new RenderContextIncorrectException();
            }

            var blockCollection = localContext.BlockCollection;

            base.RenderBlocks(blockElements, context);

            Block previousBlock = null;
            foreach (Block block in blockCollection)
            {
                if (blockCollection.FirstBlock == block)
                {
                    block.Margin = new Thickness(
                        block.Margin.Left,
                        0,
                        block.Margin.Right,
                        block.Margin.Bottom);
                }
                else if (previousBlock != null)
                {
                    block.Margin = new Thickness(
                           block.Margin.Left,
                           Math.Max(block.Margin.Top, previousBlock.Margin.Bottom),
                           block.Margin.Right,
                           block.Margin.Bottom);
                    previousBlock.Margin = new Thickness(
                        previousBlock.Margin.Left,
                        previousBlock.Margin.Top,
                        previousBlock.Margin.Right,
                        0);
                }

                previousBlock = block;
            }
        }

        /// <summary>
        /// Renders a paragraph element.
        /// </summary>
        protected override void RenderParagraph(ParagraphBlock element, IRenderContext context)
        {
            if (!(context is BlockCollectionRenderContext localContext))
            {
                throw new RenderContextIncorrectException();
            }

            var blockCollection = localContext.BlockCollection;

            var paragraph = new Paragraph
            {
                Margin = ParagraphMargin,
                LineHeight = ParagraphLineHeight,
                Foreground = localContext.Foreground
            };

            var childContext = new InlineRenderContext(paragraph.Inlines, context)
            {
                Parent = paragraph
            };

            RenderInlineChildren(element.Inlines, childContext);

            blockCollection.Add(paragraph);
        }

        /// <summary>
        /// Renders a yaml header element.
        /// </summary>
        protected override void RenderYamlHeader(YamlHeaderBlock element, IRenderContext context)
        {
            if (!(context is BlockCollectionRenderContext localContext))
            {
                throw new RenderContextIncorrectException();
            }

            var blockCollection = localContext.BlockCollection;

            var table = new Table() { CellSpacing = 0.0 };

            string[] childrenKeys = new string[element.Children.Count];
            string[] childrenValues = new string[element.Children.Count];
            element.Children.Keys.CopyTo(childrenKeys, 0);
            element.Children.Values.CopyTo(childrenValues, 0);

            while (table.Columns.Count < element.Children.Count)
            {
                table.Columns.Add(new TableColumn());
            }

            var tableRG = new TableRowGroup();

            var keyrow = new TableRow();
            tableRG.Rows.Add(keyrow);

            var valuerow = new TableRow();
            tableRG.Rows.Add(valuerow);

            table.RowGroups.Add(tableRG);

            var borderthickness = new Thickness(YamlBorderThickness / 2);

            for (int i = 0; i < element.Children.Count; i++)
            {

                var keypara = new Paragraph(new Run(childrenKeys[i]))
                {
                    Foreground = Foreground,
                    TextAlignment = TextAlignment.Center,
                    FontWeight = FontWeights.Bold,
                    Margin = TableCellPadding
                };
                var keyCell = new TableCell(keypara)
                {
                    BorderThickness = borderthickness,
                    BorderBrush = YamlBorderBrush
                };

                var valuepara = new Paragraph(new Run(childrenValues[i]))
                {
                    Foreground = Foreground,
                    TextAlignment = TextAlignment.Left,
                    Margin = TableCellPadding
                };
                var valueCell = new TableCell(valuepara)
                {
                    BorderThickness = borderthickness,
                    BorderBrush = YamlBorderBrush
                };

                keyrow.Cells.Add(keyCell);
                valuerow.Cells.Add(valueCell);
            }

            table.Margin = TableMargin;
            table.BorderBrush = YamlBorderBrush;
            table.BorderThickness = borderthickness;

            blockCollection.Add(table);
        }

        /// <summary>
        /// Renders a header element.
        /// </summary>
        protected override void RenderHeader(HeaderBlock element, IRenderContext context)
        {
            if (!(context is BlockCollectionRenderContext localContext))
            {
                throw new RenderContextIncorrectException();
            }

            var blockCollection = localContext.BlockCollection;

            var paragraph = new Paragraph();

            var childInlines = paragraph.Inlines;
            switch (element.HeaderLevel)
            {
                case 1:
                    paragraph.Margin = Header1Margin;
                    paragraph.FontSize = Header1FontSize;
                    paragraph.FontWeight = Header1FontWeight;
                    paragraph.Foreground = Header1Foreground;
                    break;

                case 2:
                    paragraph.Margin = Header2Margin;
                    paragraph.FontSize = Header2FontSize;
                    paragraph.FontWeight = Header2FontWeight;
                    paragraph.Foreground = Header2Foreground;
                    break;

                case 3:
                    paragraph.Margin = Header3Margin;
                    paragraph.FontSize = Header3FontSize;
                    paragraph.FontWeight = Header3FontWeight;
                    paragraph.Foreground = Header3Foreground;
                    break;

                case 4:
                    paragraph.Margin = Header4Margin;
                    paragraph.FontSize = Header4FontSize;
                    paragraph.FontWeight = Header4FontWeight;
                    paragraph.Foreground = Header4Foreground;
                    break;

                case 5:
                    paragraph.Margin = Header5Margin;
                    paragraph.FontSize = Header5FontSize;
                    paragraph.FontWeight = Header5FontWeight;
                    paragraph.Foreground = Header5Foreground;
                    break;

                case 6:
                    paragraph.Margin = Header6Margin;
                    paragraph.FontSize = Header6FontSize;
                    paragraph.FontWeight = Header6FontWeight;
                    paragraph.Foreground = Header6Foreground;

                    var underline = new Underline();
                    childInlines = underline.Inlines;
                    paragraph.Inlines.Add(underline);
                    break;
            }

            var childContext = new InlineRenderContext(childInlines, context)
            {
                Parent = paragraph,
                TrimLeadingWhitespace = true
            };

            RenderInlineChildren(element.Inlines, childContext);

            blockCollection.Add(paragraph);
        }

        /// <summary>
        /// Renders a list element.
        /// </summary>
        protected override void RenderListElement(ListBlock element, IRenderContext context)
        {
            if (!(context is BlockCollectionRenderContext localContext))
            {
                throw new RenderContextIncorrectException();
            }

            var blockCollection = localContext.BlockCollection;

            var list = new List
            {
                MarkerOffset = ListBulletSpacing,
                Margin = ListMargin,
                Foreground = localContext.Foreground
            };


            list.MarkerStyle = element.Style switch
            {
                ListStyle.Bulleted => TextMarkerStyle.Disc,
                ListStyle.Numbered => TextMarkerStyle.Decimal,
                _ => TextMarkerStyle.Disc
            };

            for (int rowIndex = 0; rowIndex < element.Items.Count; rowIndex++)
            {
                var listItemblock = element.Items[rowIndex];

                var content = new ListItem();
                var childContext = new BlockCollectionRenderContext(content.Blocks, localContext);
                RenderBlocks(listItemblock.Blocks, childContext);

                list.ListItems.Add(content);
            }

            blockCollection.Add(list);
        }

        /// <summary>
        /// Renders a horizontal rule element.
        /// </summary>
        protected override void RenderHorizontalRule(IRenderContext context)
        {
            if (!(context is BlockCollectionRenderContext localContext))
            {
                throw new RenderContextIncorrectException();
            }

            var blockCollection = localContext.BlockCollection;

            var brush = localContext.Foreground;
            if (HorizontalRuleBrush != null && !localContext.OverrideForeground)
            {
                brush = HorizontalRuleBrush;
            }

            var rectangle = new Rectangle
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Height = HorizontalRuleThickness,
                Fill = brush,
                Margin = HorizontalRuleMargin
            };

            var container = new BlockUIContainer() { Child = rectangle };

            blockCollection.Add(container);
        }

        /// <summary>
        /// Renders a quote element.
        /// </summary>
        protected override void RenderQuote(QuoteBlock element, IRenderContext context)
        {
            if (!(context is BlockCollectionRenderContext localContext))
            {
                throw new RenderContextIncorrectException();
            }

            var blockCollection = localContext.BlockCollection;

            var section = new Section
            {
                Margin = QuoteMargin,
                Background = QuoteBackground,
                BorderThickness = QuoteBorderThickness,
                Padding = QuotePadding,
            };

            var childContext = new BlockCollectionRenderContext(section.Blocks, context)
            {
                Parent = section
            };

            if (QuoteForeground != null && !localContext.OverrideForeground)
            {
                childContext.Foreground = QuoteForeground;
            }

            RenderBlocks(element.Blocks, childContext);

            section.BorderBrush = childContext.OverrideForeground ? childContext.Foreground : QuoteBorderBrush ?? childContext.Foreground;

            blockCollection.Add(section);
        }

        /// <summary>
        /// Renders a code element.
        /// </summary>
        protected override void RenderCode(CodeBlock element, IRenderContext context)
        {
            if (!(context is BlockCollectionRenderContext localContext))
            {
                throw new RenderContextIncorrectException();
            }

            var blockCollection = localContext.BlockCollection;

            var brush = localContext.Foreground;
            if (CodeForeground != null && !localContext.OverrideForeground)
            {
                brush = CodeForeground;
            }

            var paragraph = new Paragraph
            {
                FontFamily = CodeFontFamily ?? FontFamily,
                Foreground = brush,
                LineHeight = FontSize * 1.4,
                Background = CodeBackground,
                BorderBrush = CodeBorderBrush,
                BorderThickness = CodeBorderThickness,
                Padding = CodePadding,
                Margin = CodeMargin
            };

            var hasCustomSyntax = CodeBlockResolver.ParseSyntax(paragraph.Inlines, element.Text, element.CodeLanguage);
            if (!hasCustomSyntax)
            {
                paragraph.Inlines.Add(new Run { Text = element.Text });
            }

            blockCollection.Add(paragraph);
        }

        /// <summary>
        /// Renders a table element.
        /// </summary>
        protected override void RenderTable(TableBlock element, IRenderContext context)
        {
            if (!(context is BlockCollectionRenderContext localContext))
            {
                throw new RenderContextIncorrectException();
            }

            var blockCollection = localContext.BlockCollection;

            var table = new Table() { CellSpacing = 0.0 };

            while (table.Columns.Count < element.ColumnDefinitions.Count)
            {
                table.Columns.Add(new TableColumn());
            }

            var tableRG = new TableRowGroup();

            while (tableRG.Rows.Count < element.Rows.Count)
            {
                tableRG.Rows.Add(new TableRow());
            }
            table.RowGroups.Add(tableRG);

            var borderthickness = new Thickness(TableBorderThickness / 2);

            for (int rowIndex = 0; rowIndex < element.Rows.Count; rowIndex++)
            {
                var row = element.Rows[rowIndex];

                for (int cellIndex = 0; cellIndex < Math.Min(element.ColumnDefinitions.Count, row.Cells.Count); cellIndex++)
                {
                    var cell = row.Cells[cellIndex];

                    var paragraph = new Paragraph
                    {
                        Foreground = localContext.Foreground,
                        Margin = TableCellPadding
                    };

                    paragraph.TextAlignment = element.ColumnDefinitions[cellIndex].Alignment switch
                    {
                        ColumnAlignment.Right => TextAlignment.Right,
                        ColumnAlignment.Center => TextAlignment.Center,
                        _ => TextAlignment.Left
                    };

                    if (rowIndex == 0)
                    {
                        paragraph.FontWeight = FontWeights.Bold;
                    }

                    var childContext = new InlineRenderContext(paragraph.Inlines, context)
                    {
                        Parent = paragraph,
                        TrimLeadingWhitespace = true
                    };

                    RenderInlineChildren(cell.Inlines, childContext);

                    var tablecell = new TableCell(paragraph)
                    {
                        BorderThickness = borderthickness,
                        BorderBrush = TableBorderBrush
                    };
                    tableRG.Rows[rowIndex].Cells.Add(tablecell);
                }
            }

            table.Margin = TableMargin;
            table.BorderBrush = TableBorderBrush;
            table.BorderThickness = borderthickness;

            blockCollection.Add(table);
        }
    }
}
