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
    public class InsertPersonHandler : IRequestHandler<InsertPersonCommand, PersonModel>
    {
        public readonly IMongoDataAccess _mongoDataAccess;
        public InsertPersonHandler(IMongoDataAccess mongoDataAccess)
        {
            _mongoDataAccess = mongoDataAccess;
        }
        public async Task<PersonModel> Handle(InsertPersonCommand request, CancellationToken cancellationToken)
        {
            return await _mongoDataAccess.InsertPersonAsync(request.FirstName, request.LastName);
        }
    }
}
