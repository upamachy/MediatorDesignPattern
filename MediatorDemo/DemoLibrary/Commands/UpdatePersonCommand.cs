using DemoLibrary.Models;
using MediatR;

namespace DemoLibrary.Commands
{
    public class UpdatePersonCommand : IRequest<PersonModel>
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
