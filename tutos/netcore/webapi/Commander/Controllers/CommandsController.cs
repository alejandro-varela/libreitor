using System.Collections.Generic;
using Commander.Data;
using Commander.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Commander.Controllers
{
    [Route("api/commands")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommanderRepo _repository = null;

        public CommandsController(ICommanderRepo repository)
        {
            _repository = repository;
        }

        // // GET api/commands
        // [HttpGet]
        // public IEnumerable<Command> GetAllCommands()
        // {
        //     var commandItems = _repository.GetAppCommands();
        //     return commandItems;
        // }

        // // GET api/commands/5
        // [HttpGet("{id}")]
        // public Command GetCommandById(int id)
        // {
        //     var commandItem = _repository.GetCommandById(id);
        //     return commandItem;
        // }

        // GET api/commands
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Command>> GetAllCommands()
        {
            var commandItems = _repository.GetAppCommands();
            return Ok(commandItems);
        }

        // GET api/commands/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Command> GetCommandById([FromRoute] int id)
        {
            try 
            {
                var commandItem = _repository.GetCommandById(id);

                if (commandItem == null)
                {
                    return NotFound();
                }

                return Ok(commandItem);
            }
            catch 
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
