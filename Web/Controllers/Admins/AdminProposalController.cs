using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels.Admin;

namespace Web.Controllers.Admins
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminProposalController(ProposalService ps, IMapper m) : ControllerBase
    {
        private readonly ProposalService _proposalService = ps;
        private readonly IMapper _mapper = m;

        [HttpGet]
        public async Task<IActionResult> GetNext(Guid? id)
        {
            var nextProposal = await _proposalService.GoToNext(id);

            return Ok(_mapper.Map<ProposalAdminVM>(nextProposal));
        }

        [HttpGet]
        public async Task<IActionResult> Count()
        {
            var proposalCount = await _proposalService.Count();

            return Ok(proposalCount);
        }
    }
}
