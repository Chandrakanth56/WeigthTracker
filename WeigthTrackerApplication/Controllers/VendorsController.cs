using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeigthTrackerApplication.Models;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;

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

        [HttpPost]
        public ActionResult<List<Farmer>> AddFarmer([FromBody] Farmer model)
        {
            try
            {
                var farmers = new Farmer()
                {
                    VendorId = model.VendorId,
                    FarmerName = model.FarmerName,
                    FarmerEmail = model.FarmerEmail,
                    PassswordHAsh = model.PassswordHAsh,
                };

                _wightListContext.Farmers.Add(farmers);
                _wightListContext.SaveChanges();
                return Ok(farmers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding new farmer");
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
    }
}
