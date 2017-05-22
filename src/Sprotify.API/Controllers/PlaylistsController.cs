using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sprotify.API.Models;
using Sprotify.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sprotify.API.Controllers
{
    [Route("api/playlists")]
    public class PlaylistsController : Controller
    {
        private readonly ISprotifyRepository _sprotifyRepository;
        private readonly IUrlHelper _urlHelper;

        public PlaylistsController(ISprotifyRepository sprotifyRepository,
            IUrlHelper urlHelper)
        {
            _sprotifyRepository = sprotifyRepository;
            _urlHelper = urlHelper;
        }

        [HttpGet()]
        public IActionResult GetPlaylists()
        {
            return Ok(Mapper.Map<IEnumerable<Models.Playlist>>(
                _sprotifyRepository.GetPlaylists()));
        }

        [HttpGet("{id}", Name ="GetPlaylist")]
        [RequestHeaderMatchesMediaType("Accept",
            new[] { "application/vnd.marvin.hateoas+json" })]
        public IActionResult GetPlaylistWithLinks(Guid id)
        {
            var playListEntity = _sprotifyRepository.GetPlaylist(id);

            if (playListEntity == null)
            {
                return NotFound();
            }

            // return Ok(Mapper.Map<Models.Playlist>(playListEntity));

            // include links
            var mappedPlaylist = Mapper.Map<Models.Playlist>(playListEntity);
            return Ok(CreateLinks(mappedPlaylist));
        }

        [HttpGet("{id}", Name = "GetPlaylist")]
        [RequestHeaderMatchesMediaType("Accept",
         new[] { "application/json" })]
        public IActionResult GetPlaylist(Guid id)
        {
            var playListEntity = _sprotifyRepository.GetPlaylist(id);

            if (playListEntity == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<Models.Playlist>(playListEntity));
        }

        [HttpDelete("{id}", Name = "DeletePlaylist")]
        public IActionResult DeletePlaylist(Guid id)
        {
            var playListEntity = _sprotifyRepository.GetPlaylist(id);

            if (playListEntity == null)
            {
                return NotFound();
            }

            _sprotifyRepository.DeletePlaylist(playListEntity);

            if (!_sprotifyRepository.Save())
            {
                throw new Exception($"Deleting playlist {id} failed.");
            }

            return NoContent();
        }

        [HttpGet("{id}/averagerating")]
        public IActionResult GetAverageRating(Guid id)
        {
            if (!_sprotifyRepository.PlaylistExists(id))
            {
                return NotFound();
            }

            var averageRating = _sprotifyRepository.GetAveragePlaylistRating(id);

            return Ok(new { AverageRating = averageRating });
        }

        [HttpPost("{id}/rate", Name ="RatePlaylist")]
        public IActionResult Rate(Guid id, [FromBody] PlaylistRatingForAdd rating)
        {
            if (rating == null)
            {
                return BadRequest();
            }

            _sprotifyRepository.RatePlaylist(id, rating.Rating);

            if (!_sprotifyRepository.Save())
            {
                throw new Exception($"Adding a a rating for playlist {id} failed.");
            }

            // no content - no resource was created. 
            return NoContent();
        }

        private Playlist CreateLinks(Playlist playlist)
        {
            playlist.Links.Add(new Link(_urlHelper.Link("GetPlaylist",
                new { id = playlist.Id }),
                "self",
                "GET"));

            playlist.Links.Add(
                new Link(_urlHelper.Link("DeletePlaylist",
                new { id = playlist.Id }),
                "delete_playlist",
                "DELETE"));

            playlist.Links.Add(
                new Link(_urlHelper.Link("GetSongsFromPlaylist", 
                new { playlistId = playlist.Id }),
                "songs",
                "GET"));

            playlist.Links.Add(
                new Link(_urlHelper.Link("AddSongToPlaylist", 
                new { playlistId = playlist.Id }),
                "add_song_to_playlist",
                "POST"));

            // rule: if a playlist has more than 3 songs, we can rate it.
            if (_sprotifyRepository.GetSongCount(playlist.Id) > 3)
            {
                playlist.Links.Add(
                   new Link(_urlHelper.Link("RatePlaylist",
                   new { id = playlist.Id }),
                   "rate_playlist",
                   "POST"));
            }        

            return playlist;
        }

    }
}
