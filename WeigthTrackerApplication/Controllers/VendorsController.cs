using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeigthTrackerApplication.Models;
using System.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Http.HttpResults;
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
            _wightListContext= wightListContext;
            _configuration = configuration;
        }
        [HttpGet("GetVendorName")]
        public ActionResult<List<Vendor>> GetName(int vendorId)
        {
            var vendor = _wightListContext.Vendors.Where(v => v.VendorId ==vendorId).Select(v =>
            new Vendor { 
            
                VendorId = v.VendorId,
                 VendorName = v.VendorName,
            });
            if(vendor==null)
            {
                return NotFound("Vendor is not Found in the database");
            }
            return Ok(vendor);
        }
        [HttpGet("GetAllFarmer")]
        public ActionResult<List<Farmer>> GetAllFarmer(int VendorId)
        {
            List<Farmer> farmer = new List<Farmer>();
            var cs = _configuration.GetConnectionString("MyDbConnection");
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "spGetNameOfFarmer";
                SqlCommand cmd = new SqlCommand(query,con);
                cmd.Parameters.AddWithValue("@VendorId", VendorId);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader Dr= cmd.ExecuteReader();
                while(Dr.Read())
                {
                    Farmer far= new Farmer();
                    far.FarmerId = Convert.ToInt32(Dr["FarmerID"]);
                    far.FarmerName = Dr["FarmerName"].ToString();
                    farmer.Add(far);
                }
               
            }
            return Ok(farmer);
        }
        [HttpPost]
        public ActionResult<List<Farmer>> AddFarmer([FromBody] Farmer model)
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

        [HttpGet("GetFarmerWeights")]
        public ActionResult<List<Weight>> GetFarmerWeights(int FarmerID,int VendorId)
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
        [HttpGet("GetFArmerFullDetails")]
        public ActionResult<List<Farmer>> GetFUllDetailsOfFarmer(int FarmerId)
        {
            if(FarmerId<=0)
            {
                return BadRequest("Enter the valid FarmerId");
            }
            var farmer=_wightListContext.Farmers.Where(f=>f.FarmerId == FarmerId).FirstOrDefault();
            if(farmer == null)
            {
                _logger.LogError("Farmer with this is not found in the Database");
                return NotFound("Famernot found Enter");
            }
            var far = new Farmer()
            {
                FarmerId = farmer.FarmerId,
                FarmerName=farmer.FarmerName,
                FarmerEmail=farmer.FarmerEmail,
                VendorId=farmer.VendorId,
            };
            return Ok(far);
        }
    }
}
