using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enharmonica.Models
{
    public class ScaleModel
    {
        public string RootNote { get; set; }
        public string ScaleType { get; set; }
        public List<int> Notes { get; set; }
        public int Instrument { get; set; }
        public ScaleModel(string rootNote, string scaleType, List<int> notes, int instrument)
        {
            RootNote = rootNote;
            ScaleType = scaleType;
            Notes = notes;
            Instrument = instrument;
        }
    }
}
