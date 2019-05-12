using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace User.Api.Model
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
    }
}
