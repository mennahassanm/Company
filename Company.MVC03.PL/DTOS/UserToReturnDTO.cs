﻿namespace Company.MVC.PL.DTOS
{
    public class UserToReturnDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }

        public string Email { get; set; }
        public IEnumerable<string>? Roles { get; set; } 
    }
}
