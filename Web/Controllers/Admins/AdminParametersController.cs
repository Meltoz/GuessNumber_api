using Application.Services;
using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Enums.Sorting;
using Web.Converters;
using Web.Extensions;
using Web.ViewModels;
using Web.ViewModels.Admin;

namespace Web.Controllers.Admins
{
    [Route("api")]
    [ApiController]
    public class AdminParametersController(ActualityService acts, CommunicationService cs, ReportService rs, IMapper m) : ControllerBase
    {
        private readonly ActualityService _actualityService = acts;
        private readonly CommunicationService _communicationService = cs;
        private readonly ReportService _reportService = rs;
        private readonly IMapper _mapper = m;

        #region Actuality 

        [HttpGet]
        [Route("actualityAdmin/search")]
        public async Task<IActionResult> SearchActuality(int pageIndex, int pageSize)
        {
            var sortoption = SortOptionFactory.Create<SortActuality>("created", "ascending");
            var t = await _actualityService.Search(pageIndex, pageSize, sortoption, "");

            Response.AppendTotalCountHeader(t.TotalCount);

            return Ok(_mapper.Map<IEnumerable<ActualityAdminVM>>(t.Data));
        }

        [HttpPost]
        [Route("actualityAdmin/add")]
        public async Task<IActionResult> AddActuality([FromBody] ActualityAdminVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var startDate = DateConverter.ParseDate(model.StartDate);
            if(startDate  is null)
                return BadRequest("Invalid start date format.");

            DateTime? endDate = null;
            if (!string.IsNullOrWhiteSpace(model.EndDate))
            {
                var parsedEndDate = DateConverter.ParseDate(model.EndDate);
                if (parsedEndDate is null)
                    return BadRequest("Invalid end date format.");
                endDate = parsedEndDate;
            }

            var inserted =  await _actualityService.CreateNew(model.Title, model.Content, startDate.Value, endDate);

            return Ok(_mapper.Map<ActualityAdminVM>(inserted));
        }

        [HttpPatch]
        [Route("actualityAdmin/update")]
        public async Task<IActionResult> UpdateActuality(ActualityAdminVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var actuality = _mapper.Map<Actuality>(model);
            var actualitypUpdated = await _actualityService.UpdateAsync(actuality);

            return Ok(_mapper.Map<ActualityAdminVM>(actualitypUpdated));
        }

        [HttpDelete]
        [Route("actualityAdmin/delete")]
        public async Task<IActionResult> DeleteActuality(Guid id)
        {
            await _actualityService.DeleteActualityAsync(id);

            return Ok();
        }

        #endregion

        #region Communication

        [HttpGet]
        [Route("communicationAdmin/search")]
        public async Task<IActionResult> SearchCommunication(int pageIndex, int pageSize, string? message)
        {
            var sortoption = SortOptionFactory.Create<SortCommunication>("active", "descending");
            var communications = await _communicationService.Search(pageIndex, pageSize, sortoption, message);

            Response.AppendTotalCountHeader(communications.TotalCount);

            return Ok(_mapper.Map<IEnumerable<CommunicationAdminVM>>(communications.Data));
        }

        [HttpPost]
        [Route("communicationAdmin/add")]
        public async Task<IActionResult> AddCommunication([FromBody]CommunicationAdminVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var startDate = DateConverter.ParseDate(model.StartDate);
            if (startDate is null)
                return BadRequest("Invalid start date format.");

            DateTime? endDate = null;
            if (!string.IsNullOrWhiteSpace(model.EndDate))
            {
                var parsedEndDate = DateConverter.ParseDate(model.EndDate);
                if (parsedEndDate is null)
                    return BadRequest("Invalid end date format.");
                endDate = parsedEndDate;
            }

            var communication = await _communicationService.CreateNew(model.Content, startDate.Value, endDate);

            return Ok(_mapper.Map<CommunicationAdminVM>(communication));
        }

        [HttpPatch]
        [Route("communicationAdmin/update")]
        public async Task<IActionResult> UpdateCommunication([FromBody]CommunicationAdminVM model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var communication = await _communicationService.UpdateAsync(_mapper.Map<Communication>(model));

            return Ok(_mapper.Map<CommunicationAdminVM>(communication));
        }

        [HttpDelete]
        [Route("communicationAdmin/delete")]
        public async Task<IActionResult> DeleteCommunication(Guid idCommunication)
        {
            await _communicationService.DeleteAsync(idCommunication);

            return Ok();
        }
        #endregion

        #region report

        [HttpGet]
        [Route("reportAdmin/search")]
        public async Task<IActionResult> SearchReport(int pageIndex, int pageSize, string? search)
        {
            if (pageIndex < 0 || pageSize < 1)
                return BadRequest();

            var reports = await _reportService.GetAll(pageIndex, pageSize, search);

            Response.AppendTotalCountHeader(reports.TotalCount);

            return Ok(_mapper.Map<IEnumerable<ReportVM>>(reports.Data));
        }

        [HttpGet]
        [Route("reportAdmin/getbyid")]
        public async Task<IActionResult> ReportById(Guid id)
        {
            var report = await _reportService.GetById(id);

            return Ok(_mapper.Map<ReportVM>(report));
        }

        [HttpDelete]
        [Route("reportAdmin/delete")]
        public async Task<IActionResult> DeleteReport(Guid id)
        {
            await _reportService.Delete(id);

            return Ok();
        }

        #endregion
    }
}
