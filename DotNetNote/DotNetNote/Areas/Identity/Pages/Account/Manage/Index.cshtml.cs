using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VisualAcademy.Areas.Identity.Pages.Account.Manage;

public partial class IndexModel(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IEmailSender emailSender) : PageModel
{
    public string Username { get; set; } = string.Empty;

    public bool IsEmailConfirmed { get; set; }

    [TempData]
    public string StatusMessage { get; set; } = string.Empty;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; } = string.Empty;
    }

    private async Task<IActionResult?> LoadAsync(ApplicationUser user)
    {
        var userName = await userManager.GetUserNameAsync(user);
        var email = await userManager.GetEmailAsync(user);
        var phoneNumber = await userManager.GetPhoneNumberAsync(user);

        Username = userName ?? string.Empty;
        IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user);

        Input = new InputModel
        {
            Email = email ?? string.Empty,
            PhoneNumber = phoneNumber ?? string.Empty
        };

        return null;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        var currentEmail = await userManager.GetEmailAsync(user) ?? string.Empty;
        var currentPhoneNumber = await userManager.GetPhoneNumberAsync(user) ?? string.Empty;

        if (!string.Equals(Input.Email, currentEmail, StringComparison.Ordinal))
        {
            var setEmailResult = await userManager.SetEmailAsync(user, Input.Email);
            if (!setEmailResult.Succeeded)
            {
                var userId = await userManager.GetUserIdAsync(user);
                throw new InvalidOperationException(
                    $"Unexpected error occurred setting email for user with ID '{userId}'.");
            }
        }

        if (!string.Equals(Input.PhoneNumber, currentPhoneNumber, StringComparison.Ordinal))
        {
            var normalizedPhoneNumber = string.IsNullOrWhiteSpace(Input.PhoneNumber)
                ? null
                : Input.PhoneNumber;

            var setPhoneResult = await userManager.SetPhoneNumberAsync(user, normalizedPhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                var userId = await userManager.GetUserIdAsync(user);
                throw new InvalidOperationException(
                    $"Unexpected error occurred setting phone number for user with ID '{userId}'.");
            }
        }

        await signInManager.RefreshSignInAsync(user);
        StatusMessage = "Your profile has been updated";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSendVerificationEmailAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        var userId = await userManager.GetUserIdAsync(user);
        var email = await userManager.GetEmailAsync(user);

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidOperationException(
                $"Unable to send verification email because no email is registered for user with ID '{userId}'.");
        }

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new { userId, code },
            protocol: Request.Scheme);

        if (string.IsNullOrWhiteSpace(callbackUrl))
        {
            throw new InvalidOperationException("Unable to generate email confirmation link.");
        }

        await emailSender.SendEmailAsync(
            email,
            "Confirm your email",
            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        StatusMessage = "Verification email sent. Please check your email.";
        return RedirectToPage();
    }
}