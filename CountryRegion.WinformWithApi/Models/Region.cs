using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountryRegion.WinformWithApi.Models
{
    public class Region
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public long CountryId { get; set; }
        public virtual Country Country { get; set; }
    }
}
