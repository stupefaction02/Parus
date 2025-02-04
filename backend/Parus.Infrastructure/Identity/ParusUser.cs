﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Parus.Core.Entities;
using Parus.Core.Billing;
using System.Collections.Generic;

namespace Parus.Infrastructure.Identity
{
    public class ParusSubscriptionSession : SubscriptionSession
    {
        // null! - required
        // https://learn.microsoft.com/en-us/ef/core/modeling/relationships/one-to-many#optional-one-to-many

        public ParusUser PurchaserUser { get; set; } = null!;

        public Broadcaster Broacaster { get; set; } = null!;
    }


    public class Broadcaster
    {
        public int BroadcasterId { get; set; }

        public string OwnerId { get; set; }

        public ParusUser Owner { get; set; }

        public List<ParusSubscriptionSession> SubscriptionSessionsAsSubject { get; set; }
    }

    public class ParusUser : IdentityUser, IUser
    {
		#region Broadcast/Viewer fields

		[MaxLength(128)]
		public string AvatarPath { get; set; }

		#endregion

        public List<SubscriptionSession> SubscribeSessions { get; set; }

		public string ChatColor { get; set; }

        public string GetUsername() { return base.UserName; }

        public string GetEmail() { return base.Email; }

        public string GetId() { return base.Id; }

		public new bool EmailConfirmed { get => base.EmailConfirmed; set => base.EmailConfirmed = value; }

        public ConfirmCode ConfirmCode { get; set; }

        public TwoFactoryEmailVerificationCode TwoFAEmailVerificationCode { get; set; }

        public TwoFactoryCustomerKey CustomerKey { get; set; }

        public PasswordRecoveryToken PasswordRecoveryToken { get; set; }

        [NotMapped]
        public IVerificationCode ConfirmCodeCore { get => ConfirmCode; }

        public RefreshSession RefreshSession { get; set; }

        /// <summary>
        /// ref: <see cref="IndexingRule"/>
        /// </summary>
        public byte IndexingRule { get; set; }

        public Broadcaster Broadcaster { get; set; }

        public void SetIndexingRule(IndexingRule rule)
        {
            IndexingRule = (byte)rule;
        }

        public override string ToString()
        {
            return $"{this.UserName} {this.Email} {this.Id}";
        }

        public string GetAvatarPath()
        {
            return AvatarPath;
        }

        public bool GetTwoFactorEnabled()
        {
            return this.TwoFactorEnabled;
        }

        public byte GetIndexingStatus()
        {
            return IndexingRule;
        }
    }
}