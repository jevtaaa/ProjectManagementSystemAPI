using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectManagementSystemAPI.Data;
using ProjectManagementSystemAPI.Data.Helpers;
using ProjectManagementSystemAPI.Data.Models;
using ProjectManagementSystemAPI.Data.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystemAPI.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationContext _context;
        private readonly AppSettings _appSettings;

        public UserService(ApplicationContext context, IOptions<AppSettings> options)
        {
            _context = context;
            _appSettings = options.Value;
        }

        // Login for all roles
        public User Login(string username, string password)
        {
            var user = _context.Users
                .SingleOrDefaultAsync(x => x.Username == username && x.Password == password);

            if (user.Result == null)
            {
                Debug.WriteLine("User doesnt authentificate.");
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Result.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Result.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Result.Token = tokenHandler.WriteToken(token);

            Debug.WriteLine("User successfully logged in.");
            return  user.Result.WithoutPassword();
        }

        // Creating users by admin
        public async Task<User> Create(User user)
        {
            try
            {
                EntityEntry<User> createdResult = await _context.Set<User>().AddAsync(user);
                await _context.SaveChangesAsync();
                Debug.WriteLine("User successfully created.");

                return createdResult.Entity.WithoutPassword();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: " + e.Message);
                return null;
            }
        }

        // Deleting users by admin
        public async Task<bool> Delete(int id)
        {
            try
            {
                var entity = await _context.Set<User>().FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return false;
                _context.Set<User>().Remove(entity);
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        // Updating user by admin
        public async Task<bool> Update(int id, User updatedUser)
        {
            var user = GetById(id);

            if (user == null)
            {
                return false;
            }

            UpdateUser(ref user, updatedUser);

            try
            {
                 _context.Update(user);
                 await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }

        //  Help for register user
        public bool DoesUserExist(string username)
        {
            var user = _context.Users.SingleOrDefaultAsync(x => x.Username == username);

            if (user.Result == null)
                return false;

            return true;
        }

        // Return all users by admin
        public async Task<IEnumerable<User>> GetAll(string role)
        {
            var users = await _context.Users.ToListAsync();
            if (role == Roles.ProjectManager)
            {
                
                users = await _context.Users
                                         .Where(u => u.Role == "Developer")
                                         .ToListAsync();

                /*foreach(User u in users)
                {
                    int devTasks =  _context.Tasks.Count(task => task.Deadline > DateTime.Now && task.Developer == u);
                    if(devTasks >= 3)
                    {
                        users.Remove(u);
                    }
                }*/
            }   
    
            return users.WithoutPasswords();
        }

        // Return a users with specific role
        public async Task<IEnumerable<User>> GetUserWithRole(string role)
        {
            var users =  await _context.Users.Where(u => u.Role == role).ToListAsync();

            if (users == null)
            {
                Debug.WriteLine("No accounts with role: " + role);
                return null;
            }

            return users.WithoutPasswords();
        }

        // Help for update users
        public  User GetById(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return null;
            }
            return user;
        }

        private void UpdateUser(ref User userForUpdate, User updatedUser)
        {
            userForUpdate.Name = updatedUser.Name;
            userForUpdate.Surname = updatedUser.Surname;
            userForUpdate.Email = updatedUser.Email;
            userForUpdate.Role = updatedUser.Role;
        }

        public async Task<User> GetLogged(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return null;
            }
            return user;
        }
    }
}
