using CMSProject.Application.Dtos;
using CMSProject.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CMSProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly IContentService _contentService;
        private readonly ILogger<ContentController> _logger;

        public ContentController(IContentService contentService, ILogger<ContentController> logger)
        {
            _contentService = contentService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContentDto>>> GetAll()
        {
            var contents = await _contentService.GetAllContentsAsync();
            return Ok(contents);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContentDto>> Get(int id)
        {
            var content = await _contentService.GetContentAsync(id);
            return Ok(content);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] ContentDto createContentDto)
        {
            var contentId = await _contentService.CreateContentAsync(createContentDto);
            return CreatedAtAction(nameof(Get), new { id = contentId }, contentId);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ContentDto>>> GetByCategory(int categoryId)
        {
            var contents = await _contentService.GetContentsByCategoryAsync(categoryId);
            return Ok(contents);
        }

        [HttpGet("language/{language}")]
        public async Task<ActionResult<IEnumerable<ContentDto>>> GetByLanguage(string language)
        {
            var contents = await _contentService.GetContentsByLanguageAsync(language);
            return Ok(contents);
        }
    }
}
