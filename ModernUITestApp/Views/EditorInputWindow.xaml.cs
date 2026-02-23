using System;
using System.Windows;
using System.Windows.Input;

namespace ModernUITestApp.Views
{
    /// <summary>
    /// WPF Popup 대신 AllowsTransparency=False 독립 Window를 사용합니다.
    /// AllowsTransparency=True Popup은 OS가 IME를 레이어드 윈도우에
    /// 정상 바인딩하지 못해 한글 입력 지연이 발생합니다.
    /// 표준 HWND(AllowsTransparency=False)만이 IME 컨텍스트를 올바르게 받습니다.
    /// </summary>
    public partial class EditorInputWindow : Window
    {
        // --- 이벤트 ---
        public event Action<string> SaveRequested;
        public event Action        CancelRequested;

        public EditorInputWindow()
        {
            InitializeComponent();

            // 외부 클릭 시 자동 닫기 (StaysOpen=False 동작 재현)
            Deactivated += OnWindowDeactivated;
        }

        // --- 공개 API ---

        /// <summary>텍스트를 초기화하고 에디터 창을 지정 좌표에 표시합니다.</summary>
        public void OpenAt(string initialText, double screenX, double screenY)
        {
            // 텍스트 설정
            EditorTextBox.Text = initialText ?? string.Empty;

            // 화면 좌표 배치
            Left = screenX;
            Top  = screenY;

            // 보이지 않으면 Show()로 HWND 생성, 이미 보이면 Activate()
            if (!IsVisible)
                Show();
            else
                Activate();

            // HWND 확정 후 포커스 (IME 컨텍스트가 이 HWND에 정상 연결)
            EditorTextBox.Focus();
            EditorTextBox.SelectAll();
        }

        /// <summary>에디터 창을 숨깁니다.</summary>
        public void CloseEditor()
        {
            if (IsVisible)
                Hide();
        }

        // --- 내부 핸들러 ---

        private void OnEditorPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Shift+Enter → 줄바꿈 (기본 동작 허용)
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                    return;

                // Enter → 저장
                InvokeSave();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                InvokeCancel();
                e.Handled = true;
            }
        }

        private void OnWindowDeactivated(object sender, EventArgs e)
        {
            // 외부 클릭으로 포커스를 잃으면 취소로 처리
            if (IsVisible)
                InvokeCancel();
        }

        private void InvokeSave()
        {
            var text = EditorTextBox.Text;
            Hide();
            SaveRequested?.Invoke(text);
        }

        private void InvokeCancel()
        {
            Hide();
            CancelRequested?.Invoke();
        }

        // Window가 닫히는 대신 Hide()만 사용하도록 재사용 보호
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            CancelRequested?.Invoke();
        }
    }
}
