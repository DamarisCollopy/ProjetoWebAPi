using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Domain.APIService
{
    public class BaseApi
    {
        public HttpClient Consumer { get; set; }

        public BaseApi()
        {
            Consumer = new HttpClient();
            Consumer.BaseAddress = new Uri("https://localhost:44307/");
        }
    }
}
