using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SDMXReg.Models
{
    public class SDMXContext : DbContext
    {
        public SDMXContext(DbContextOptions<SDMXContext> options)
            : base(options)
        {
        }

        public DbSet<Register> SDMXItems { get; set; }

    }
}
