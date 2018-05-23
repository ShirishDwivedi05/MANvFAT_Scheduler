using MANvFAT_Football.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MANvFAT_Football.Models.Repositories
{
    public class MessagesRepository
    {
        public void BuildMessageList(string Msg, TypeOfMessage typeOfMsg, ref List<MessagesExt> ListOfMsgs)
        {
            MessagesExt m = new MessagesExt()
            {
                Message = Msg,
                TypeOfMsg = typeOfMsg
            };

            //switch (typeOfMsg)
            //{
            //    case TypeOfMessage.Information:
            //        m.Css = "text-info";
            //        break;
            //    case TypeOfMessage.Success:
            //        m.Css = "text-success";
            //        break;
            //    case TypeOfMessage.Warning:
            //        m.Css = "text-warning";
            //        break;
            //    case TypeOfMessage.Error:
            //        m.Css = "text-danger";
            //        break;
            //    default:
            //        break;
            //}

            if (ListOfMsgs.Count == 0)
            {
                ListOfMsgs = new List<MessagesExt>();
            }

            ListOfMsgs.Add(m);
        }
    }

    public class MessagesExt
    {
        public string Message { get; set; }
        //  public string Css { get; set; }
        public TypeOfMessage TypeOfMsg { get; set; }
    }
}