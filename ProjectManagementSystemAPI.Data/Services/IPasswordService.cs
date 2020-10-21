using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagementSystemAPI.Data.Services
{
    public interface IPasswordService
    {
        string ComputePasswordHash(string password);
    }
}
