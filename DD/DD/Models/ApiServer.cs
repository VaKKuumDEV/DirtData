using System;
using System.Collections.Generic;
using System.Text;

namespace DD.Models
{
    public class ApiServer
    {
        public string Address { get; }
        public string Name { get; }

        public ApiServer(string address, string name)
        {
            Address = address;
            Name = name;
        }
    }
}
