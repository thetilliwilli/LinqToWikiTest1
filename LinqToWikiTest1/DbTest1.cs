using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;

namespace LinqToWikiTest1
{
    public class MyClass
    {
        public static void NotMain()
        {
            DataContext dataContext = new DataContext(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\TilliWilli\source\repos\LinqToWikiTest1\LinqToWikiTest1\Database1.mdf;Integrated Security=True")
            {
                Log = Console.Out,
            };

            //IEnumerable<Person> entities = new List<Person>()
            //{
            //    new Person(1, "First"),
            //    new Person(2, "First"),
            //    new Person(3, "First")
            //};

            //dataContext.Data

            //insert if none
            //if (dataContext.GetTable<Person>().Count() == 0)
            //{
            //    dataContext.GetTable<Person>().InsertAllOnSubmit(entities);
            //    dataContext.SubmitChanges();
            //}
            //Console.WriteLine(dataContext.GetTable<Person>().Count());

            var persons = dataContext.GetTable<Person>()
                //.Where(p => p.Sername != null)
                .AsEnumerable()
                .Where(p=>p.Name.Split(' ').Count() == 1)
                ;

            foreach (var person in persons)
                Console.WriteLine(person);

            Console.ReadKey();
        }
    }

    [Table(Name = "Table")]
    public class Person
    {
        public Person()
        {

        }

        public Person(int key, string name)
        {
            Key = key;
            Name = name;
        }

        [Column(IsPrimaryKey = true)]
        public int Key { get; set; }

        [Column]
        public string Name { get; set; }

        [Column]
        public string Sername { get; set; }

        public override string ToString()
        {
            return $"{Key} {Name} {Sername??"Unknown"}";
        }
    }
}