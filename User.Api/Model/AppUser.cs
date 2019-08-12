using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace User.Api.Model
{
    public class AppUser
    {
        /*
         其中DatabaseGeneratedOption的有三个属性 
        Identity：自增长 
        None：不处理 
        Computed：表示这一列是计算列。
             */
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //[Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string phone { get; set; }
    }
}
