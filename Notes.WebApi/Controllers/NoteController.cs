using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.Application.Notes.Command.CreateNote;
using Notes.Application.Notes.Commands.DeleteNote;
using Notes.Application.Notes.Commands.UpdateNote;
using Notes.Application.Notes.Queries.GetNoteDetails;
using Notes.Application.Notes.Queries.GetNoteList;
using Notes.WebApi.Models;

namespace Notes.WebApi.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Produces("application/json")]
    [Route("api/{version:apiVersion}/[controller]")]
    public class NoteController : BaseControler
    {
        private readonly IMapper _mapper;
        public NoteController(IMapper mapper)
        {
            _mapper = mapper;
        }


        /// <summary>
        /// Get all notes
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// Get /note
        /// </remarks>
        /// <returns>Returns NoteListVm</returns>
        /// <response code="200">Returns NoteListVm</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<NoteListVm>> GetAll()
        {
            var query = new GetNoteListQuery
            {
                UserId = UserId
            };
            var vm = await Mediator.Send(query);
            return Ok(vm);
        }


        /// <summary>
        /// Get note by id
        /// </summary>
        /// <remarks>
        /// Sample remarks:
        /// Get /note/7284CE2D-F296-4E01-9519-381220B5B030
        /// </remarks>
        /// <param name="id">Note id (guid)</param>
        /// <returns>Returns NoteDetailsVm</returns>
        /// <response code="200">Success</response>
        /// <response code="401">Unauthorized</response>

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<NoteDetailsVm>> Get(Guid id)
        {
            var query = new GetNoteDetailsQuery
            {
                UserId = UserId,
                Id = id
            };
            var vm = await Mediator.Send(query);
            return Ok(vm);
        }


        /// <summary>
        /// Create new note
        /// </summary>
        /// <remarks>
        ///Sample request:
        /// Post /note
        /// {
        ///   title: "Note title",
        ///   details: "Note details"
        /// }
        /// </remarks>
        /// <param name="createNoteDto"></param>
        /// <returns>Returns id (guid)</returns>
        /// <response code="201">Success</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateNoteDto createNoteDto)
        {
            var command = _mapper.Map<CreateNoteCommand>(createNoteDto);
            command.UserId = UserId;
            var noteId = await Mediator.Send(command);
            return Ok(noteId);
        }


        /// <summary>
        /// Update note
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// PUT /note
        /// {
        ///    title: "Note title",
        /// }
        /// </remarks>
        /// <param name="updateNoteDto"></param>
        /// <returns>Returns NoContent</returns>
        /// <response code="204">Success</response>
        /// <response code="401">Unauthorized</response>

        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update([FromBody] UpdateNoteDto updateNoteDto)
        {
            var command = _mapper.Map<UpdateNoteCommand>(updateNoteDto);
            command.UserId = UserId;
            await Mediator.Send(command);
            return NoContent();
        }


        /// <summary>
        /// Delete note 
        /// </summary>
        /// <remarks>
        ///Sample request:
        /// DELETE /note/7284CE2D-F296-4E01-9519-381220B5B030
        /// </remarks>
        /// <param name="id">Id of the note (guid)</param>
        /// <returns>Returns NoContent</returns>
        /// <response code="204">Success</response>
        /// <response code="401">Unauthorized</response>

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteNoteCommand
            {
                Id = id,
                UserId = UserId
            };
            await Mediator.Send(command);
            return NoContent();
        }

    }
}
