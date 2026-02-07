using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared.Filters;
using Web.Extensions;
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

        [HttpGet]
        public async Task<IActionResult> Search(int pageIndex, int pageSize, [FromQuery]QuestionFilter filter)
        {
            if (pageIndex < 0 || pageSize < 1)
                return BadRequest();

            var questions = await _questionService.Search(pageIndex, pageSize, filter);

            Response.AppendTotalCountHeader(questions.TotalCount);
            return Ok(_mapper.Map<IEnumerable<QuestionAdminVM>>(questions.Data));
        }

        [HttpGet]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            if(id == Guid.Empty)
                return BadRequest();

            var question = await _questionService.GetById(id);

            return Ok(_mapper.Map<QuestionAdminVM>(question));
        }

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


        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] QuestionAdminVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _questionService.DeleteQuestion(id);

            return Ok();

        }
    }
}
