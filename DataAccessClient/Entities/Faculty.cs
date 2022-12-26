using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessClient.Entities
{
    internal class Faculty
    {
        public Guid FacultyId { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public Guid? CollegeId { get; set; }
    }
}
