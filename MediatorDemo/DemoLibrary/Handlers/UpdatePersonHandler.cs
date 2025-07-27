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
    public class UpdatePersonHandler : IRequestHandler<UpdatePersonCommand, PersonModel>
    {
        public readonly IMongoDataAccess _mongoDataAccess;
        public UpdatePersonHandler(IMongoDataAccess mongoDataAccess)
        {
            _mongoDataAccess = mongoDataAccess;
        }
        
        public async Task<PersonModel> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
        {
            return await _mongoDataAccess.UpdatePersonAsync(request.Id, request);
        }
    }
}
