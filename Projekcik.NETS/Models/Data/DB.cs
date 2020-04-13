﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Projekcik.NETS.Models.Data
{
    public class Db :DbContext
    {
        public DbSet<PageDTO> Pages { get; set; }
        public DbSet<SidebarDTO> Sidebar { get; set; }
        public DbSet<CategoryDTO> Categories { get; set; }
        public DbSet<ProductDTO> Products { get; set; }
        public DbSet<UserDTO> User { get; set; }
        public DbSet<RoleDTO> Role { get; set; }
    }
}