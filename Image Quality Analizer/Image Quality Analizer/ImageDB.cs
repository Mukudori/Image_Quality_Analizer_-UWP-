using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Quality_Analizer
{
    public class ImagesTable
    {
        public int id { get; set; }
        public string name { get; set; }
        public string format { get; set; }
        public string pathInport { get; set; }
        public string pathExport { get; set; }
        public string pathLocal { get; set; }
        public double jq { get; set; }
        public double blur { get; set; }
        public bool accepted { get; set; }
        public bool exported { get; set; }

        public int historyId { get; set; }
        public HistoryTable HistoryTable { get; set; }

        public ImagesTable() { }

        public ImagesTable(ImagesTableView it)
        {
            id = it.id;
            name = it.name;
            format = it.format;
            pathInport = it.pathInport;
            pathExport = it.pathExport;
            pathLocal = it.pathLocal;
            jq = it.jq;
            blur = it.blur;
            accepted = it.accepted;
            exported = it.exported;
            historyId = it.historyId;
        }

        public override string ToString()
        {
            return name;
        }
    }


    public class HistoryTable
    {
        public int id { get; set; }
        public DateTime dateTime { get; set; }
        public double maxJQ { get; set; }
        public double minJQ { get; set; }
        public double maxBlur { get; set; }
        public double minBlur { get; set; }
        public bool copyToGalery { get; set; }

        public List<ImagesTable> images { get; set; }

        public override string ToString()
        {
            return dateTime.ToString();
        }

    }

    public class ImageContext : DbContext
    {
        public DbSet<ImagesTable> Images { get; set; }        
        public DbSet<HistoryTable> History { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Images.db");
        }
    }
}
