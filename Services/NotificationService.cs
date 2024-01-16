using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCSampleApp.Services
{
    public class NotificationService
    {
        private readonly List<string> _registeredParticipants = new List<string>();

        public void RegisterParticipant(string participantName)
        {
            _registeredParticipants.Add(participantName);
        }

        public void UnregisterParticipant(string participantName)
        {
            _registeredParticipants.Remove(participantName);
        }

        public void NotifyParticipants(string message)
        {
            foreach (var participant in _registeredParticipants)
            {               
                Console.WriteLine($"Notifying participant '{participant}': {message}");
            }
        }
    }

}
