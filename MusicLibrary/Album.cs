using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLibrary
{
    public class Album
    {
        private int id;
        private string title;
        private int releaseYear;
        private int reReleased;
        private List<Track> tracks;

        public List<Track> Tracks
        {
        get
            {
                return tracks;
            }
        }
    }
}
