using System.Collections.Generic;
using Naturistic.Core.Entities;

namespace Naturistic.Core.Interfaces.Repositories
{
    public interface IViewerUsersRepository
    {
        IEnumerable<ViewerUser> Channels { get; }

        bool Add(ViewerUser channel);
    }
}