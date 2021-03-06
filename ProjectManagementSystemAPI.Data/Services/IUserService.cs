﻿using ProjectManagementSystemAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManagementSystemAPI.Data.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAll(string role);
        Task<IEnumerable<User>> GetUserWithRole(string role);
        User GetById(int id);
        Task<User> GetLogged(int id);

        
        User Login(string username, string password);
        bool DoesUserExist(string username);
        Task<User> Create(User user);
        Task<bool> Delete(int id);
        Task<bool> Update(int id, User user);
    }
}
