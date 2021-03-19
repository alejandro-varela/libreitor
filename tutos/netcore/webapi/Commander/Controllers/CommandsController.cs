using System.Collections.Generic;
using Commander.Data;
using Commander.Models;
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

        // GET api/commands
        [HttpGet]
        public IEnumerable<Command> GetAllCommands()
        {
            var commandItems = _repository.GetAppCommands();
            return commandItems;
        }

        // GET api/commands/5
        [HttpGet("{id}")]
        public Command GetCommandById(int id)
        {
            var commandItem = _repository.GetCommandById(id);
            return commandItem;
        }

        // // GET api/commands
        // [HttpGet]
        // public ActionResult<IEnumerable<Command>> GetAllCommands()
        // {
        //     var commandItems = _repository.GetAppCommands();
        //     return Ok(commandItems);
        // }

        // // GET api/commands/5
        // [HttpGet("{id}")]
        // public ActionResult<Command> GetCommandById(int id)
        // {
        //     var commandItem = _repository.GetCommandById(id);
        //     return Ok(commandItem);
        // }
    }
}
