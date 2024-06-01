using CodePulse.API.Data;
using CodePulse.API.Models.Domain;
using CodePulse.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.API.Repositories.Implementation
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly ApplicationDbContext dbContext;

        public BlogPostRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<BlogPost> CreateAsync(BlogPost blogPost)
        {
            await dbContext.BlogPosts.AddAsync(blogPost);

            await dbContext.SaveChangesAsync();

            return blogPost;

        }

        public async  Task<BlogPost?> DeleteAsync(Guid id)
        {
            var existingBlogPost = await dbContext.BlogPosts.FirstOrDefaultAsync(x=>x.Id == id);

            if (existingBlogPost != null)
            {
                dbContext.BlogPosts.Remove(existingBlogPost);
                await dbContext.SaveChangesAsync();

                return existingBlogPost;
            }

            return null;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync(string? query = null,
            string? shortBy = null,
            string? shortDirection = null,
            int? pageNumber = 1,
            int? pageSize = 100)
        {
            var blogPosts = dbContext.BlogPosts.AsQueryable();

            //fillterbing
            if (string.IsNullOrWhiteSpace(query) == false)
            {
                blogPosts = blogPosts.Where(x => x.Title.Contains(query));
            }

            //shorting
            if (string.IsNullOrWhiteSpace(shortBy) == false)
            {
                if (string.Equals(shortBy, "title", StringComparison.OrdinalIgnoreCase))
                {
                    var isAsc = string.Equals(shortDirection, "asc", StringComparison.OrdinalIgnoreCase) ? true : false;

                    blogPosts = isAsc ? blogPosts.OrderBy(x => x.Title) : blogPosts.OrderByDescending(x => x.Title);
                }


            }

            //pagination
            var skipResults = (pageNumber - 1) * pageSize;
            blogPosts = blogPosts.Skip(skipResults ?? 0).Take(pageSize ?? 100);
            return await dbContext.BlogPosts.Include(x=>x.Categories).ToListAsync();
        }

        public  async Task<BlogPost?> GetByIdAsync(Guid id)
        {
            return await dbContext.BlogPosts.Include(x=> x.Categories).FirstOrDefaultAsync(x=>x.Id ==id);
        }

        public async Task<BlogPost?> GetByUrlHandleAsync(string urlHandle)
        {
            return await dbContext.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);
        }

        public async Task<int> GetCount()
        {
            return await dbContext.BlogPosts.CountAsync();
        }

        public async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
        {
         var existingBlogPost =  await dbContext.BlogPosts.Include(x=>x.Categories).FirstOrDefaultAsync(X=>X.Id == blogPost.Id);

            if (existingBlogPost == null) 
            {
                return null;
            }

            dbContext.Entry(existingBlogPost).CurrentValues.SetValues(blogPost);

            existingBlogPost.Categories=blogPost.Categories;

            await dbContext.SaveChangesAsync();

            return blogPost;
        }
    }
}
