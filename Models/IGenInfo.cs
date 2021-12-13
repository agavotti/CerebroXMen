using System;

namespace CerebroXMenAPI.Models
{
    public class IGenInfo
    {
        public int ID { get; set; }
        public string[] Dna { get; set; }
        public bool EsMutante { get; set; }
        public DateTime FechaAlta { get; set; }

    }
}
