using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Implementation;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogPostRepository blogPostRepository;
        private readonly ICategoryRepository categoryRepository;

        public BlogPostsController(IBlogPostRepository blogPostRepository, ICategoryRepository categoryRepository)
        {
            this.blogPostRepository = blogPostRepository;
            this.categoryRepository = categoryRepository;
        }

        [HttpPost]
        [Authorize(Roles ="Writer")]
        public async Task<IActionResult> CreateBlogPost([FromBody]CreateBlogPostRequestDto request)
        {
            //convert dto to domain
            var blogPost = new BlogPost 
            { 
                Title = request.Title, 
                Content = request.Content, 
                Author=request.Author,
                ShortDescription=request.ShortDescription,
                FeaturedImageUrl=request.FeaturedImageUrl,
                IsVisible=request.IsVisible,
                PublishedDate=request.PublishedDate,
                UrlHandle=request.UrlHandle,
                Categories=new List<Category>()
            };

            foreach (var categoryGuid in request.Categories) 
            {
                var existingCategory = await categoryRepository.GetById(categoryGuid);
                if (existingCategory is not  null) 
                {
                    blogPost.Categories.Add(existingCategory);
                }
            }

            blogPost = await blogPostRepository.CreateAsync(blogPost);

          

            //convert domain model back to dto

            var response = new BlogPostDto
            {
                Title = blogPost.Title,
                Content = blogPost.Content,
                Author = blogPost.Author,
                ShortDescription = blogPost.ShortDescription,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                IsVisible = blogPost.IsVisible,
                PublishedDate = blogPost.PublishedDate,
                UrlHandle = blogPost.UrlHandle,
                Categories = blogPost.Categories.Select(x => new CategoryDto {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle,
                }).ToList()

                
            };
            return Ok();

        }

        [HttpGet]
        public async Task<IActionResult> GetAllBlogPosts()
        {
           var blogPosts = await blogPostRepository.GetAllAsync();

           var response = new List<BlogPostDto>();
            foreach (var blogPost in blogPosts)
            {
                response.Add(new BlogPostDto
                {
                    Id = blogPost.Id,
                    Title = blogPost.Title,
                    Content = blogPost.Content,
                    Author = blogPost.Author,
                    ShortDescription = blogPost.ShortDescription,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    IsVisible = blogPost.IsVisible,
                    PublishedDate = blogPost.PublishedDate,
                    UrlHandle = blogPost.UrlHandle,
                    Categories = blogPost.Categories.Select(x => new CategoryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        UrlHandle = x.UrlHandle,
                    }).ToList()

                });
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetBlogPostById(Guid id)
        {
            var blogPost = await blogPostRepository.GetByIdAsync(id);

            if(blogPost is null)
            {
                return NotFound();
            }
            var response = new BlogPostDto
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Content = blogPost.Content,
                Author = blogPost.Author,
                ShortDescription = blogPost.ShortDescription,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                IsVisible = blogPost.IsVisible,
                PublishedDate = blogPost.PublishedDate,
                UrlHandle = blogPost.UrlHandle,
                Categories = blogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle,
                }).ToList()

            };
            return Ok(response);
           
        }


        //get: {apibaseurl} /api/blogposts/{urlhandle}
        [HttpGet]
        [Route("{urlHandle}")]
        public async Task<IActionResult> GetBlogPostByUrlHandle([FromRoute] string urlHandle)
        {
            var blogPost = await blogPostRepository.GetByUrlHandleAsync(urlHandle);

            if (blogPost is null)
            {
                return NotFound();
            }
            var response = new BlogPostDto
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Content = blogPost.Content,
                Author = blogPost.Author,
                ShortDescription = blogPost.ShortDescription,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                IsVisible = blogPost.IsVisible,
                PublishedDate = blogPost.PublishedDate,
                UrlHandle = blogPost.UrlHandle,
                Categories = blogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle,
                }).ToList()

            };
            return Ok(response);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> UpdateBlogPostById([FromRoute] Guid id,  UpdateBlogPostRequestDto request)
        {
            var blogPost = new BlogPost
            {
                Id = id,
                Title = request.Title,
                Content = request.Content,
                Author = request.Author,
                ShortDescription = request.ShortDescription,
                FeaturedImageUrl = request.FeaturedImageUrl,
                IsVisible = request.IsVisible,
                PublishedDate = request.PublishedDate,
                UrlHandle = request.UrlHandle,
                Categories = new List<Category>()
            };

            foreach (var categoryGuid in request.Categories) 
            {
                var existingCatgeory = await categoryRepository.GetById(categoryGuid);

                if (existingCatgeory != null) 
                {
                    blogPost.Categories.Add(existingCatgeory);
                }
            }

            var updateBlogPost = await blogPostRepository.UpdateAsync(blogPost);

            if (updateBlogPost == null) 
            {
                return NotFound();
            }

            var response = new BlogPostDto
            {
                Title = blogPost.Title,
                Content = blogPost.Content,
                Author = blogPost.Author,
                ShortDescription = blogPost.ShortDescription,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                IsVisible = blogPost.IsVisible,
                PublishedDate = blogPost.PublishedDate,
                UrlHandle = blogPost.UrlHandle,
                Categories = blogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle,
                }).ToList()


            };

            return Ok(response);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]   
        public async Task<IActionResult> DeleteBlogPost([FromRoute] Guid id)
        {
            var deletedBlogPost = await blogPostRepository.DeleteAsync(id);
            if (deletedBlogPost == null) 
            {
                return NotFound();
            }

            var response = new BlogPostDto { 

                Id = deletedBlogPost.Id,
                Title = deletedBlogPost.Title, 
                Content = deletedBlogPost.Content,
                Author = deletedBlogPost.Author,
                ShortDescription = deletedBlogPost.ShortDescription,
                FeaturedImageUrl= deletedBlogPost.FeaturedImageUrl,
                IsVisible = deletedBlogPost.IsVisible,
                PublishedDate = deletedBlogPost.PublishedDate,
                UrlHandle = deletedBlogPost.UrlHandle,


            
            };

            return Ok(response);
        }
    }
}
