using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enharmonica.Models
{
    public class ChordModel
    {
        public string RootNote { get; set; }
        public string ChordType { get; set; }
        public List<int> Notes { get; set; }
        public int Instrument {  get; set; }
        public ChordModel(string rootNote, string chordType, List<int> notes, int instrument)
        {
            RootNote = rootNote;
            ChordType = chordType;
            Notes = notes;
            Instrument = instrument;
        }
    }
}
