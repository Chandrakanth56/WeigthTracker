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
        public ActionResult<List<Farmer>> GetFarmer(int farmerID)
        {
            var far = _context.Farmers.Where(f => f.FarmerId == farmerID).Select(s =>
                new Farmer
                {
                    FarmerId = s.FarmerId,
                    FarmerName = s.FarmerName,
                    FarmerEmail = s.FarmerEmail,
                    VendorId = s.VendorId,
                    PassswordHAsh = s.PassswordHAsh,
                }).FirstOrDefault();
            return Ok(far);
        }

        [HttpPost("AddWeights")]
        public ActionResult<List<Weight>> AddWeights([FromBody] List<Weight> models, int FarmerId)
        {
            if (models == null || !models.Any())
                return BadRequest("No weights provided");

            foreach (var model in models)
            {
                var weight = new Weight
                {
                    FarmerId = FarmerId,
                    Weights = model.Weights,
                    Timestamp = DateTime.Now
                };
                _context.Add(weight);
            }

            _context.SaveChanges();
            return Ok(models);
        }


        [HttpGet]
        public List<Weight> GetWeightsbyID(int FarmerID)
        {
            List<Weight> weight = _context.Weights
                .Where(w => w.FarmerId == FarmerID)
                .OrderByDescending(w => w.Timestamp) 
                .Take(5)                            
                .ToList();

            return weight;
        }

        [HttpGet("GetTotalWeight")]
        public ActionResult<int> GetTotalWeight(int farmerId)
        {
            var totalWeight = _context.Weights
                .Where(w => w.FarmerId == farmerId)
                .Sum(w => w.Weights);

            return Ok(totalWeight);
        }
    }
}

