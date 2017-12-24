using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLibrary
{
    public class Artist
    {
        private int id;
        private string name;
        private Country country;
        private int startingYear;
        private List<Album> albums;
        
        public List<Album> Albums
        {
        get
            {
                return albums;
            }
        }

        #region Properties

        public int ID
        {
            get; set;
        }
        #endregion
    }
}
