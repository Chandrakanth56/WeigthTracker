using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using WeigthTrackerApplication.Models;

namespace WeigthTrackerApplication.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VendorsController : ControllerBase
    {
        private readonly ILogger<VendorsController> _logger;
        private readonly WightListContext _wightListContext;
        private readonly IConfiguration _configuration;

        public VendorsController(ILogger<VendorsController> logger, WightListContext wightListContext, IConfiguration configuration)
        {
            _logger = logger;
            _wightListContext = wightListContext;
            _configuration = configuration;
        }

        [HttpGet("GetVendorName")]
        public ActionResult<List<Vendor>> GetName(int vendorId)
        {
            try
            {
                var vendor = _wightListContext.Vendors
                    .Where(v => v.VendorId == vendorId)
                    .Select(v => new Vendor
                    {
                        VendorId = v.VendorId,
                        VendorName = v.VendorName,
                    })
                    .ToList();

                if (vendor == null || !vendor.Any())
                {
                    return NotFound("Vendor not found in the database");
                }

                return Ok(vendor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching vendor details for VendorId: {vendorId}", vendorId);
                return StatusCode(500, "An error occurred while fetching vendor details.");
            }
        }

        [HttpGet("GetAllFarmer")]
        public ActionResult<List<Farmer>> GetAllFarmer(int VendorId)
        {
            try
            {
                List<Farmer> farmer = new List<Farmer>();
                var cs = _configuration.GetConnectionString("MyDbConnection");

                using (SqlConnection con = new SqlConnection(cs))
                {
                    string query = "spGetNameOfFarmer";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@VendorId", VendorId);
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();
                    SqlDataReader Dr = cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        Farmer far = new Farmer
                        {
                            FarmerId = Convert.ToInt32(Dr["FarmerID"]),
                            FarmerName = Dr["FarmerName"].ToString()
                        };
                        farmer.Add(far);
                    }
                }
                return Ok(farmer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching farmers for VendorId: {VendorId}", VendorId);
                return StatusCode(500, "An error occurred while fetching farmers.");
            }
        }

        [HttpPost("AddFarmer")]
        public ActionResult<Farmer> AddFarmer([FromBody] Farmer model)
        {
            try
            {
                if (model.VendorId <= 0 || string.IsNullOrWhiteSpace(model.FarmerName) ||
                    string.IsNullOrWhiteSpace(model.FarmerEmail) || string.IsNullOrWhiteSpace(model.PassswordHAsh))
                {
                    return BadRequest("VendorId, FarmerName, FarmerEmail, and Password are required.");
                }

                if (_wightListContext.Farmers.Any(f => f.FarmerEmail == model.FarmerEmail && f.VendorId == model.VendorId))
                {
                    return BadRequest("A farmer with this email already exists under your vendor.");
                }

                var farmer = new Farmer
                {
                    VendorId = model.VendorId,
                    FarmerName = model.FarmerName,
                    FarmerEmail = model.FarmerEmail,
                    PassswordHAsh = model.PassswordHAsh
                };

                _wightListContext.Farmers.Add(farmer);
                _wightListContext.SaveChanges();

                var result = new Farmer
                {
                    FarmerId = farmer.FarmerId,
                    VendorId = farmer.VendorId,
                    FarmerName = farmer.FarmerName,
                    FarmerEmail = farmer.FarmerEmail
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding new farmer.");
                return StatusCode(500, "An error occurred while adding the farmer.");
            }
        }

        [HttpGet("GetFarmerWeights")]
        public ActionResult<List<Weight>> GetFarmerWeights(int FarmerID, int VendorId)
        {
            try
            {
                List<Weight> weights = new List<Weight>();
                string cs = _configuration.GetConnectionString("MyDbConnection");

                using (SqlConnection con = new SqlConnection(cs))
                {
                    using (SqlCommand cmd = new SqlCommand("spEachFarmerRecordforVendor", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FarmerID", FarmerID);
                        cmd.Parameters.AddWithValue("@VendorId", VendorId);
                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                Weight weight = new Weight()
                                {
                                    Weights = Convert.ToDouble(dr["Weights"]),
                                    Timestamp = dr["Timestamp"] != DBNull.Value
                                        ? Convert.ToDateTime(dr["Timestamp"])
                                        : null,
                                };
                                weights.Add(weight);
                            }
                        }
                    }
                }

                return Ok(weights);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching weights for FarmerID: {FarmerID}, VendorId: {VendorId}", FarmerID, VendorId);
                return StatusCode(500, "An error occurred while fetching weights.");
            }
        }

        [HttpGet("GetFArmerFullDetails")]
        public ActionResult<List<Farmer>> GetFUllDetailsOfFarmer(int FarmerId)
        {
            try
            {
                if (FarmerId <= 0)
                {
                    return BadRequest("Enter a valid FarmerId");
                }

                var farmer = _wightListContext.Farmers.FirstOrDefault(f => f.FarmerId == FarmerId);
                if (farmer == null)
                {
                    _logger.LogWarning("Farmer with FarmerId: {FarmerId} not found", FarmerId);
                    return NotFound("Farmer not found");
                }

                var far = new Farmer()
                {
                    FarmerId = farmer.FarmerId,
                    FarmerName = farmer.FarmerName,
                    FarmerEmail = farmer.FarmerEmail,
                    VendorId = farmer.VendorId,
                };

                return Ok(far);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching full details for FarmerId: {FarmerId}", FarmerId);
                return StatusCode(500, "An error occurred while fetching farmer details.");
            }
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentWeights([FromQuery] int vendorId)
        {
            try
            {
                var recentWeights = await _wightListContext.Weights
                .Include(w => w.Farmer)
                .Where(w => w.Farmer.VendorId == vendorId)
                .OrderByDescending(w => w.Timestamp)
                .Take(5)
                .Select(w => new
                {
                    w.WeightId,
                    w.FarmerId,
                    FarmerName = w.Farmer.FarmerName,
                    w.Weights,
                    w.Timestamp
                })
                .ToListAsync();

                if (recentWeights == null || recentWeights.Count == 0)
                    return NotFound(new { Message = "No recent weights found for this vendor." });

                return Ok(recentWeights);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching recent weights for this Vendor");
                return StatusCode(500, "An error occurred while fetching weight details.");
            }

        }

        [HttpGet("FarmerStats")]
        public async Task<IActionResult> GetFarmerStatsByVendor([FromQuery] int vendorId)
        {
            try
            {
                var farmerStats = await _wightListContext.Farmers
                    .Where(f => f.VendorId == vendorId)
                    .Select(f => new
                    {
                        f.FarmerId,
                        LastEntry = _wightListContext.Weights
                            .Where(w => w.FarmerId == f.FarmerId)
                            .OrderByDescending(w => w.Timestamp)
                            .Select(w => w.Timestamp)
                            .FirstOrDefault(),
                        TotalWeight = _wightListContext.Weights
                            .Where(w => w.FarmerId == f.FarmerId)
                            .Sum(w => (double?)w.Weights) ?? 0
                    })
                    .ToListAsync();

                if (farmerStats == null || farmerStats.Count == 0)
                    return NotFound(new { Message = "No farmers found for this vendor." });

                return Ok(farmerStats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching Farmer Stats for this vendor");
                return StatusCode(500, "An error occurred while fetching Farmer stats.");
            }
        }

        [HttpGet("SearchWithLatestWeight")]
        public async Task<IActionResult> SearchFarmersWithLatestWeight(
    [FromQuery] string? name,
    [FromQuery] int? id,
    [FromQuery] int? vendorId)
        {
            if (string.IsNullOrEmpty(name) && !id.HasValue && !vendorId.HasValue)
                return BadRequest("Provide at least one search parameter (name, ID, or vendor).");

            var query = _wightListContext.Farmers.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(f => f.FarmerName.Contains(name));

            if (id.HasValue)
                query = query.Where(f => f.FarmerId == id.Value);

            if (vendorId.HasValue)
                query = query.Where(f => f.VendorId == vendorId.Value);

            var result = await query
                .Select(f => new
                {
                    f.FarmerId,
                    f.FarmerName,
                })
                .ToListAsync();

            if (!result.Any())
                return NotFound("No farmers found matching the criteria.");

            return Ok(result);
        }

        [HttpGet("GetFarmerWeightsByVendor")]
        public ActionResult<object> GetFarmerWeightsByVendor(
                                                                int vendorId,
                                                                int farmerId,
                                                                int pageNumber = 1,
                                                                int pageSize = 10,
                                                                DateOnly? startDate = null,
                                                                DateOnly? endDate = null)
                                                                    {
            try
            {
                if (farmerId <= 0)
                    return BadRequest("Invalid Farmer ID.");

                var query = _wightListContext.Weights
                    .Where(w => w.FarmerId == farmerId && w.Farmer.VendorId == vendorId);

                if (!query.Any())
                    return NotFound("No weights found for this farmer under the specified vendor.");

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
                _logger.LogError(ex, "Error fetching weights for farmer by vendor.");
                return StatusCode(500, "Server error.");
            }
        }


    }
}
