using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController(IMapper m) : ControllerBase
    {
        private readonly IMapper _mapper = m;


    }
}
