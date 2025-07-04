﻿using DemoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DemoLibrary.DataAccess
{
    public class DemoDataAccess : IDataAccess
    {
        private List<Models.PersonModel> people = new();
        public DemoDataAccess()
        {
            people.Add(new Models.PersonModel { Id = 1, FirstName = "John", LastName = "Doe" });
            people.Add(new Models.PersonModel { Id = 2, FirstName = "Jane", LastName = "Smith" });
            people.Add(new Models.PersonModel { Id = 3, FirstName = "Jim", LastName = "Brown" });
        }

        public List<PersonModel> GetPeople()
        {
            return people;
        }

        public PersonModel InsertPerson(string firstName, string lastName)
        {
            PersonModel person = new()
            {
                FirstName = firstName,
                LastName = lastName
            };

            person.Id = people.Max(x => x.Id) + 1;
            people.Add(person);
            return person;
        }
    }
}
