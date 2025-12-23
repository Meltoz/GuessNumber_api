using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;

namespace Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController(ProposalService ps, ReportService rs, IMapper m) : ControllerBase
    {
        private readonly ProposalService _proposalService = ps;
        private readonly ReportService _reportService = rs;
        private readonly IMapper _mapper = m;

        [HttpPost]
        public async Task<IActionResult> AddProposal([FromBody]ProposalVM proposal)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _proposalService.AddProposal(proposal.Libelle, proposal.Response, proposal.Author, proposal.Source);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddReport([FromBody]ReportVM report)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            await _reportService.CreateReport(report.Type, report.Context, report.Explanation, report.Mail);

            return Ok();
        }
    }
}
