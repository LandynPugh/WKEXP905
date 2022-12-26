using DataAccessClient.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessClient.TestData
{
    internal class TestUser : User
    {

        public static User GenerateUser()
        {
            string[] facultyIds = Global.facultyIds;
            User user = new User();
            Random random = new Random();
            user.UserId = new Guid();
            user.Address = "TestAddress";
            user.City = "TestCity";
            user.Email = "TestEmail";
            user.Phone = "1234567890";
            user.FacultyId = Guid.Parse(facultyIds[random.Next(0, facultyIds.Length)]);
            user.Name = "TestName";
            return user;
        }

        public static List<User> GenerateUserList(int numUsers)
        {
            List<User> list = new List<User>();
            for (int i = 0; i < numUsers; i++)
            {
                list.Add(GenerateUser());
            }
            return list;
        }
    }
}






