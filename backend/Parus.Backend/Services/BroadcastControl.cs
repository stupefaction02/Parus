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
using Parus.Core.Entities;
using Parus.Infrastructure.DLA;
using Parus.Infrastructure.Identity;

namespace Parus.Backend.Services
{
    public class BroadcastControl
    {
        public async Task<int> StartBroadcastAsync(
            int category, int[] tags, string title,
            ParusUser user,
            ParusDbContext dbContext)
        {
            List<BroadcastTag> userTags = new List<BroadcastTag>();

            foreach (int tag in tags)
            {
                userTags.Add(dbContext.Tags.SingleOrDefault(x => x.BroadcastTagId == tag));
            }

            var broadcastInfo = new Broadcast
            {
                HostUserId = user.GetId(),
                AvatarPic = user.AvatarPath,
                Category = dbContext.Categories.SingleOrDefault(x => x.Id == category),
                Tags = userTags,
                Username = user.UserName,
                Preview = user.GetId().Replace("-", "") + ".png",
                Ref = user.UserName,
                Title = title,
            };

            // Sending notifications

            // Updating players

            await dbContext.Broadcasts.AddAsync(broadcastInfo);

            await dbContext.SaveChangesAsync();

            Console.WriteLine($"{user.UserName} has started a broadcast.");

            return 1;
        }

        public async Task StopBroadcastAsync(ParusUser hostUser, Core.Interfaces.Repositories.IBroadcastInfoRepository context)
        {
            context.RemoveOne(hostUser.Id);
        }

        public void ChangePreviewImageAsync(string userName, string newImageFn, HttpClient httpClient)
        {

        }
    }
}