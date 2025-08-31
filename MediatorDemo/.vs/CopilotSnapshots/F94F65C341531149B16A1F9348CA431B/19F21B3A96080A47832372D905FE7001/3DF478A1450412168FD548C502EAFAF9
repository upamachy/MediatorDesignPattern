using DemoLibrary.Commands;
using DemoLibrary.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLibrary.DataAccess
{
    public class MongoDataAccess : IMongoDataAccess
    {
        private const string PersonCollectionName = "Person";
        private readonly IMongoCollection<PersonModel> _personCollection;

        public MongoDataAccess(IMongoDatabase database)
        {
            _personCollection=database.GetCollection<PersonModel>(PersonCollectionName);
        }

        public async Task<List<PersonModel>> GetPeopleAsync()
        {
            var result = await _personCollection.FindAsync(_ => true);
            return result.ToList();
        }

        public async Task<PersonModel> GetPersonByIdAsync(string id)
        {
            var result=await _personCollection.FindAsync(x => x.Id == id);
            return result.FirstOrDefault();
        }

        public async Task<PersonModel> InsertPersonAsync(string firstName, string lastName)
        {
            var person = new PersonModel
            {
                FirstName = firstName,
                LastName = lastName
            };
            await _personCollection.InsertOneAsync(person);
            return person;
        }

        public async Task<PersonModel> UpdatePersonAsync(string id, UpdatePersonCommand command)
        {
            var update = Builders<PersonModel>.Update
                .Set(p => p.FirstName, command.FirstName)
                .Set(p => p.LastName, command.LastName);
            
            var options = new FindOneAndUpdateOptions<PersonModel>
            {
                ReturnDocument = ReturnDocument.After
            };
            
            var result = await _personCollection.FindOneAndUpdateAsync(
                p => p.Id == id,
                update,
                options);
            
            return result;
        }

        public async Task<PersonModel> DeletePersonAsync(string id)
        {
            var result = await _personCollection.FindOneAndDeleteAsync(p => p.Id == id);
            
            return result;
        }
    }
}
