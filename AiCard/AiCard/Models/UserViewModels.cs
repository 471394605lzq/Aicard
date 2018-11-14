using AiCard.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class UserForApiViewModel
    {
        public UserForApiViewModel() { }

        public UserForApiViewModel(ApplicationUser user)
        {
            UserID = user.Id;
            UserName = user.UserName;
            NickName = user.NickName;
            Avatar = user.Avatar;
        }

        public string UserID { get; set; }

        public string UserName { get; set; }

        public string NickName { get; set; }

        public string Avatar { get; set; }
        
    }
}