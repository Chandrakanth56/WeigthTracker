using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.Xml;
using WeigthTrackerApplication.Models;

namespace WeigthTrackerApplication.Controllers
{
   // [Authorize(Roles = "Farmer")]
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
            try
            {
                var FarmerID= User.Claims.FirstOrDefault(c => c.Type == "FarmerId")?.Value;

                if (farmerID <= 0)
                {
                    return BadRequest("Invalid farmer ID.");
                }
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving details for farmer ID {0}", farmerID);
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost("AddWeights")]
        public ActionResult<List<Weight>> AddWeights([FromBody] List<Weight> models, int FarmerId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding weights for farmer ID {0}", FarmerId);
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet]
        public List<Weight> GetWeightsbyID(int FarmerID)
        {
            try
            {
                if (FarmerID <= 0)
                {
                    return new List<Weight>();
                }
                List<Weight> weight = _context.Weights
                    .Where(w => w.FarmerId == FarmerID)
                    .OrderByDescending(w => w.Timestamp)
                    .Take(5)
                    .ToList();

                return weight;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving weights for farmer ID {0}", FarmerID);
                return new List<Weight>();
            }
        }


        [HttpGet("GetTotalWeight")]
        public ActionResult<int> GetTotalWeight(int farmerId)
        {
            try
            {
                if (farmerId <= 0)
                {
                    return BadRequest("Invalid farmer ID.");
                }
                var totalWeight = _context.Weights
                    .Where(w => w.FarmerId == farmerId)
                    .Sum(w => w.Weights);

                return Ok(totalWeight);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while calculating total weight for farmer ID {0}", farmerId);
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet("RecordCount")]
        public ActionResult<int> GetRecordCount(int farmerId)
        {

            try
            {
                if (farmerId <= 0)

                {
                    return BadRequest("Invalid farmer ID.");
                }
                var recordCount = _context.Weights
                    .Count(w => w.FarmerId == farmerId);
                return Ok(recordCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the record count for farmer ID {0}", farmerId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetPaginatedWeights")]
        public ActionResult<object> GetPaginatedWeights(
                                                        int farmerId,
                                                        int pageNumber = 1,
                                                        int pageSize = 10,
                                                        DateOnly? startDate = null,
                                                        DateOnly? endDate = null)
        {
            try
            {
                if (farmerId <= 0)
                    return BadRequest("Invalid Farmer ID");

                var query = _context.Weights
                    .Where(w => w.FarmerId == farmerId);

                if (startDate.HasValue && endDate.HasValue)
                {
                    DateTime from = startDate.Value.ToDateTime(TimeOnly.MinValue);
                    DateTime to = endDate.Value.ToDateTime(TimeOnly.MaxValue);
                    query = query.Where(w => w.Timestamp >= from && w.Timestamp <= to);
                }

                var totalRecords = query.Count();

                var data = query
                    .OrderByDescending(w => w.Timestamp)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var response = new
                {
                    TotalRecords = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                    Data = data
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated weights.");
                return StatusCode(500, "Server error.");
            }
        }


    }
}

