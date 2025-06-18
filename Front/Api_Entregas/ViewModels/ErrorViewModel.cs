namespace Api_Entregas.ViewModels;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public string? ErrorMsg { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
