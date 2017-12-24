using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLibrary
{
    public class Track
    {
        private int id;
        private string title;
        private Genre genre;
        private TimeSpan duration;

        #region Properties

        public int ID
        {
            get; set;
        }
        #endregion
    }
}
