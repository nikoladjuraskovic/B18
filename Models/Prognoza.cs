using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace B18.Models
{
    public class Prognoza
    {
        [Key]
        public int Id { get; set; }
        public string Mesto { get; set; }
        public string NazivMesta { get; set; }
        public int MinTemperatura { get; set; }
        public int MaxTemperatura { get; set; }
        public string Vreme { get; set; }
    }

    public class PrognozaDBContext : DbContext
    {
        public DbSet<Prognoza> Prognoze { get; set; }
    }
}