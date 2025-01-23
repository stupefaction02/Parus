using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Parus.Common.Utils;
using Parus.Core.Identity;
using Parus.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parus.Infrastructure.Services
{
    public class RefreshTokensService
    {
        private readonly RefreshTokenOptions options;
        private readonly ParusDbContext database;

        public RefreshTokensService(IOptions<RefreshTokenOptions> options, ParusDbContext database)
        {
            this.options = options.Value;
            this.database = database;
        }

        public async Task<RefreshSession> AddUpdateSessionAsync(HttpContext httpContext, ParusUser user)
        {
            string fingerPrint = httpContext.Request.Headers.UserAgent;
            RefreshSession refreshSession = RefreshSession.CreateDefault(fingerPrint, user);

            await database.RefreshSessions.AddAsync(refreshSession);

            await database.SaveChangesAsync();

            return refreshSession;
        }

        public RefreshSession GenerateToke(string fingerPrint, ParusUser user)
        {
            return new RefreshSession
            {
                // TODO: Replace with something more efficeint
                Token = RefreshSession.GenerateToken(),
                Fingerprint = fingerPrint,
                ExpiresAt = DateTimeUtils.ToUnixTimeSeconds(DateTime.Now.Add(RefreshSession.LifeTime)),
                User = user
            };
        }
    }
}
