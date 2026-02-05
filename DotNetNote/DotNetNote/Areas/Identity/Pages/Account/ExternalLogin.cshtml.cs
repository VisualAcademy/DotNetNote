#nullable enable

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace DotNetNote.Areas.Identity.Pages.Account
{
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            ILogger<ExternalLoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore(userManager, userStore);
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ProviderDisplayName { get; set; }
        public string? LoginProvider { get; set; }
        public string? ReturnUrl { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
        }

        public IActionResult OnGet() => RedirectToPage("./Login");

        public IActionResult OnPost(string provider, string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl = ReturnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (!string.IsNullOrEmpty(remoteError))
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info is null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl });
            }

            var result = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider, info.ProviderKey, false, true);

            if (result.Succeeded)
            {
                return LocalRedirect(ReturnUrl);
            }

            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }

            ProviderDisplayName = info.ProviderDisplayName;
            LoginProvider = info.LoginProvider;

            var email = info.Principal?.FindFirstValue(ClaimTypes.Email);
            Input.Email = email ?? string.Empty;

            return Page();
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info is null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl });
            }

            if (!ModelState.IsValid)
            {
                ProviderDisplayName = info.ProviderDisplayName;
                LoginProvider = info.LoginProvider;
                return Page();
            }

            var user = new ApplicationUser();
            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                ProviderDisplayName = info.ProviderDisplayName;
                LoginProvider = info.LoginProvider;
                return Page();
            }

            await _userManager.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, false, info.LoginProvider);

            return LocalRedirect(ReturnUrl);
        }

        private static IUserEmailStore<ApplicationUser> GetEmailStore(UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore)
        {
            if (!userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("User store must support email.");
            }
            return (IUserEmailStore<ApplicationUser>)userStore;
        }
    }
}
