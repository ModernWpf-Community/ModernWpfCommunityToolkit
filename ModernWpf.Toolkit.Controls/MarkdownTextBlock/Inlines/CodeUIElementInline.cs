using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ModernWpf.Toolkit.Controls.Markdown.Inlines
{
    internal class CodeUIElementInline : InlineUIContainer, IUIElementInline
    {
        public CodeUIElementInline()
        {

        }

        /// <summary>
        /// Redeclare the Child property to prevent it from being serialized.
        /// </summary>
        public new Border Child
        {
            get => base.Child as Border;
            internal set => base.Child = value;
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string), typeof(CodeUIElementInline),
            new PropertyMetadata(""));

        protected override bool ShouldSerializeProperty(DependencyProperty dp)
            => dp.Name == nameof(Text) && base.ShouldSerializeProperty(dp);

        protected bool ShouldSerializeChild() => false;

        public string GetUIContentString() => Text;
    }
}
