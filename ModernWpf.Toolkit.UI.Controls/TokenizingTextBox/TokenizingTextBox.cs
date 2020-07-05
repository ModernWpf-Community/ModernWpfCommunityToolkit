﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ModernWpf.Controls;
using ModernWpf.Toolkit.Deferred;
using ModernWpf.Toolkit.UI.Extensions;
using ModernWpf.Toolkit.UI.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ModernWpf.Toolkit.UI.Controls
{
    /// <summary>
    /// A text input control that auto-suggests and displays token items.
    /// </summary>
    [TemplateVisualState(GroupName = "CommonStates", Name = PART_NormalState)]
    [TemplateVisualState(GroupName = "CommonStates", Name = PART_MouseOverState)]
    [TemplateVisualState(GroupName = "FocusStates", Name = PART_FocusedState)]
    [TemplateVisualState(GroupName = "FocusStates", Name = PART_UnfocusedState)]
    public partial class TokenizingTextBox : ListViewBase
    {
        internal const string PART_NormalState = "Normal";
        internal const string PART_MouseOverState = "MouseOver";
        internal const string PART_FocusedState = "Focused";
        internal const string PART_UnfocusedState = "Unfocused";

        /// <summary>
        /// Gets a value indicating whether the shift key is currently in a pressed state
        /// </summary>
        internal static bool IsShiftPressed => Keyboard.Modifiers == ModifierKeys.Shift;

        /// <summary>
        /// Gets a value indicating whether the control key is currently in a pressed state
        /// </summary>
        internal bool IsControlPressed => Keyboard.Modifiers == ModifierKeys.Control;

        internal bool PauseTokenClearOnFocus { get; set; }

        internal bool IsClearingForClick { get; set; }

        private InterspersedObservableCollection _innerItemsSource;
        private ITokenStringContainer _currentTextEdit; // Don't update this directly outside of initialization, use UpdateCurrentTextEdit Method - in future see https://github.com/dotnet/csharplang/issues/140#issuecomment-625012514
        private ITokenStringContainer _lastTextEdit;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenizingTextBox"/> class.
        /// </summary>
        public TokenizingTextBox()
        {
            // Setup our base state of our collection
            _innerItemsSource = new InterspersedObservableCollection(new ObservableCollection<object>()); // TODO: Test this still will let us bind to ItemsSource in XAML?
            _currentTextEdit = _lastTextEdit = new PretokenStringContainer(true);
            _innerItemsSource.Insert(_innerItemsSource.Count, _currentTextEdit);
            ItemsSource = _innerItemsSource;
            //// TODO: Consolidate with callback below for ItemsSourceProperty changed?

            DefaultStyleKey = typeof(TokenizingTextBox);

            // TODO: Do we want to support ItemsSource better? Need to investigate how that works with adding...
            this.RegisterPropertyChangedCallback(ItemsSourceProperty, ItemsSource_PropertyChanged);
            PreviewKeyDown += TokenizingTextBox_PreviewKeyDown;
            PreviewKeyUp += TokenizingTextBox_PreviewKeyUp;
            ItemClick += TokenizingTextBox_ItemClick;
        }

        private void ItemsSource_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // If we're given a different ItemsSource, we need to wrap that collection in our helper class.
            if (ItemsSource != null && ItemsSource.GetType() != typeof(InterspersedObservableCollection))
            {
                _innerItemsSource = new InterspersedObservableCollection(ItemsSource);
                _currentTextEdit = _lastTextEdit = new PretokenStringContainer(true);
                _innerItemsSource.Insert(_innerItemsSource.Count, _currentTextEdit);
                ItemsSource = _innerItemsSource;
            }
        }

        private void TokenizingTextBox_ItemClick(object sender, ItemClickEventArgs e)
        {
            // If the user taps an item in the list, make sure to clear any text selection as required
            // Note, token selection is cleared by the listview default behaviour
            if (!IsControlPressed)
            {
                // Set class state flag to prevent click item being immediately deselected
                IsClearingForClick = true;
                ClearAllTextSelections(null);
            }
        }

        private void TokenizingTextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            TokenizingTextBox_PreviewKeyUp(e.Key);
        }

        internal void TokenizingTextBox_PreviewKeyUp(Key key)
        {
            switch (key)
            {
                case Key.Escape:
                    {
                        // Clear any selection and place the focus back into the text box
                        DeselectAllTokensAndText();
                        FocusPrimaryAutoSuggestBox();
                        break;
                    }
            }
        }

        /// <summary>
        /// Set the focus to the last item in the collection
        /// </summary>
        private void FocusPrimaryAutoSuggestBox()
        {
            if (Items?.Count > 0)
            {
                (ItemContainerGenerator.ContainerFromIndex(Items.Count - 1) as TokenizingTextBoxItem).Focus();
            }
        }

        private async void TokenizingTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = await TokenizingTextBox_PreviewKeyDown(e.Key);
        }

        internal async Task<bool> TokenizingTextBox_PreviewKeyDown(Key key)
        {
            // Global handlers on control regardless if focused on item or in textbox.
            switch (key)
            {
                case Key.C:
                    if (IsControlPressed)
                    {
                        CopySelectedToClipboard();
                        return true;
                    }

                    break;

                case Key.X:
                    if (IsControlPressed)
                    {
                        CopySelectedToClipboard();

                        // now clear all selected tokens and text, or all if none are selected
                        await RemoveAllSelectedTokens();
                    }

                    break;

                // For moving between tokens
                case Key.Left:
                    return MoveFocusAndSelection(MoveDirection.Previous);

                case Key.Right:
                    return MoveFocusAndSelection(MoveDirection.Next);

                case Key.A:
                    // modify the select-all behaviour to ensure the text in the edit box gets selected.
                    if (IsControlPressed)
                    {
                        SelectAllTokensAndText();
                        return true;
                    }

                    break;
            }

            return false;
        }

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var selectAllMenuItem = new MenuItem
            {
                Header = "SelectAll"
            };
            selectAllMenuItem.Click += (s, e) => SelectAllTokensAndText();
            var contextMenu = new ContextMenu();
            contextMenu.Items.Add(selectAllMenuItem);

            ContextMenu = contextMenu;
        }

        internal void RaiseQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            QuerySubmitted?.Invoke(sender, args);
        }

        internal void RaiseSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            SuggestionChosen?.Invoke(sender, args);
        }

        internal void RaiseTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            TextChanged?.Invoke(sender, args);
        }

        private async void TokenizingTextBox_CharacterReceived(Key key)
        {
            var container = ItemContainerGenerator.ContainerFromItem(_currentTextEdit) as TokenizingTextBoxItem;

            if (container != null && !(GetFocusedElement() == container._autoSuggestTextBox || IsControlPressed))
            {
                if (SelectedItems.Count > 0)
                {
                    var index = _innerItemsSource.IndexOf(SelectedItems[0]);

                    await RemoveAllSelectedTokens();

                    // Wait for removal of old items
                    _ = Dispatcher.InvokeAsync(() =>
                    {
                        // If we're before the last textbox and it's empty, redirect focus to that one instead
                        if (index == _innerItemsSource.Count - 1 && string.IsNullOrWhiteSpace(_lastTextEdit.Text))
                        {
                            var lastContainer = ItemContainerGenerator.ContainerFromItem(_lastTextEdit) as TokenizingTextBoxItem;

                            lastContainer.UseCharacterAsUser = true; // Make sure we trigger a refresh of suggested items.

                            _lastTextEdit.Text = string.Empty + key.ToString();

                            UpdateCurrentTextEdit(_lastTextEdit);

                            lastContainer._autoSuggestTextBox.SelectionStart = 1; // Set position to after our new character inserted

                            Keyboard.Focus(lastContainer._autoSuggestTextBox);
                        }
                        else
                        {
                            //// Otherwise, create a new textbox for this text.

                            UpdateCurrentTextEdit(new PretokenStringContainer((string.Empty + key.ToString()).Trim())); // Trim so that 'space' isn't inserted and can be used to insert a new box.

                            _innerItemsSource.Insert(index, _currentTextEdit);

                            // Need to wait for containerization
                            _ = Dispatcher.InvokeAsync(() =>
                            {
                                var newContainer = ItemContainerGenerator.ContainerFromIndex(index) as TokenizingTextBoxItem; // Should be our last text box

                                newContainer.UseCharacterAsUser = true; // Make sure we trigger a refresh of suggested items.

                                void WaitForLoad(object s, RoutedEventArgs eargs)
                                {
                                    if (newContainer._autoSuggestTextBox != null)
                                    {
                                        newContainer._autoSuggestTextBox.SelectionStart = 1; // Set position to after our new character inserted

                                        Keyboard.Focus(newContainer._autoSuggestTextBox);
                                    }

                                    newContainer.Loaded -= WaitForLoad;
                                }

                                newContainer.AutoSuggestTextBoxLoaded += WaitForLoad;
                            }, DispatcherPriority.Normal);
                        }
                    }, DispatcherPriority.Normal);
                }
                else
                {
                    // TODO: It looks like we're setting selection and focus together on items? Not sure if that's what we want...
                    // If that's the case, don't think this code will ever be called?

                    //// TODO: Behavior question: if no items selected (just focus) does it just go to our last active textbox?
                    //// Community voted that typing in the end box made sense

                    if (_innerItemsSource[_innerItemsSource.Count - 1] is ITokenStringContainer textToken)
                    {
                        var last = ItemContainerGenerator.ContainerFromIndex(Items.Count - 1) as TokenizingTextBoxItem; // Should be our last text box
                        var position = last._autoSuggestTextBox.SelectionStart;
                        textToken.Text = last._autoSuggestTextBox.Text.Substring(0, position) + key.ToString() +
                                         last._autoSuggestTextBox.Text.Substring(position);

                        last._autoSuggestTextBox.SelectionStart = position + 1; // Set position to after our new character inserted

                        Keyboard.Focus(last._autoSuggestTextBox);
                    }
                }
            }
        }

        private object GetFocusedElement()
        {
            return FocusManager.GetFocusedElement(this);
        }

        #region ItemsControl Container Methods

        /// <inheritdoc/>
        protected override DependencyObject GetContainerForItemOverride() => new TokenizingTextBoxItem();

        /// <inheritdoc/>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TokenizingTextBoxItem;
        }

        /// <inheritdoc/>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            var tokenitem = element as TokenizingTextBoxItem;

            tokenitem.Owner = this;

            tokenitem.ContentTemplateSelector = TokenItemTemplateSelector;
            tokenitem.ContentTemplate = TokenItemTemplate;

            tokenitem.ClearClicked -= TokenizingTextBoxItem_ClearClicked;
            tokenitem.ClearClicked += TokenizingTextBoxItem_ClearClicked;

            tokenitem.ClearAllAction -= TokenizingTextBoxItem_ClearAllAction;
            tokenitem.ClearAllAction += TokenizingTextBoxItem_ClearAllAction;

            tokenitem.GotFocus -= TokenizingTextBoxItem_GotFocus;
            tokenitem.GotFocus += TokenizingTextBoxItem_GotFocus;

            tokenitem.LostFocus -= TokenizingTextBoxItem_LostFocus;
            tokenitem.LostFocus += TokenizingTextBoxItem_LostFocus;

            var contextMenu = new ContextMenu();

            var removeMenuItem = new MenuItem
            {
                Header = "Remove"
            };
            removeMenuItem.Click += (s, e) => TokenizingTextBoxItem_ClearClicked(tokenitem, null);

            contextMenu.Items.Add(removeMenuItem);

            var selectAllMenuItem = new MenuItem
            {
                Header = "SelectAll"
            };
            selectAllMenuItem.Click += (s, e) => SelectAllTokensAndText();

            contextMenu.Items.Add(selectAllMenuItem);

            tokenitem.ContextMenu = contextMenu;
        }
        #endregion

        private void TokenizingTextBoxItem_GotFocus(object sender, RoutedEventArgs e)
        {
            // Keep track of our currently focused textbox
            if (sender is TokenizingTextBoxItem ttbi && ttbi.Content is ITokenStringContainer text)
            {
                UpdateCurrentTextEdit(text);
            }
        }

        private void TokenizingTextBoxItem_LostFocus(object sender, RoutedEventArgs e)
        {
            // Keep track of our currently focused textbox
            if (sender is TokenizingTextBoxItem ttbi && ttbi.Content is ITokenStringContainer text &&
                string.IsNullOrWhiteSpace(text.Text) && text != _lastTextEdit)
            {
                // We're leaving an inner textbox that's blank, so we'll remove it
                _innerItemsSource.Remove(text);

                UpdateCurrentTextEdit(_lastTextEdit);

                GuardAgainstPlaceholderTextLayoutIssue();
            }
        }

        /// <summary>
        /// Adds the specified data item as a new token to the collection, will raise the <see cref="TokenItemAdding"/> event asynchronously still for confirmation.
        /// </summary>
        /// <remarks>
        /// The <see cref="TokenizingTextBox"/> will automatically handle adding items for you, or you can add items to your backing <see cref="ItemsControl.ItemsSource"/> collection. This method is provide for other cases where you may need to drive inserting a new token item to where the user is currently inserting text between tokens.
        /// </remarks>
        /// <param name="data">Item to add as a token.</param>
        /// <param name="atEnd">Flag to indicate if the item should be inserted in the last used textbox (inserted) or placed at end of the token list.</param>
        public void AddTokenItem(object data, bool atEnd = false)
        {
            _ = AddTokenAsync(data, atEnd);
        }

        /// <summary>
        /// Clears the whole collection, will raise the <see cref="TokenItemRemoving"/> event asynchronously for each item.
        /// </summary>
        /// <returns>async task</returns>
        public async Task ClearAsync()
        {
            while (_innerItemsSource.Count > 1)
            {
                var container = ItemContainerGenerator.ContainerFromItem(_innerItemsSource[0]) as TokenizingTextBoxItem;
                if (!await RemoveTokenAsync(container, _innerItemsSource[0]))
                {
                    // if a removal operation fails then stop the clear process
                    break;
                }
            }

            // Clear the active pretoken string.
            // Setting the text property directly avoids a delay when setting the text in the autosuggest box.
            Text = string.Empty;
        }

        internal async Task AddTokenAsync(object data, bool? atEnd = null)
        {
            if (data is string str && TokenItemAdding != null)
            {
                var tiaea = new TokenItemAddingEventArgs(str);
                await TokenItemAdding.InvokeAsync(this, tiaea);

                if (tiaea.Cancel)
                {
                    return;
                }

                if (tiaea.Item != null)
                {
                    data = tiaea.Item; // Transformed by event implementor
                }
            }

            // If we've been typing in the last box, just add this to the end of our collection
            if (atEnd == true || _currentTextEdit == _lastTextEdit)
            {
                _innerItemsSource.InsertAt(_innerItemsSource.Count - 1, data);
            }
            else
            {
                // Otherwise, we'll insert before our current box
                var edit = _currentTextEdit;
                var index = _innerItemsSource.IndexOf(edit);

                // Insert our new data item at the location of our textbox
                _innerItemsSource.InsertAt(index, data);

                // Remove our textbox
                _innerItemsSource.Remove(edit);
            }

            // Focus back to our end box as Outlook does.
            var last = ItemContainerGenerator.ContainerFromItem(_lastTextEdit) as TokenizingTextBoxItem;
            Keyboard.Focus(last?._autoSuggestTextBox);

            TokenItemAdded?.Invoke(this, data);

            GuardAgainstPlaceholderTextLayoutIssue();
        }

        /// <summary>
        /// Helper to change out the currently focused text element in the control.
        /// </summary>
        /// <param name="edit"><see cref="ITokenStringContainer"/> element which is now the main edited text.</param>
        protected void UpdateCurrentTextEdit(ITokenStringContainer edit)
        {
            _currentTextEdit = edit;

            Text = edit.Text; // Update our text property.
        }

        /// <summary>
        /// Remove the specified token from the list.
        /// </summary>
        /// <param name="item">Item in the list to delete</param>
        /// <param name="data">data </param>
        /// <remarks>
        /// the data parameter is passed in optionally to support UX UTs. When running in the UT the Container items are not manifest.
        /// </remarks>
        /// <returns><b>true</b> if the item was removed successfully, <b>false</b> otherwise</returns>
        private async Task<bool> RemoveTokenAsync(TokenizingTextBoxItem item, object data = null)
        {
            if (data == null)
            {
                data = ItemContainerGenerator.ItemFromContainer(item);
            }

            if (TokenItemRemoving != null)
            {
                var tirea = new TokenItemRemovingEventArgs(data, item);
                await TokenItemRemoving.InvokeAsync(this, tirea);

                if (tirea.Cancel)
                {
                    return false;
                }
            }

            _innerItemsSource.Remove(data);

            TokenItemRemoved?.Invoke(this, data);

            GuardAgainstPlaceholderTextLayoutIssue();

            return true;
        }

        private void GuardAgainstPlaceholderTextLayoutIssue()
        {
            // If the *PlaceholderText is visible* on the last AutoSuggestBox, it can incorrectly layout itself
            // when the *ASB has focus*. We think this is an optimization in the platform, but haven't been able to
            // isolate a straight-reproduction of this issue outside of this control (though we have eliminated
            // most Toolkit influences like ASB/TextBox Style, the InterspersedObservableCollection, etc...).
            // The only Toolkit component involved here should be WrapPanel (which is a straight-forward Panel).
            // We also know the ASB itself is adjusting it's size correctly, it's the inner component.
            //
            // To combat this issue:
            //   We toggle the visibility of the Placeholder ContentControl in order to force it's layout to update properly
            var placeholder = ItemContainerGenerator.ContainerFromItem(_lastTextEdit).FindDescendantByName("PlaceholderTextContentPresenter");

            if (placeholder?.Visibility == Visibility.Visible)
            {
                placeholder.Visibility = Visibility.Collapsed;

                // After we ensure we've hid the control, make it visible again (this is inperceptable to the user).
                _ = CompositionTargetHelper.ExecuteAfterCompositionRenderingAsync(() =>
                {
                    placeholder.Visibility = Visibility.Visible;
                });
            }
        }
    }
}
