using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;

namespace Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController(ProposalService ps, IMapper m) : ControllerBase
    {
        private readonly ProposalService _proposalService = ps;
        private readonly IMapper _mapper = m;

        [HttpPost]
        public async Task<IActionResult> AddProposal([FromBody]ProposalVM proposal)
        {
            if (proposal is null)
                return BadRequest("Proposal is null");

            await _proposalService.AddProposal(proposal.Libelle, proposal.Response, proposal.Author, proposal.Source);

            return Ok();
        }
    }
}
