using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernUITestApp.Views
{
    /// <summary>
    /// Interaction logic for ContextmenuEditorKR.xaml
    /// </summary>
    public partial class ContextmenuEditorKR : ContextMenu
    {
        private Popup _editorPopup;
        private TextBox _editorTextBox;
        private System.Action<string> _onSaveCallback;

        public ContextmenuEditorKR()
        {
            InitializeComponent();
            InitializeEditor();
        }

        private void InitializeEditor()
        {
            // Create the Popup programmatically because Popup behavior (stays open, etc.) 
            // is often better managed in code for this specific "Overlay" use case,
            // BUT we will use the Template from XAML for the content.
            _editorPopup = new Popup
            {
                StaysOpen = false,
                AllowsTransparency = true,
                PopupAnimation = PopupAnimation.Fade,
                Placement = PlacementMode.Bottom
            };

            // Load the ControlTemplate from Resources
            if (this.TryFindResource("EditorTemplate") is ControlTemplate editorTemplate)
            {
                // Directly instantiate the visual tree from the template
                var content = (UIElement)editorTemplate.LoadContent();

                // Helper to find visual child after template application
                _editorTextBox = FindVisualChild<TextBox>(content);
                if (_editorTextBox != null)
                {
                    _editorTextBox.PreviewKeyDown += EditorBox_PreviewKeyDown;
                }

                _editorPopup.Child = content;
            }
        }

        // --- Public API ---

        /// <summary>
        /// Shows the inline editor with the specified text.
        /// </summary>
        public void ShowEditor(string initialText, System.Action<string> onSave)
        {
            if (_editorPopup == null || _editorTextBox == null) return;

            _onSaveCallback = onSave;

            // 1. Capture target before closing to ensure valid context
            var target = this.PlacementTarget ?? Application.Current.MainWindow;

            // 2. Close Menu
            this.IsOpen = false;

            // 3. Setup Editor
            _editorTextBox.Text = initialText ?? string.Empty;

            // 4. Position Editor at Mouse Pointer
            _editorPopup.PlacementTarget = target;
            _editorPopup.Placement = PlacementMode.MousePoint;
            _editorPopup.StaysOpen = false;

            // 4. Open
            _editorPopup.IsOpen = true;

            // 5. Focus
            _editorTextBox.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new System.Action(() =>
            {
                _editorTextBox.Focus();
                _editorTextBox.SelectAll();
            }));
        }

        // --- Event Handlers ---

        private void EditorBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    return; // New Line
                }
                else
                {
                    // Commit
                    _onSaveCallback?.Invoke(_editorTextBox.Text);
                    _editorPopup.IsOpen = false;
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Escape)
            {
                // Cancel
                _editorPopup.IsOpen = false;
                e.Handled = true;
            }
        }

        // Helper to find named part in Template
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typed)
                {
                    return typed;
                }
                var result = FindVisualChild<T>(child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}
