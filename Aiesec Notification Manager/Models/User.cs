using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiesec_Notification_Manager.Models
{
    class User
    {
        public int id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TgChatId { get; set; }
        public string VkUserId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Depatment { get; set; }
    }
}