using ModernWpf.Toolkit.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ModernWpf.Toolkit.SampleApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            MDbox.LinkClicked += MDbox_LinkClicked;
            MDbox.ImageClicked += MDbox_ImageClicked;
            MDbox.CodeBlockResolving += MDbox_CodeBlockResolving;
            //MDbox.EmojiInlineResolving += MDbox_EmojiInlineResolving;
            MDbox.MarkdownRendered += MDbox_MarkdownRendered;
        }

        private void MDbox_MarkdownRendered(object sender, MarkdownRenderedEventArgs e)
        {
            if (e.Exception == null && MDbox.Text != null)
            {
                Debug.WriteLine("Rendered yay!");
            }
        }

        //private void MDbox_EmojiInlineResolving(object sender, EmojiInlineResolvingEventArgs e)
        //{
        //    try
        //    {
        //        double fontSize = Math.Max(16.0, MDbox.FontSize);
        //        e.EmojiInline = new .EmojiUIElementInline { Text = e.EmojiString, FontSize = fontSize, FallbackBrush = MDbox.Foreground };
        //        e.Handled = true;
        //    }
        //    catch
        //    {
        //        e.EmojiInline = null;
        //        e.Handled = false;
        //    }
        //}

        private void MDbox_CodeBlockResolving(object sender, CodeBlockResolvingEventArgs e)
        {
            if (e.CodeLanguage == "CUSTOM")
            {
                e.Handled = true;
                e.InlineCollection.Add(new Run { Foreground = new SolidColorBrush(Colors.Red), Text = e.Text, FontWeight = FontWeights.Bold });
            }
        }

        private void MDbox_ImageClicked(object sender, LinkClickedEventArgs e)
        {
            if (!Uri.TryCreate(e.Link, UriKind.Absolute, out Uri result))
            {
                MessageBox.Show("Masked relative Images needs to be manually handled.");
            }
            else
            {
                var psi = new ProcessStartInfo()
                {
                    FileName = e.Link,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
        }

        private void MDbox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (!Uri.TryCreate(e.Link, UriKind.Absolute, out Uri result))
            {
                MessageBox.Show("Masked relative links needs to be manually handled.");
            }
            else
            {
                var psi = new ProcessStartInfo()
                {
                    FileName = e.Link,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string initialmd = GetInitialMD();
            EditorBox.Text = initialmd;
        }

        private string GetInitialMD()
        {
            string output = "";

            output = File.ReadAllText("InitialContent.md");

            return output;
        }
    }
}
