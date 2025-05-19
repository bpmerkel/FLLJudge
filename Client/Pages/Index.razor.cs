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
    private bool? fontSize = false; // tri-state; fontSize == null ? Size.Large : fontSize == true ? Size.Small : Size.Medium

    /// <summary>
    /// Represents the model of the application.
    /// </summary>
    private Model model;

    /// <summary>
    /// Represents whether the filter is on.
    /// </summary>
    private bool FilterOn;

    /// <summary>
    /// Represents the selected item.
    /// </summary>
    private Comment selectedItem;

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
    private void OpenWelcomeDialog() => DialogService.ShowAsync<WelcomeDialog>("Welcome", new DialogOptions
    {
        MaxWidth = MaxWidth.Large,
        CloseButton = true,
        BackdropClick = false,
        NoHeader = false,
        Position = DialogPosition.Center,
        CloseOnEscapeKey = true
    });

    /// <summary>
    /// Gets the name of the section.
    /// </summary>
    private static string GetSectionName(Comment.Sections section) => section switch
    {
        Comment.Sections.ThinkAbout => "Think about...",
        Comment.Sections.GreatJob => "Great job...",
        _ => section.ToString()
    };

    /// <summary>
    /// Gets the name of the area.
    /// #fcd2c0
    /// #c6eafa
    /// #cbe6d3
    /// </summary>
    private static (string description, string color) GetAreaInfo(string Name) => Name switch
    {
        "Core Values" => ("How did the team demonstrate teamwork, discovery, inclusion, innovation, impact, and fun in their work?", "#fcd2c0"),
        "Innovation Project" => ("How did the team identify and approach solving a problem connected to the season theme?", "#c6eafa"),
        "Robot Design" => ("How did the team approach solving robot game missions using building and coding?", "#cbe6d3"),
        _ => (string.Empty, "#000000")
    };

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
