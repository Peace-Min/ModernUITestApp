using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernUITestApp.ViewModels;

namespace ModernUITestApp.Views
{
    public partial class GlobalEditorOverlayKR : UserControl
    {
        // --- 상태 ---
        private AnnotationViewModel            _currentTarget;
        private string                         _originalText;
        private System.Action<string>          _onSaveCallback;

        // AllowsTransparency=False 독립 Window → 한글 IME 정상 동작
        private EditorInputWindow _editorWindow;

        public GlobalEditorOverlayKR()
        {
            InitializeComponent();

            // 에디터 Window 단일 인스턴스 생성 (Show/Hide 방식으로 재사용)
            _editorWindow = new EditorInputWindow();
            _editorWindow.SaveRequested   += OnEditorSaved;
            _editorWindow.CancelRequested += OnEditorCancelled;
        }

        // --- PUBLIC API ---

        public void ShowMenu(AnnotationViewModel target, double x, double y)
        {
            _currentTarget = target;

            // 에디터가 열려 있으면 닫기
            _editorWindow.CloseEditor();

            // 메뉴 위치 및 열기
            MenuPopup.HorizontalOffset = x;
            MenuPopup.VerticalOffset   = y;
            MenuPopup.IsOpen           = true;
        }

        public void ShowEditor(AnnotationViewModel target)
        {
            _currentTarget  = target;
            _onSaveCallback = null;
            if (_currentTarget == null) return;

            MenuPopup.IsOpen = false;

            _originalText = _currentTarget.Text;
            _editorWindow.OpenAt(_originalText,
                                 MenuPopup.HorizontalOffset,
                                 MenuPopup.VerticalOffset);
        }

        public void ShowEditor(string text, System.Action<string> onSave, Point position)
        {
            _currentTarget  = null;
            _onSaveCallback = onSave;

            MenuPopup.IsOpen = false;

            _originalText = text;
            _editorWindow.OpenAt(text, position.X, position.Y);
        }

        public void HideAll()
        {
            MenuPopup.IsOpen = false;
            _editorWindow.CloseEditor();
        }

        // --- INTERNAL HANDLERS ---

        private void OnEditClicked(object sender, RoutedEventArgs e)
        {
            if (_currentTarget == null) return;

            // 1. 메뉴 닫기
            double posX = MenuPopup.HorizontalOffset;
            double posY = MenuPopup.VerticalOffset;
            MenuPopup.IsOpen = false;

            // 2. 에디터 창 열기 (위치는 메뉴가 있던 좌표)
            _originalText = _currentTarget.Text;
            _editorWindow.OpenAt(_originalText, posX, posY);
        }

        private void OnDeleteClicked(object sender, RoutedEventArgs e)
        {
            if (_currentTarget?.DeleteCommand?.CanExecute(null) == true)
                _currentTarget.DeleteCommand.Execute(null);

            MenuPopup.IsOpen = false;
        }

        private void OnEditorSaved(string newText)
        {
            if (_currentTarget != null)
                _currentTarget.Text = newText;
            else
                _onSaveCallback?.Invoke(newText);
        }

        private void OnEditorCancelled()
        {
            // 필요 시 원본 텍스트로 복원
            // _currentTarget?.Text = _originalText;
        }
    }
}
