using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FLLJudge.Client.Pages;

public partial class WelcomeDialog
{
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

    private void Submit() => MudDialog.Close(DialogResult.Ok(true));

    private string GetVersion()
    {
        var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        var buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
        return $"{version} - {buildDate:ddd MM-dd-yyyy hh\\:mm tt}";
    }
}