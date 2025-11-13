using System;
using System.Windows.Input;

namespace WpfApp4
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;             // 실행할 동작
        private readonly Func<bool>? _canExecute;     // 실행 가능 여부 (옵션)

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // 버튼이 눌릴 수 있는지 여부 확인
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute();
        }

        // 버튼이 눌렸을 때 실행되는 로직
        public void Execute(object? parameter)
        {
            _execute();
        }

        // 버튼 활성화 상태를 다시 확인하게 만드는 이벤트
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
