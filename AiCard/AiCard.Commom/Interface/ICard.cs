using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCard.Common
{
    public interface ICard
    {
        int ID { get; set; }

        string UserID { get; set; }

        string Name { get; set; }


        string Avatar { get; set; }


        string PhoneNumber { get; set; }


        string Email { get; set; }

        string WeChatCode { get; set; }


        string Mobile { get; set; }


        bool Enable { get; set; }


        string Position { get; set; }


        Common.Enums.Gender Gender { get; set; }


        string Remark { get; set; }

        string Info { get; set; }

        string Voice { get; set; }

        string Video { get; set; }

        string Images { get; set; }

        string WeChatMiniQrCode { get; set; }

        string Poster { get; set; }

        string Industry { get; set; }

        int Like { get; set; }

        int View { get; set; }


        DateTime? Birthday { get; set; }

        int Sort { get; set; }
    }
}
