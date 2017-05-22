using AutoMapper;
using Library.API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Sprotify.API.Models;
using Sprotify.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sprotify.API.Controllers
{
    [Route("api/playlists/{playlistId}/songcollections")]
    public class SongCollectionsController : Controller
    {
        private readonly ISprotifyRepository _sprotifyRepository;

        public SongCollectionsController(ISprotifyRepository sprotifyRepository)
        {
            _sprotifyRepository = sprotifyRepository;
        }
        
        [HttpPost]
        public IActionResult AddSongCollectionToPlaylist(Guid playlistId,
            [FromBody] IEnumerable<SongForCreation> songCollection)
        {
            if (songCollection == null)
            {
                return BadRequest();
            }

            var songEntities = Mapper.Map<IEnumerable<Entities.Song>>(songCollection);

            foreach (var song in songEntities)
            {
                _sprotifyRepository.AddSongToPlaylist(playlistId, song);
            }

            if (!_sprotifyRepository.Save())
            {
                throw new Exception($"Adding a collection of songs to playlist {playlistId} failed on save.");
            }

            var songCollectionToReturn = Mapper.Map<IEnumerable<Models.Song>>(songEntities);
            var idsAsString = string.Join(",", songCollectionToReturn.Select(a => a.Id));

            return CreatedAtRoute("GetSongCollection",
                new { ids = idsAsString },
                songCollectionToReturn);           
        }
        
        // (key1,key2, ...)
        [HttpGet("({ids})", Name = "GetSongCollection")]
        public IActionResult GetSongCollection(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var songEntities = _sprotifyRepository.GetSongs(ids);

            if (ids.Count() != songEntities.Count()) 
            {
                return NotFound();
            } 

            return Ok(Mapper.Map<IEnumerable<Models.Song>>(songEntities));
        }
    }
}
