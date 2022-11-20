// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using LinkedLanguages.BL.Facades;
using LinkedLanguages.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace LinkedLanguages.Areas.Identity.Pages.Account.Manage
{
    public class DownloadPersonalDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SetupFacade setupFacade;
        private readonly TestWordPairFacade testWordPairFacade;
        private readonly ILogger<DownloadPersonalDataModel> _logger;

        public DownloadPersonalDataModel(
            UserManager<ApplicationUser> userManager,
            SetupFacade setupFacade,
            TestWordPairFacade testWordPairFacade,
            ILogger<DownloadPersonalDataModel> logger)
        {
            _userManager = userManager;
            this.setupFacade = setupFacade;
            this.testWordPairFacade = testWordPairFacade;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            _logger.LogInformation("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User));

            // Only include personal data for download
            Dictionary<string, object> personalData = new();
            IEnumerable<System.Reflection.PropertyInfo> personalDataProps = typeof(ApplicationUser).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (System.Reflection.PropertyInfo p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            BL.DTO.UserProfileDto profile = await setupFacade.GetUserProfileAsync();

            personalData.Add($"Linked Languages Profile", profile);

            IList<BL.DTO.WordPairDto> learnedWordPairs = await testWordPairFacade.GetLearnedWordPairs();
            personalData.Add($"Learned Word Pairs", learnedWordPairs);

            Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
            return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");
        }
    }
}
