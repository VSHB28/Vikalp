using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Vikalp.DTO;
using Vikalp.Service.Interfaces;

namespace Vikalp.Service.Services
{
    public class LocationDistrictService : ILocationDistrictService
    {
        private readonly IConfiguration _configuration;

        public LocationDistrictService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));
        }

        public List<LocationDistrictDTO> GetAll()
        {
            List<LocationDistrictDTO> list = new();

            using SqlConnection con = GetConnection();
            SqlCommand cmd = new("USP_LocationDistrict_CRUD", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "SELECT");

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new LocationDistrictDTO
                {
                    DistrictId = Convert.ToInt32(dr["DistrictId"]),
                    StateId = Convert.ToInt32(dr["StateId"]),
                    DistrictName = dr["DistrictName"].ToString(),
                    DistrictCode = dr["DistrictCode"].ToString(),
                    IsActive = Convert.ToBoolean(dr["IsActive"]),
                    IsCentinalDistrict = Convert.ToBoolean(dr["IsCentinalDistrict"])
                });
            }
            return list;
        }

      
        public LocationDistrictDTO GetById(int districtId)
        {
            LocationDistrictDTO model = null;

            using SqlConnection con = GetConnection();
            SqlCommand cmd = new("USP_LocationDistrict_CRUD", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "SELECTBYID");
            cmd.Parameters.AddWithValue("@DistrictId", districtId);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                model = new LocationDistrictDTO
                {
                    DistrictId = dr["DistrictId"] != DBNull.Value ? Convert.ToInt32(dr["DistrictId"]) : 0,
                    StateId = dr["StateId"] != DBNull.Value ? Convert.ToInt32(dr["StateId"]) : 0,
                    DistrictName = dr["DistrictName"] != DBNull.Value ? dr["DistrictName"].ToString() : string.Empty,
                    DistrictCode = dr["DistrictCode"] != DBNull.Value ? dr["DistrictCode"].ToString() : string.Empty,
                    IsActive = dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false,
                    IsCentinalDistrict = dr["IsCentinalDistrict"] != DBNull.Value ? Convert.ToBoolean(dr["IsCentinalDistrict"]) : false
                };
            }

            return model;
        }

        public string Insert(LocationDistrictDTO model)
        {
            using SqlConnection con = GetConnection();
            SqlCommand cmd = new("USP_LocationDistrict_CRUD", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "INSERT");
            cmd.Parameters.AddWithValue("@StateId", model.StateId);
            cmd.Parameters.AddWithValue("@DistrictName", model.DistrictName);
            cmd.Parameters.AddWithValue("@DistrictCode", model.DistrictCode);
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
            cmd.Parameters.AddWithValue("@IsCentinalDistrict", model.IsCentinalDistrict);

            con.Open();
            return cmd.ExecuteScalar()?.ToString();
        }


        public string Update(LocationDistrictDTO model)
        {
            if (model.DistrictId <= 0)
                throw new ArgumentException("Invalid DistrictId");

            using SqlConnection con = GetConnection();
            SqlCommand cmd = new SqlCommand("USP_LocationDistrict_CRUD", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "UPDATE");
            cmd.Parameters.AddWithValue("@DistrictId", model.DistrictId);
            cmd.Parameters.AddWithValue("@StateId", model.StateId);
            cmd.Parameters.AddWithValue("@DistrictName", model.DistrictName);
            cmd.Parameters.AddWithValue("@DistrictCode", model.DistrictCode);
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
            cmd.Parameters.AddWithValue("@IsCentinalDistrict", model.IsCentinalDistrict);

            con.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected.ToString();
        }

        public string Delete(int districtId)
        {
            using SqlConnection con = GetConnection();
            SqlCommand cmd = new("USP_LocationDistrict_CRUD", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "DELETE");
            cmd.Parameters.AddWithValue("@DistrictId", districtId);

            con.Open();
            return cmd.ExecuteScalar()?.ToString();
        }

        
        public List<LocationDistrictDTO> GetByStateId(int stateId)
        {
            List<LocationDistrictDTO> list = new();

            using SqlConnection con = GetConnection();
            SqlCommand cmd = new("USP_LocationDistrict_CRUD", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "SELECT");
            cmd.Parameters.AddWithValue("@StateId", stateId);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new LocationDistrictDTO
                {
                    DistrictId = Convert.ToInt32(dr["DistrictId"]),
                    StateId = Convert.ToInt32(dr["StateId"]),
                    DistrictName = dr["DistrictName"].ToString(),
                    DistrictCode = dr["DistrictCode"].ToString(),
                    IsActive = dr["IsActive"] != DBNull.Value && Convert.ToBoolean(dr["IsActive"]),
                    IsCentinalDistrict = dr["IsCentinalDistrict"] != DBNull.Value && Convert.ToBoolean(dr["IsCentinalDistrict"])
                });
            }

            return list;
        }

    }
}
