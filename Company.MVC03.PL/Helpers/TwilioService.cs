using Company.MVC.DAL.SMS;
using Company.MVC.PL.Settings;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Company.MVC.PL.Helpers
{
    public class TwilioService(IOptions<TwilioSettings> _options) : ITwilioService
    {
        public MessageResource SendSMS(SMS sms)
        { 
            TwilioClient.Init(_options.Value.AccountSID, _options.Value.AuthToken);

            var message = MessageResource.Create(
                body : sms.Body,
                to : sms.To,
                from : new Twilio.Types.PhoneNumber (_options.Value.PhoneNumber)
               
            );

            return message;

        }
    }
}
