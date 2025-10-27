namespace ReportBuilder.Services;

public class ToastService
{
    private readonly List<ToastNotification> _toasts = new();
    public event Action? OnChange;

    public IReadOnlyList<ToastNotification> Toasts => _toasts.AsReadOnly();

    public void ShowSuccess(string message, int durationMs = 10000)
    {
        Show(message, ToastType.Success, durationMs);
    }

    public void ShowError(string message, int durationMs = 10000)
    {
        Show(message, ToastType.Error, durationMs);
    }

    public void ShowInfo(string message, int durationMs = 10000)
    {
        Show(message, ToastType.Info, durationMs);
    }

    public void ShowWarning(string message, int durationMs = 10000)
    {
        Show(message, ToastType.Warning, durationMs);
    }

    private void Show(string message, ToastType type, int durationMs)
    {
        var toast = new ToastNotification
        {
            Id = Guid.NewGuid().ToString(),
            Message = message,
            Type = type,
            DurationMs = durationMs
        };

        _toasts.Add(toast);
        OnChange?.Invoke();

        // Auto-dismiss after duration
        if (durationMs > 0)
        {
            _ = Task.Delay(durationMs).ContinueWith(_ => Dismiss(toast.Id));
        }
    }

    public void Dismiss(string toastId)
    {
        var toast = _toasts.FirstOrDefault(t => t.Id == toastId);
        if (toast != null)
        {
            _toasts.Remove(toast);
            OnChange?.Invoke();
        }
    }

    public void DismissAll()
    {
        _toasts.Clear();
        OnChange?.Invoke();
    }
}

public class ToastNotification
{
    public string Id { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ToastType Type { get; set; }
    public int DurationMs { get; set; }
}

public enum ToastType
{
    Success,
    Error,
    Info,
    Warning
}
