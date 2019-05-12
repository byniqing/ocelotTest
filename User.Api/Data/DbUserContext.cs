using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.Api.Model;

namespace User.Api.Data
{
    public class DbUserContext : DbContext
    {
        public DbUserContext(DbContextOptions<DbUserContext> options) : base(options)
        { }

        /// <summary>
        /// 默认当前类名就是表名
        /// 如果不想，需要重新该方法，自定义表的名字
        /// 该方法是：在创建model的时候触发
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //修改默认的AppUser表名，改为user
            modelBuilder.Entity<AppUser>()
                .ToTable("Users") //表名
                .HasKey(u => u.Id); //主键
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<AppUser> User { get; set; }
    }
}
