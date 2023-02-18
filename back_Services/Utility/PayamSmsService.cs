using Models.MessageClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace back_Services.Utility
{
    public class PayamSmsService
    {
        public static MessageClass Send(string name, string verificationCode, string toNumber)
        {
            var mc = new MessageClass();
            PanelSMS.smsserver client = new PanelSMS.smsserver();
            var username = "09116107086";
            var password = "faraz2670143725";
            var fromNum = "+983000505";
            string[] toNum = { toNumber };

            var patternCode = "orzzwwhmkq";


            var data = new PanelSMS.input_data_type[] {
                // key is your parameter name and value is what you want to send to the receiptor 
                new PanelSMS.input_data_type(){ key ="name",value = name } ,
                new PanelSMS.input_data_type(){ key ="verification-code",value = verificationCode }
            };

            var response = client.sendPatternSms(fromNum, toNum, username, password, patternCode, data);
            mc.Message = response;
            return mc;
        }
    }
}
