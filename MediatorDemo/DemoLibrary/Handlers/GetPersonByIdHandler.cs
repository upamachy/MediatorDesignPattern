using DemoLibrary.DataAccess;
using DemoLibrary.Models;
using DemoLibrary.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLibrary.Handlers
{
    public class GetPersonByIdHandler : IRequestHandler<GetPersonByIdQuery, PersonModel>
    { 
        public readonly IMongoDataAccess _mongoDataAccess;
        public GetPersonByIdHandler( IMongoDataAccess mongoDataAccess)
        {
            _mongoDataAccess = mongoDataAccess;
        }
        public async Task<PersonModel> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
        {
           return await _mongoDataAccess.GetPersonByIdAsync(request.Id);
        }
    }
}
