using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;
using Parus.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parus.Infrastructure.DLA.Repositories
{
    public class DapperUserRepository : IDisposable, IUserRepository
    {
        private readonly SqlConnection db;

        public DapperUserRepository(string connectionString)
        {
            db = new SqlConnection(connectionString);
        }

        public IEnumerable<Core.Entities.IUser> Users { get; }

        public bool IsEmailTaken(string email)
        {
            throw new NotImplementedException();
        }

        public bool IsUsernameTaken(string nickname)
        {
            throw new NotImplementedException();
        }

        public void ClearTracking()
        {
            throw new NotImplementedException();
        }

        public bool Contains(Func<Core.Entities.IUser, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (db != null)
            {
                db.Dispose();
            }
        }

        public Core.Entities.IUser FindUserByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public Core.Entities.IUser FindUserByUsername(string nickname)
        {
            throw new NotImplementedException();
        }

        public int GetUserRegionId(string userId)
        {
            string q = "";

            q = $"select u.CountryId from [Identity].[AspNetUsers] u " +
                    $"where u.[Id] = '{userId}'";
            //Console.WriteLine(q);
            var ret1 = db.Query<Country>(q);

            var e = ret1.GetEnumerator();
            if (e.MoveNext())
            {
                return e.Current.CountryId;
            }

            return default;
        }

        public Core.Entities.IUser One(Func<Core.Entities.IUser, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public void RemoveOne(string username)
        {
            throw new NotImplementedException();
        }

        public int SaveChanges()
        {
            throw new NotImplementedException();
        }

        public void Update(Action value)
        {
            throw new NotImplementedException();
        }

        public bool Update(Core.Entities.IUser user)
        {
            throw new NotImplementedException();
        }

        public void UpdateWithoutContextSave(Core.Entities.IUser user)
        {
            throw new NotImplementedException();
        }
    }

    public class DapperSubscriptionSessionsRepository : IDisposable, ISubscribeSessionsRepository
    {
        private readonly SqlConnection db;

        public DapperSubscriptionSessionsRepository(string connectionString)
        {
            db = new SqlConnection(connectionString);
        }

        public Task AddSessionAsync(SubscriptionSession s)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (db != null)
            {
                db.Dispose();
            }
        }

        public IEnumerable<SubscriptionSession> GetExpiringSoon(DateTime expiringDate, int clockslew)
        {
            throw new NotImplementedException();
        }

        public SubscriptionSession OneById(string purchaserUserId, bool includeProfile = true)
        {
            string q = "";

            if (includeProfile)
            {
                q = $"select * from [Billing].[SubscriptionSessions] s " +
                    $"join [Billing].[SubscriptionProfiles] p on s.[ProfileId] = p.[ProfileId] " +
                    $"where s.[PurchaserUserId] = '{purchaserUserId}'";
                //Console.WriteLine(q);
                var ret1 =  db.Query<SubscriptionSession, SubscriptionProfile, SubscriptionSession>(q, (session, profile) => {
                        session.Profile = profile;
                        return session;
                    }, splitOn: "ProfileId");

                var e = ret1.GetEnumerator();
                if (e.MoveNext())
                {
                    return e.Current;
                }

                return null;
            }

            var ret = db.Query<SubscriptionSession>(q);
            return default;
        }

        public SubscriptionSession OneByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class SubscribeSessionsRepository : ISubscribeSessionsRepository
    {
        private readonly BillingDbContext _context;

        public SubscribeSessionsRepository(BillingDbContext context)
        {
            _context = context;
        }

        public Task AddSessionAsync(SubscriptionSession s)
        {
            return Task.CompletedTask;
        }

        public IEnumerable<SubscriptionSession> GetExpiringSoon(DateTime currentDate, int clockslew)
        {
            return _context.SubscriptionSessions
                .Where(where)
                .ToList();

            bool where(SubscriptionSession session)
            {
                if (session.ExpiresAt > currentDate && currentDate.Minute > (session.ExpiresAt.Minute - clockslew))
                {
                    Console.WriteLine($"{session.BroadcasterId} true");
                    return true;
                }
                else if (currentDate > session.ExpiresAt)
                {
                    Console.WriteLine($"{session.BroadcasterId} true");
                    return true;
                }

                Console.WriteLine($"{session.BroadcasterId} false");
                return false;
            }
        }

        public SubscriptionSession OneByUserId(string userId)
        {
            return _context.SubscriptionSessions.FirstOrDefault(x => x.PurchaserUserId == userId);
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
