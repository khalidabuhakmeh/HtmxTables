using System.Collections.Generic;
using System.Linq;
using Htmx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HtmxTables.Pages
{
    public class Index : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public static List<Person> Database = new()
        {
            new(1,"Khalid", 38),
            new(2,"Nicole", 37),
            new(3,"Samson", 35),
            new(4,"Guinness", 21),
            new(5,"Bryn", 16)
        };

        public void OnGet()
        {
        }

        public IActionResult OnGetRow()
        {
            return Partial("_Row", Database.FirstOrDefault(p => p.Id == Id));
        }

        public IActionResult OnGetEdit()
        {
            return Partial("_Edit", Database.FirstOrDefault(p => p.Id == Id));
        }

        public IActionResult OnPostUpdate([FromForm] Person person)
        {
            if (Database.FirstOrDefault(x => x.Id == Id) is { } p)
            {
                p.Age = person.Age;
                p.Name = person.Name;

                return Request.IsHtmx()
                    ? Partial("_Row", p)
                    : Redirect("Index");
            }

            return BadRequest();
        }

        public List<Person> People => Database;
    }

    public class Person
    {
        public Person()
        {
        }

        public Person(int id, string name, int age)
        {
            Id = id;
            Name = name;
            Age = age;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}