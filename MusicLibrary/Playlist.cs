using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLibrary
{
    public class Playlist
    {
        private int id;
        private string title;
        private string description;
        private bool shuffle;
        private bool repeat;
        private List<Track> tracks;

        public Track this[int trackID]
        {
            get
            {
                return tracks.FirstOrDefault(t => t.ID == id);
            }
        }
    }
}
