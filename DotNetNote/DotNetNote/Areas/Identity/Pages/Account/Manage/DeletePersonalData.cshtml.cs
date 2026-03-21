using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;

namespace VisualAcademy.Areas.Identity.Pages.Account.Manage;

public class DeletePersonalDataModel(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ILogger<DeletePersonalDataModel> logger) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new(); // 초기화

    public class InputModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty; // null 방지
    }

    public bool RequirePassword { get; set; }

    public async Task<IActionResult> OnGet()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        RequirePassword = await userManager.HasPasswordAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        RequirePassword = await userManager.HasPasswordAsync(user);

        if (RequirePassword)
        {
            if (!await userManager.CheckPasswordAsync(user, Input.Password))
            {
                ModelState.AddModelError(string.Empty, "Password not correct.");
                return Page();
            }
        }

        var result = await userManager.DeleteAsync(user);
        var userId = await userManager.GetUserIdAsync(user);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Unexpected error occurred deleting user with ID '{userId}'.");
        }

        await signInManager.SignOutAsync();

        logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

        return Redirect("~/");
    }
}