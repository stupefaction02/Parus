using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Naturistic.Core.Entities;
using Naturistic.Infrastructure.DLA;
using Naturistic.Infrastructure.Identity;

namespace Naturistic.Backend.Services
{
    public class BroadcastControl
    {
        public async Task StartBroadcastAsync(
            int category, int[] tags, string title,
            ApplicationUser user,
            ApplicationDbContext dbContext)
        {
            List<Tag> userTags = new List<Tag>();

            foreach (int tag in tags)
            {
                userTags.Add(dbContext.Tags.SingleOrDefault(x => x.Id == tag));
            }

            var broadcastInfo = new BroadcastInfo
            {
                HostUserId = user.GetId(),
                AvatarPic = user.AvatarPath,
                Category = dbContext.Categories.SingleOrDefault(x => x.Id == category),
                Tags = userTags,
                Username = user.UserName,
                Preview = "defaults/preview_bright.jpg",
                Ref = user.UserName,
                Title = title,
            };

            // Sending notifications

            // Updating players

            dbContext.Broadcasts.Add(broadcastInfo);

            await dbContext.SaveChangesAsync();
        }

        public async Task StopBroadcastAsync(ApplicationUser hostUser, Core.Interfaces.Repositories.IBroadcastInfoRepository context)
        {
            context.RemoveOne(hostUser.Id);
        }

        public void ChangePreviewImageAsync(string userName, string newImageFn, HttpClient httpClient)
        {

        }
    }
}