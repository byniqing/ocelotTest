using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.Identity.Services;

namespace User.Identity.Impl
{
    public class TestAuthCode : IAuthCodeService
    {
        public bool Validate(string phone, string authCode)
        {
            return true;
        }
    }
}
