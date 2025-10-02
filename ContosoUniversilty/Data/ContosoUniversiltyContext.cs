using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ContosoUniversilty.Models;

namespace ContosoUniversilty.Data
{
    public class ContosoUniversiltyContext : DbContext
    {
        public ContosoUniversiltyContext (DbContextOptions<ContosoUniversiltyContext> options)
            : base(options)
        {
        }

        public DbSet<ContosoUniversilty.Models.Student> Student { get; set; } = default!;
    }
}
