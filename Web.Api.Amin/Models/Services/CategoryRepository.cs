using Web.Api.Amin.Models.Contexts;
using Web.Api.Amin.Models.Entities;

namespace Web.Api.Amin.Models.Services
{
    public class CategoryRepository
    {
        private readonly DataBaseContext context;

        public CategoryRepository(DataBaseContext context)
        {
            this.context = context;
        }

        public List<CategoryDto> GetAll()
        {
           var data= context.Categories.ToList()
                .Select(p=>new CategoryDto
                {
                    Id= p.Id,   
                    Name= p.Name,   
                }).ToList();
            return data;

        }

        public CategoryDto Find(int Id)
        {
            var category =context.Categories.Find(Id);
            return new CategoryDto()
            {
                Id = category.Id,
                Name = category.Name,
            };
        }


        public int AddCategory(string Name)
        {
            Category category = new Category()
            {
                Name = Name
            };

             context.Add(category);
            return context.SaveChanges();
            return category.Id;
        }

        public int Delete(int Id)
        {
            context.Categories.Remove(new Category { Id= Id }); 
            return context.SaveChanges();   
        }

        public int Edit(CategoryDto categoryDto)
        {
            var category = context.Categories.SingleOrDefault(c => c.Id == categoryDto.Id);
            category.Name = categoryDto.Name;
            return context.SaveChanges();
        }

        public class CategoryDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
