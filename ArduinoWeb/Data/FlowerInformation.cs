using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArduinoWeb.Data
{
    [System.ComponentModel.DataAnnotations.Display(Name = "FlowerInfo")]
    public class FlowerInfo
    {
        [Key]
        public int Id { get; set; }
        public double Humidity { get; set; }
        public DateTime RecoredTime { get; set; }

    }
}
