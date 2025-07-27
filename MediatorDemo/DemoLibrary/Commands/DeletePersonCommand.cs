using DemoLibrary.Models;
using MediatR;

namespace DemoLibrary.Commands
{
    public class DeletePersonCommand : IRequest<PersonModel>
    {
        public string Id { get; }

        public DeletePersonCommand(string id)
        {
            Id = id;
        }
    }
}
