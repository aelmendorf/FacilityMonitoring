using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityMonitoring.Infrastructure.Services {
    public interface IEmailService {
        Task SendMessageAsync(string msg);
        void SendMessage(string msg);
    }
}
