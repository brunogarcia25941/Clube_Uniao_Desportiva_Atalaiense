// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail; // Adicionado para SmtpException
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Clube_Uniao_Desportiva_Atalaiense.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User {UserEmail} account created successfully in database.", Input.Email);

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    _logger.LogInformation("Preparing to send confirmation email to {UserEmail}. Callback URL: {CallbackUrl}", Input.Email, callbackUrl);
                    try
                    {
                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        _logger.LogInformation("Request to send email to {UserEmail} was made. Check EmailSender logs for final status of delivery.", Input.Email);
                    }
                    catch (SmtpException smtpEx)
                    {
                        _logger.LogError(smtpEx, "SmtpException caught in RegisterModel while sending email to {UserEmail}. Status Code: {StatusCode}. Email will likely not be sent. User account IS created.", Input.Email, smtpEx.StatusCode);
                        // Opcional: Adicionar um erro ao ModelState para informar o utilizador que o email pode não ter sido enviado,
                        // mas permitir que o registo continue.
                        // ModelState.AddModelError(string.Empty, "Houve um problema ao enviar o email de confirmação, mas a sua conta foi criada. Por favor, tente a confirmação mais tarde ou contacte o suporte.");
                    }
                    catch (TaskCanceledException tce)
                    {
                        _logger.LogError(tce, "TaskCanceledException caught in RegisterModel while sending email to {UserEmail}. This might indicate a timeout or other issue with the email sending task. Email was not sent. User account IS created.", Input.Email);
                        // ModelState.AddModelError(string.Empty, "O envio do email de confirmação demorou demasiado e foi cancelado. A sua conta foi criada. Por favor, tente a confirmação mais tarde ou contacte o suporte.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Generic exception caught in RegisterModel while sending email to {UserEmail}. Email was not sent. User account IS created.", Input.Email);
                        // ModelState.AddModelError(string.Empty, "Ocorreu um erro inesperado ao tentar enviar o email de confirmação. A sua conta foi criada. Por favor, tente a confirmação mais tarde ou contacte o suporte.");
                    }


                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        _logger.LogInformation("Redirecting user {UserEmail} to RegisterConfirmation page as account requires confirmation.", Input.Email);
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        _logger.LogInformation("Account for {UserEmail} does not require confirmation. Signing in user.", Input.Email);
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    _logger.LogWarning("User creation failed for {UserEmail}. Error: {ErrorDescription}", Input.Email, error.Description);
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            _logger.LogWarning("Model state was invalid for registration attempt with email {UserEmail}. Redisplaying page.", Input?.Email ?? "N/A");
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}