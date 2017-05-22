using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sprotify.API.Models
{
    public class PlaylistWithSongs : Playlist
    {     
        public ICollection<Song> Songs { get; set; }
            = new List<Song>();
    }
}
