using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;


namespace tech_software_engineer_consultant_int_backend.Services
{ 

    public class SmsService : ISmsService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromNumber;

        public SmsService(string accountSid, string authToken, string fromNumber)
        {
            _accountSid = accountSid;
            _authToken = authToken;
            _fromNumber = fromNumber;

            // Initialiser Twilio avec Account SID et Auth Token
            TwilioClient.Init(_accountSid, _authToken);
        }

        public async Task SendSmsAsync(string toNumber, string message)
        {
            var messageOptions = new CreateMessageOptions(
                new PhoneNumber(toNumber)) // Numéro du destinataire
            {
                From = new PhoneNumber(_fromNumber), // Numéro Twilio
                Body = message // Message à envoyer
            };

            var msg = await MessageResource.CreateAsync(messageOptions);

            Console.WriteLine($"Message envoyé avec SID : {msg.Sid}");
        }
    }

}
