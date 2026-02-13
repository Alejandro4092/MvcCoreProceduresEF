using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;
using System;
using System.Data.Common;

namespace MvcCoreProceduresEF.Repositories
{
    #region procedures 
//    create procedure SP_ALL_ENFERMOS
//as
//select* from ENFERMO
//go
//create procedure SP_FIND_ENFERMO(@inscripcion nvarchar(50))
//as
//select* from ENFERMO
//where INSCRIPCION=@inscripcion
//go

//create procedure SP_DELETE_ENFERMO(@inscripcion nvarchar(50))
//as 
//delete from ENFERMO where INSCRIPCION=@inscripcion
//go

//create procedure SP_INSERT_ENFERMO(@apellido nvarchar(50),@direccion nvarchar(50),@fechanac datetime,@s nvarchar(50),@nss nvarchar(50))
//as
//declare @inscripcion nvarchar(50)
//select @inscripcion=MAX(INSCRIPCION)+1 from ENFERMO
//insert into ENFERMO VALUES(@inscripcion,@apellido,@direccion,@fechanac,@s,@nss) 
//go

    #endregion
    public class RepositoryEnfermos
    {
        private HospitalContext context;
        public RepositoryEnfermos(HospitalContext context)
        {
            this.context = context;
        }
        public async Task<List<Enfermo>> GetEnfermosAsync()
        {
            //necesitamos un command vamos a utilizar un using
            using(DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
                    
            {
                string sql = "SP_ALL_ENFERMOS";
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.CommandText = sql;
                await com.Connection.OpenAsync();
                DbDataReader reader = await com.ExecuteReaderAsync();
                List<Enfermo> enfermos = new List<Enfermo>();
                while(await reader.ReadAsync())
                {
                    Enfermo enfermo = new Enfermo
                    {
                        Inscripcion = reader["INSCRIPCION"].ToString(),
                        Apellido = reader["APELLIDO"].ToString(),
                        Direccion = reader["DIRECCION"].ToString(),
                        FechaNacimiento = DateTime.Parse(reader["FECHA_NAC"].ToString()),
                        Genero = reader["S"].ToString(),
                        Nss = reader["NSS"].ToString()
                    };
                    enfermos.Add(enfermo);
                }
                await reader.CloseAsync();
                await com.Connection.CloseAsync();
                return enfermos;
            
            }

        }
        public async Task<Enfermo> FindEnfermoAsync(string inscripcion)
        {
            //para llmar a un proceimiento que contiene parameters la llamada se realiza mediante el nombre del procedure
            //y cada parametro a continuacion en la declaracion del sql SP_PROCEDURE @pam1,@pam2
            string sql = "SP_FIND_ENFERMO @inscripcion";
            SqlParameter pamIns = new SqlParameter("@inscripcion", inscripcion);
            //si los datos que devuelve el procedure estan mapeados podemos utiizar el metodo from sqlraw para recuperar directamente el model o los models
            //no podemos consultar y extraer a la vez,s e deb realizar siempre en dos pasos
            var consulta = this.context.Enfermos.FromSqlRaw(sql, pamIns);
            //debemos utilizar asenumerable() par a extraer los datos
            Enfermo enfermo = await consulta.AsAsyncEnumerable().FirstOrDefaultAsync();
            return enfermo;
        }
        public async Task DelteEnfermoAsync(string inscripcion)
        {
            string sql = "SP_DELETE_ENFERMO";
            SqlParameter pamIns = new SqlParameter("@inscripcion", inscripcion);
            using(DbCommand com=this.context.Database.GetDbConnection().CreateCommand())
            {
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.CommandText = sql;
                com.Parameters.Add(pamIns);
                await com.Connection.OpenAsync();
                await com.ExecuteNonQueryAsync();
                await com.Connection.CloseAsync();
                com.Parameters.Clear();
            }
        }
        public async Task DeleteENfermoRawAsync(string inscripcion)
        {
            string sql = "SP_DELETE_ENFERMO @inscripcion";
            SqlParameter pamIns = new SqlParameter("@inscripcion", inscripcion);
            await this.context.Database.ExecuteSqlRawAsync(sql, pamIns);
        }
        public async Task InsertEnfermoAsynx(string apellido, string direccion,DateTime fechaNac,string sexo,string nss)
        {
            string sql = "SP_INSERT_ENFERMO @apellido,@direccion,@fechanac,@s,@nss";
            SqlParameter pamApe = new SqlParameter("@apellido", apellido);
            SqlParameter pamDirec = new SqlParameter("@direccion", direccion);
            SqlParameter pamFecha = new SqlParameter("@fechanac", fechaNac);
            SqlParameter pamSexo = new SqlParameter("@s", sexo);
            SqlParameter pamNss = new SqlParameter("@nss", nss);
            await this.context.Database.ExecuteSqlRawAsync(sql, pamApe,pamDirec,pamFecha,pamSexo,pamNss);
        }
    }
}
