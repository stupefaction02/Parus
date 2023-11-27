
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Parus.Backend.Controllers;
using Parus.Core;
using Parus.Core.Entities;
using Parus.Core.Interfaces;
using Parus.Core.Interfaces.Repositories;
using Parus.Core.Interfaces.Services;
using Parus.Infrastructure.DLA.Repositories;
using Parus.Infrastructure.Identity;
using Newtonsoft.Json.Linq;
using Xunit;
using System.Formats.Asn1;
using IdentityTest;

namespace Tests.Core
{
    public class LocalizationTests
    {
        public LocalizationTests()
        {
            Helper.Boot();
        }

        [Fact]
        public void SetLocale_GetOneWord1()
        {
            ILocalizationService localization =
                Helper.GetBackendService<ILocalizationService>();
            string locale = "ru";
            string key1 = "BTN_CONT";

            string exprectedWord = "Продолжить";

            localization.SetLocale(locale);

            string got = localization.RetrievePhrase(key1);

            Assert.Equal(got, exprectedWord);
        }

        [Fact]
        public void SetLocale_GetOneWord2()
        {
            ILocalizationService localization =
                Helper.GetBackendService<ILocalizationService>();
            string locale = "ru";
            string key1 = "EDITPSWRD_WEAKPASSWORD";

            string exprectedWord = "Пароль должен содержать более 6 символов";

            localization.SetLocale(locale);

            string got = localization.RetrievePhrase(key1);

            Assert.Equal(got, exprectedWord);
        }
    }
}