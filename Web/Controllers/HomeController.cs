using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;
using Web.ViewModels.Admin;

namespace Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController(ProposalService ps, ReportService rs, ActualityService acs, CommunicationService cs, IMapper m) : ControllerBase
    {
        private readonly ProposalService _proposalService = ps;
        private readonly ReportService _reportService = rs;
        private readonly ActualityService _actualityService = acs;
        private readonly CommunicationService _communicationService = cs;
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

        [HttpGet]
        public async Task<IActionResult> GetActualities()
        {
            var actualities = await _actualityService.GetActiveActualities();


            if(!actualities.Any())
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<ActualityVM>>(actualities));
        }


        [HttpGet]
        public async Task<IActionResult> GetCommunications()
        {
            var communications = await _communicationService.GetActiveCommunications();

            if (!communications.Any())
                return NoContent();

            return Ok(communications.Select(c => c.Content).ToList());
        }
    }
}
