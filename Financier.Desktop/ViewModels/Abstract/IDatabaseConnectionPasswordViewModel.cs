using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IDatabaseConnectionPasswordViewModel
    {
        string UserId { get; }
        string Password { get; set; }
    }
}
