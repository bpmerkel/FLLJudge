using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Services;

namespace FLLJudge.Client.Pages;

/// <summary>
/// Represents the main page of the application.
/// </summary>
public partial class Index
{
    /// <summary>
    /// Gets or sets the task to get the model.
    /// </summary>
    [Inject] private Task<Model> GetModel { get; set; }

    /// <summary>
    /// Gets or sets the dialog service.
    /// </summary>
    [Inject] private IDialogService DialogService { get; set; }

    /// <summary>
    /// Gets or sets the browser viewport service.
    /// </summary>
    [Inject] private IBrowserViewportService BrowserViewportService { get; set; }

    /// <summary>
    /// Gets or sets the font size.
    /// </summary>
    private bool? fontSize = true; // tri-state; fontSize == null ? Size.Large : fontSize == true ? Size.Small : Size.Medium

    /// <summary>
    /// Represents the model of the application.
    /// </summary>
    private Model model;

    /// <summary>
    /// Represents whether the filter is on.
    /// </summary>
    private bool FilterOn;

    /// <summary>
    /// Represents the selected chips.
    /// </summary>
    private MudChip[] selected;

    /// <summary>
    /// Represents the selected item.
    /// </summary>
    private MudListItem selectedItem;

    /// <summary>
    /// Gets the search terms.
    /// </summary>
    private IEnumerable<string> SearchTerms => selected == null
        ? []
        : selected.Select(c => c.Text);

    /// <summary>
    /// Represents the width of the browser viewport.
    /// </summary>
    private int _width = 0;

    /// <summary>
    /// Gets a value indicating whether the screen is small.
    /// </summary>
    private bool IsSmallScreen => _width < 900;

    /// <summary>
    /// Initializes the component.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        model = await GetModel;
        StateHasChanged();
    }

    /// <summary>
    /// Executes after the component is rendered.
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await BrowserViewportService.SubscribeAsync(this, fireImmediately: true);
            OpenWelcomeDialog();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// Opens the welcome dialog.
    /// </summary>
    private void OpenWelcomeDialog() => DialogService.Show<WelcomeDialog>("Welcome", new DialogOptions
    {
        MaxWidth = MaxWidth.Large,
        CloseButton = true,
        DisableBackdropClick = true,
        NoHeader = false,
        Position = DialogPosition.Center,
        CloseOnEscapeKey = true
    });

    /// <summary>
    /// Gets the name of the section.
    /// </summary>
    private string GetSectionName(Comment.Sections section) => section switch
    {
        Comment.Sections.ThinkAbout => "Think about...",
        Comment.Sections.GreatJob => "Great job...",
        _ => section.ToString()
    };

    /// <summary>
    /// Gets the name of the area.
    /// </summary>
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

    /// <summary>
    /// Resets the component.
    /// </summary>
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

    /// <summary>
    /// Gets the ID of the browser viewport observer.
    /// </summary>
    Guid IBrowserViewportObserver.Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Gets the resize options of the browser viewport observer.
    /// </summary>
    ResizeOptions IBrowserViewportObserver.ResizeOptions { get; } = new()
    {
        ReportRate = 1000,
        NotifyOnBreakpointOnly = false
    };

    /// <summary>
    /// Notifies the browser viewport change.
    /// </summary>
    Task IBrowserViewportObserver.NotifyBrowserViewportChangeAsync(BrowserViewportEventArgs browserViewportEventArgs)
    {
        _width = browserViewportEventArgs.BrowserWindowSize.Width;
        //_height = browserViewportEventArgs.BrowserWindowSize.Height;
        return InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Disposes the component.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await BrowserViewportService.UnsubscribeAsync(this);
        GC.SuppressFinalize(this);
    }
}
