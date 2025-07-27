using DemoLibrary.Commands;
using DemoLibrary.Models;
using DemoLibrary.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DemoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    { 
        public readonly IMediator _mediator;
        public PersonController(IMediator mediator)
        {
            _mediator = mediator;
        }
        // GET: api/<PersonController>
        [HttpGet]
        public async Task<List<PersonModel>> Get()
        {
            return await _mediator.Send(new GetPersonListQuery());
        }

        // GET api/<PersonController>/5
        [HttpGet("{id}")]
        public async Task<PersonModel> Get(string id)
        {
            return await _mediator.Send(new GetPersonByIdQuery(id));
        }

        // POST api/<PersonController>
        [HttpPost]
        public async Task<PersonModel> Post([FromBody] InsertPersonCommand command)
        {
            return await _mediator.Send(command);
        }
        
        // PUT api/<PersonController>/5
        [HttpPut("{id}")]
        public async Task<PersonModel> Update(string id, [FromBody] UpdatePersonCommand command)
        {
            command.Id = id;
            return await _mediator.Send(command);
        }
        
        // DELETE api/<PersonController>/5
        [HttpDelete("{id}")]
        public async Task<PersonModel> Delete(string id)
        {
            return await _mediator.Send(new DeletePersonCommand(id));
        }
    }
}
