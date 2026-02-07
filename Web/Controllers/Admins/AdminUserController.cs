using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Enums.Sorting;
using System.Text.RegularExpressions;
using Web.Extensions;
using Web.ViewModels.Admin;

namespace Web.Controllers.Admins
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminUserController(UserService us, IMapper m) : ControllerBase
    {
        private readonly UserService _userService = us;
        private readonly IMapper _mapper = m;

        [HttpGet]
        public async Task<IActionResult> SearchUser(int pageIndex, int pageSize, bool includeGuest, string sort, string? search)
        {
            if (pageIndex < 0 || pageSize < 1)
                return BadRequest();

            if (!GetSorting(sort, out var sortOptions))
                return BadRequest();

            var users = await _userService.Search(pageIndex, pageSize, sortOptions, search, includeGuest);

            Response.AppendTotalCountHeader(users.TotalCount);
            return Ok(_mapper.Map<IEnumerable<UserAdminVM>>(users.Data)); 
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetDetail([FromRoute]Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            var user = await _userService.GetDetail(id);

            return Ok(_mapper.Map<UserAdminVM>(user));
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            var user = await _userService.CreateDefaultUser();

            return Ok(_mapper.Map<UserAdminVM>(user));
        }

        #region private methods

        private bool GetSorting(string sort, out SortOption<SortUser> sortOption)
        {
            var patternSort = @"^(pseudo|created|lastlogin)_(ascending|descending)$";
            var regex = new Regex(patternSort);
            var match = regex.Match(sort);
            if (!match.Success)
            {
                sortOption = null;
                return false;
            }

            var field = match.Groups[1].Value;
            var direction = match.Groups[2].Value;
            sortOption = SortOptionFactory.Create<SortUser>(field, direction);
            return true;
        }

        #endregion

    }
}
