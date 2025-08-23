using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace ClearBudget.Web.Components.Pages;

public partial class Error : CustomServerComponent
{
    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    private string? RequestId { get; set; }
    private bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    protected override void OnInitialized() => RequestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier;
}