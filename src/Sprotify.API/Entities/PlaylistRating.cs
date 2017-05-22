using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sprotify.API.Entities
{
    public class PlaylistRating
    {
        [Key]
        // avoids issues with EF Core composite keys
        public Guid Id { get; set; }

        [Required]
        public Guid PlaylistId { get; set; }

        [ForeignKey("PlaylistId")]
        public Playlist Playlist { get; set; }

        [Required]
        // users can rate other people's playlists
        public Guid UserId { get; set; }

        [Required]
        public int Rating { get; set; }
    }
}
