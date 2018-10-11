using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class UserViewModel
    {
        public UserViewModel() { }

        public UserViewModel(ApplicationUser user)
        {
            ID = user.Id;
            UserName = user.UserName;
            NickName = user.NickName;
            Avatar = user.Avatar;
        }

        public string ID { get; set; }

        public string UserName { get; set; }

        public string NickName { get; set; }

        public string Avatar { get; set; }
        
    }
}