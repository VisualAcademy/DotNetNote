using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VisualAcademy.Areas.Identity.Pages.Account.Manage;

public class TwoFactorAuthenticationModel(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : PageModel
{
    private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}";

    public bool HasAuthenticator { get; set; }

    public int RecoveryCodesLeft { get; set; }

    [BindProperty]
    public bool Is2faEnabled { get; set; }

    public bool IsMachineRemembered { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGet()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        HasAuthenticator = await userManager.GetAuthenticatorKeyAsync(user) != null;
        Is2faEnabled = await userManager.GetTwoFactorEnabledAsync(user);
        IsMachineRemembered = await signInManager.IsTwoFactorClientRememberedAsync(user);
        RecoveryCodesLeft = await userManager.CountRecoveryCodesAsync(user);

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        await signInManager.ForgetTwoFactorClientAsync();
        StatusMessage = "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.";
        return RedirectToPage();
    }
}