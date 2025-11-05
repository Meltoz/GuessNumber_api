using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Enums.Sorting;
using System.Text.RegularExpressions;
using Web.Extensions;
using Web.ViewModels;

namespace Web.Controllers.Admins
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminCategoryController(CategoryService cs, IMapper m) : ControllerBase
    {
        private readonly CategoryService _categoryService = cs;
        private readonly IMapper _mapper = m;

        [HttpGet]
        public async Task<IActionResult> Search(int pageIndex, int pageSize, string sort, string search = "")
        {
            if (pageIndex < 0 || pageSize < 1)
                return BadRequest();

            if (!GetSorting(sort, out var sortOptions))
                return BadRequest();

            var categories = await _categoryService.Search(pageIndex, pageSize, sortOptions, search);

            Response.AddTotalCountHeader(categories.TotalCount);

            return Ok(_mapper.Map<IEnumerable<CategoryAdminVM>>(categories.Data));
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CategoryAdminVM category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var categoryInserted = await _categoryService.CreateNew(category.Name);

            return Ok(_mapper.Map<CategoryAdminVM>(categoryInserted));
        }

        private bool GetSorting(string sort, out SortOption<SortCategory> sortOption)
        {
            var patternSort = @"^(name|created)_(ascending|descending)$";
            var regex = new Regex(patternSort);
            var match = regex.Match(sort);
            if (!match.Success)
            {
                sortOption = null;
                return false;
            }

            var field = match.Groups[1].Value;
            var direction = match.Groups[2].Value;
            sortOption = SortOptionFactory.Create<SortCategory>(field, direction);
            return true;
        }
    }
}
