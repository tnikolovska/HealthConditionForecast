using HealthConditionForecast.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;

namespace HealthConditionForecast.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByEmailAsync(Input.Email);
           /* if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don’t reveal the user does not exist or is not confirmed
                return RedirectToPage("./ForgotPasswordConfirmation");
            }
            // added for testing purposes
            if (user == null)
            {
                Console.WriteLine("User not found.");
                // Don’t reveal the user does not exist or is not confirmed
                return RedirectToPage("./ForgotPasswordConfirmation");
            }
            else if (!(await _userManager.IsEmailConfirmedAsync(user)))
            {
                Console.WriteLine("User email is not confirmed.");
                return RedirectToPage("./ForgotPasswordConfirmation");
            }
            else
            {
                Console.WriteLine("Sending password reset email...");
            }*/



            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code = token, email = Input.Email },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                Input.Email,
                "Reset Password",
                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return RedirectToPage("./ForgotPasswordConfirmation");
        }
    }
}