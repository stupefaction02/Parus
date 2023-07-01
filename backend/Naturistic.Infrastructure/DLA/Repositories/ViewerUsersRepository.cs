using System;
using System.Collections.Generic;
using Naturistic.Core.Entities;
using Naturistic.Core.Interfaces.Repositories;

namespace Naturistic.Infrastructure.DLA.Repositories
{
    public class ViewerUsersRepository : IViewerUsersRepository
    {
        private readonly ApplicationDbContext context;

        public IEnumerable<ViewerUser> Channels => context.ViewerUsers; 

        public ViewerUsersRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Add(ViewerUser channel)
        {
            context.ViewerUsers.Add(channel);
            context.SaveChanges();
        }
    }
}