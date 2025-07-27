using DemoLibrary.Commands;
using DemoLibrary.DataAccess;
using DemoLibrary.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLibrary.Handlers
{
    public class DeletePersonHandler : IRequestHandler<DeletePersonCommand, PersonModel>
    {
        public readonly IMongoDataAccess _mongoDataAccess;
        public DeletePersonHandler(IMongoDataAccess mongoDataAccess)
        {
            _mongoDataAccess = mongoDataAccess;
        }
        
        public async Task<PersonModel> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
        {
            return await _mongoDataAccess.DeletePersonAsync(request.Id);
        }
    }
}