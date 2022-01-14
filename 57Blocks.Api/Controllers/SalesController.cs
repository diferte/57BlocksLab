using _57Blocks.Api.Helpers;
using _57Blocks.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace _57Blocks.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ApiContext _apiContext;
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;

        public SalesController(ILogger<AccountController> logger, ApiContext context, IConfiguration configuration)
        {
            _apiContext = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(Sales saleModel)
        {
            try
            {
                await _apiContext.Sales.AddAsync(saleModel);
                await _apiContext.SaveChangesAsync();

                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(Sales saleModel)
        {
            try
            {
                _apiContext.Sales.Remove(saleModel);
                await _apiContext.SaveChangesAsync();

                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("list")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetPaged(string ID,bool isPublic,[FromQuery] PageParameters pageParameters)
        {

            Guid requestID = Guid.Parse(ID);

            // Get total number of records
            int total = _apiContext.Sales.AsNoTracking().Where(x => x.CreatorID == requestID && x.IsPublic == isPublic).Count();

            // Select the customers based on paging parameters
            var usuarios = await _apiContext.Sales.AsNoTracking().Where(x => x.CreatorID == requestID && x.IsPublic == isPublic)
                .Skip((pageParameters.PageNumber - 1) * pageParameters.PageSize)
                .Take(pageParameters.PageSize)
                .ToListAsync();

            // Return the list of users

            var metadata = new MetaData
            {
                CurrentPage = pageParameters.PageNumber,
                TotalPages = (int)Math.Ceiling(total / (double)pageParameters.PageSize),
                TotalCount = total,
                PageSize = pageParameters.PageSize
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(usuarios);
        }


    }
}
