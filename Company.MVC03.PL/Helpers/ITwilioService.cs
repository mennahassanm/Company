using Company.MVC.DAL.SMS;
using Twilio.Rest.Api.V2010.Account;

namespace Company.MVC.PL.Helpers
{
    public interface ITwilioService
    {
        public MessageResource SendSMS(SMS sms);
    }
}
