using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Parus.Core.Billing.YooMoney
{
    public class YooMoneyClientResponse
    {

    }

    public class YooMoneyClient : HttpClient
    {
        public YooMoneyClient(string clientId, string redirectUri, string[] scope)
        {
            BaseAddress = new Uri("https://yoomoney.ru");
        }

        //public Task<YooMoneyClientResponse> Oath()
        //{

        //}
    }
}
