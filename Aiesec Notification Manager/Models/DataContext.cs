using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Aiesec_Notification_Manager.Models
{
    class DataContext : DbContext
    {
        public DataContext() : base("DbConection") { }
        public DbSet<User> Users { get; set; }
    }
}
