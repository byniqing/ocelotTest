using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.Identity.Services;
using System.Net.Http;
using System.Net;
using Microsoft.Extensions.Options;
using DnsClient;
using User.Identity.Dtos;
using System.Linq;

namespace User.Identity.Impl
{
    public class UserService : IUserService
    {
        private HttpClient _httpClient;
        //private readonly string _userServiceUrl = "http://localhost:5000";
        private readonly string _userServiceUrl;
        private readonly IDnsQuery _dns;
        private readonly IOptions<ServiceDiscoveryOptions> _options;

        public UserService(HttpClient httpClient, IDnsQuery dns, IOptions<ServiceDiscoveryOptions> options)
        {
            _httpClient = httpClient;
            _dns = dns ?? throw new ArgumentNullException(nameof(dns));
            _options = options ?? throw new ArgumentNullException(nameof(options));

            var result = _dns.ResolveService("service.consul", _options.Value.UserServiceName);
            /*
             如果服务注册用的是localhost,那么AddressList为空，则取HostName
             必须是ip地址，比如127.0.0.1
             */
            var addressList = result.First().AddressList;
            //var address = result.First().AddressList.FirstOrDefault();
          
            var address = addressList.Any() ?
                addressList.FirstOrDefault().ToString() :
                result.First().HostName;
            var port = result.First().Port;

            _userServiceUrl = $"http://{address}:{port}";
        }
        public async Task<int> CheckOrCreate(string phone)
        {
            var result = await _dns.ResolveServiceAsync("service.consul", _options.Value.UserServiceName);
            var address = result.First().AddressList.FirstOrDefault();
            var port = result.First().Port;


            var form = new Dictionary<string, string> {
                { "phone",phone}
            };
            var content = new FormUrlEncodedContent(form);
            var response = await _httpClient.PostAsync(_userServiceUrl + "/api/user/check-or-create", content);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var userId = await response.Content.ReadAsStringAsync();
                int.TryParse(userId, out int intUserId);
                return intUserId;
            }

            return 0;
            //throw new NotImplementedException();
        }
    }
}
