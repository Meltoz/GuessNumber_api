using Application.Services;
using AutoMapper;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels.Admin;

namespace Web.Controllers.Admins
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminQuestionController(QuestionService qs, CategoryService cs, IMapper m) : ControllerBase
    {
        private readonly QuestionService _questionService = qs;
        private readonly CategoryService _categoryService = cs;
        private readonly IMapper _mapper = m;

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] QuestionAdminAddVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _categoryService.GetByIdAsync(model.Category.Id);

            var question = await _questionService.AddQuestion(
                model.Libelle, 
                model.Response.ToString(), 
                category, 
                model.Visibility, 
                model.Type, 
                model.Author, 
                model.Unit);

            return Ok(_mapper.Map<QuestionAdminVM>(question));
        }
    }
}
