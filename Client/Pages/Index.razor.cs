using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Services;

namespace FLLJudge.Client.Pages;

public partial class Index
{
    [Inject] private Task<Model> GetModel { get; set; }
    [Inject] private IDialogService DialogService { get; set; }
    [Inject] private IBrowserViewportService BrowserViewportService { get; set; }
    private bool? fontSize = true; // tri-state; fontSize == null ? Size.Large : fontSize == true ? Size.Small : Size.Medium
    private Model model;
    private bool FilterOn;
    private MudChip[] selected;
    private MudListItem selectedItem;
    private IEnumerable<string> SearchTerms => selected == null
        ? Array.Empty<string>()
        : selected.Select(c => c.Text);
    private int _width = 0;
    private bool IsSmallScreen => _width < 900;

    protected override async Task OnInitializedAsync()
    {
        model = await GetModel;
        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await BrowserViewportService.SubscribeAsync(this, fireImmediately: true);
            OpenWelcomeDialog();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private void OpenWelcomeDialog() => DialogService.Show<WelcomeDialog>("Welcome", new DialogOptions
    {
        MaxWidth = MaxWidth.Large,
        CloseButton = true,
        DisableBackdropClick = true,
        NoHeader = false,
        Position = DialogPosition.Center,
        CloseOnEscapeKey = true
    });

    private string GetSectionName(Comment.Sections section) => section switch
    {
        Comment.Sections.ThinkAbout => "Think about...",
        Comment.Sections.GreatJob => "Great job...",
        _ => section.ToString()
    };

    private string GetAreaName(string Name)
    {
        if (IsSmallScreen)
        {
            return Name switch
            {
                "Core Values" => "CV",
                "Innovation Project" => "IP",
                "Robot Design" => "RD",
                _ => Name
            };
        }
        return Name;
    }

    private void DoReset(MouseEventArgs e)
    {
        // perform a reset by clearing all selected comments and unchecking chips
        model.Areas.ForEach(a =>
        {
            a.Comments.ForEach(c => c.Selected = false);
        });

        selected = [];
        selectedItem = null;
        FilterOn = false;
        StateHasChanged();
    }

    Guid IBrowserViewportObserver.Id { get; } = Guid.NewGuid();

    ResizeOptions IBrowserViewportObserver.ResizeOptions { get; } = new()
    {
        ReportRate = 1000,
        NotifyOnBreakpointOnly = false
    };

    Task IBrowserViewportObserver.NotifyBrowserViewportChangeAsync(BrowserViewportEventArgs browserViewportEventArgs)
    {
        _width = browserViewportEventArgs.BrowserWindowSize.Width;
        //_height = browserViewportEventArgs.BrowserWindowSize.Height;
        return InvokeAsync(StateHasChanged);
    }

    public async ValueTask DisposeAsync() => await BrowserViewportService.UnsubscribeAsync(this);
}