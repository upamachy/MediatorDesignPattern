using DemoLibrary.Commands;
using DemoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLibrary.DataAccess
{
    public interface IMongoDataAccess
    {
        Task<List<PersonModel>> GetPeopleAsync();
        Task<PersonModel> GetPersonByIdAsync(string id);
        Task<PersonModel> InsertPersonAsync(string firstName, string lastName);
        Task<PersonModel> UpdatePersonAsync(string id, UpdatePersonCommand command);
        Task<PersonModel> DeletePersonAsync(string id);
    }
}
