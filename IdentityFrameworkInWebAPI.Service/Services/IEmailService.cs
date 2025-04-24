using IdentityFrameworkInWebAPI.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityFrameworkInWebAPI.Service.Services
{
    public interface IEmailService
    {
       public void SendEmail(Message message);
    }
}
