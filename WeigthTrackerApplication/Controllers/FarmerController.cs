using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Security.Cryptography.Xml;
using WeigthTrackerApplication.Models;
using System.Data;

namespace WeigthTrackerApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FarmerController : ControllerBase
    {
        private readonly ILogger<FarmerController> _logger;
        private readonly WightListContext _context;
        private readonly IConfiguration _configuration;

        public FarmerController(ILogger<FarmerController> logger, WightListContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("GetTheirDetails")]
        public ActionResult<List<Farmer>> GetAllFarmer(int farmerID)
        {
            var far = _context.Farmers.Where(f => f.FarmerId == farmerID).Select(s =>
                new Farmer
                {
                    FarmerId = s.FarmerId,
                    FarmerName = s.FarmerName,
                    FarmerEmail = s.FarmerEmail,
                    PassswordHAsh = s.PassswordHAsh,
                }).FirstOrDefault();
            return Ok(far);
        }

        [HttpPost]
        public ActionResult<List<Weight>> AddWieght([FromBody] Weight model, int FarmerId)
        {
            var weight = new Weight()
            {
                FarmerId = FarmerId,
                Weights = model.Weights,
            };
            _context.Add(weight);
            _context.SaveChanges();
            return Ok(weight);
        }
        [HttpGet]
        public List<Weight> GetWeightsbyID(int FarmerID)
        {
            List<Weight> weight = _context.Weights.Where(w=>w.FarmerId==FarmerID).ToList();
            return weight;
        }
    }
}

