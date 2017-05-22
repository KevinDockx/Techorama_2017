using Microsoft.EntityFrameworkCore;
using Sprotify.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sprotify.API.Services
{
    public class SprotifyRepository : ISprotifyRepository, IDisposable
    {
        private SprotifyContext _context;
        public SprotifyRepository(SprotifyContext context)
        {
            _context = context;
        }

        public void AddSongToPlaylist(Guid playlistId, Song song)
        {
            var playlist = GetPlaylist(playlistId);
            // create id (could be handled by DB)
            song.Id = Guid.NewGuid();
            playlist.Songs.Add(song);
        }

        public void DeleteSong(Song song)
        {
            _context.Songs.Remove(song);
        }

        public Playlist GetPlaylist(Guid playlistId, bool includeSongs = false)
        {
            if (includeSongs)
            {
                return _context.Playlists.Include(p => p.Songs)
                    .Where(p => p.Id == playlistId).FirstOrDefault();
            }

            return _context.Playlists.Where(c => c.Id == playlistId).FirstOrDefault();
        }
        
        public IEnumerable<Playlist> GetPlaylists()
        {
            return _context.Playlists.ToList();
        }

        public double GetAveragePlaylistRating(Guid playlistId)
        {
            var ratingsForPlaylist = _context.PlaylistRatings.Where(p => p.PlaylistId == playlistId);

            if (ratingsForPlaylist.Any())
            {
                return ratingsForPlaylist.Average(r => r.Rating);
            }

            return 0;
        }

        public Song GetSongFromPlaylist(Guid playlistId, Guid songId)
        {
            return _context.Songs.Where(s => s.PlaylistId == playlistId 
                         && s.Id == songId).FirstOrDefault();
        }

        public IEnumerable<Song> GetSongsFromPlaylist(Guid playlistId)
        {
           return _context.Songs.Where(s => s.PlaylistId == playlistId).ToList();
        }

        public bool PlaylistExists(Guid playlistId)
        {
            return _context.Playlists.Any(p => p.Id == playlistId);
        }

        public void DeletePlaylist(Playlist playlist)
        {
            _context.Playlists.Remove(playlist);
        }

        public void UpdateSong(Song song)
        {
            // no code in this implementation
        }

        public void RatePlaylist(Guid id, int rating)
        {
            // always add ratings for one user.  If auth is implemented,
            // the userid should come from the token's sub value
            _context.PlaylistRatings.Add(
                new PlaylistRating()
                {
                    Id = Guid.NewGuid(),
                    PlaylistId = id,
                    Rating = rating,
                    UserId = new Guid("24ec92ba-e6a3-421f-a599-2a0bf88c807a")
                });
        }

        public IEnumerable<Song> GetSongs(IEnumerable<Guid> ids)
        {
            return _context.Songs.Where(s => ids.Contains(s.Id)).ToList();
        }

        public int GetSongCount(Guid playlistId)
        {
            return _context.Songs.Where(s => s.PlaylistId == playlistId).Count();
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }       
    }
}
