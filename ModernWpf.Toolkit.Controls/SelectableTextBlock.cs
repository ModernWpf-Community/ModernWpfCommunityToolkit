using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Toolkit.Controls
{
    internal class TextEditorWrapper
    {

#if NETFRAMEWORK
        private static readonly Type TextEditorType = Type.GetType("System.Windows.Documents.TextEditor, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
#else
        private static readonly Type TextEditorType = Type.GetType("System.Windows.Documents.TextEditor, PresentationFramework");
#endif
        private static readonly PropertyInfo IsReadOnlyProp = TextEditorType.GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly PropertyInfo TextViewProp = TextEditorType.GetProperty("TextView", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo RegisterMethod = TextEditorType.GetMethod("RegisterCommandHandlers",
            BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(Type), typeof(bool), typeof(bool), typeof(bool) }, null);


#if NETFRAMEWORK
        private static readonly Type TextContainerType = Type.GetType("System.Windows.Documents.ITextContainer, PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
#else
        private static readonly Type TextContainerType = Type.GetType("System.Windows.Documents.ITextContainer, PresentationFramework");
#endif
        private static readonly PropertyInfo TextContainerTextViewProp = TextContainerType.GetProperty("TextView");

        private static readonly PropertyInfo TextContainerProp = typeof(TextBlock).GetProperty("TextContainer", BindingFlags.Instance | BindingFlags.NonPublic);

        public static void RegisterCommandHandlers(Type controlType, bool acceptsRichContent, bool readOnly, bool registerEventListeners)
        {
            RegisterMethod.Invoke(null, new object[] { controlType, acceptsRichContent, readOnly, registerEventListeners });
        }

        public static TextEditorWrapper CreateFor(TextBlock tb)
        {
            var textContainer = TextContainerProp.GetValue(tb);

            var editor = new TextEditorWrapper(textContainer, tb, false);
            IsReadOnlyProp.SetValue(editor._editor, true);
            TextViewProp.SetValue(editor._editor, TextContainerTextViewProp.GetValue(textContainer));

            return editor;
        }

        private readonly object _editor;

        public TextEditorWrapper(object textContainer, FrameworkElement uiScope, bool isUndoEnabled)
        {
            _editor = Activator.CreateInstance(TextEditorType, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                null, new[] { textContainer, uiScope, isUndoEnabled }, null);
        }
    }

    internal class SelectableTextBlock : TextBlock
    {
        static SelectableTextBlock()
        {
            FocusableProperty.OverrideMetadata(typeof(SelectableTextBlock), new FrameworkPropertyMetadata(true));
            TextEditorWrapper.RegisterCommandHandlers(typeof(SelectableTextBlock), true, true, true);
        }

        public SelectableTextBlock()
        {
            _ = TextEditorWrapper.CreateFor(this);
        }
    }
}
