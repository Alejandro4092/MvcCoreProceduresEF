using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;
using System.Collections.Generic;
using System.Data.Common;
using System.Numerics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MvcCoreProceduresEF.Repositories
{
    #region procedures
    //    create procedure SP_GET_ESPECIALIDAD
    //AS
    //select DISTINCT ESPECIALIDAD from DOCTOR
    //GO
    //CREATE PROCEDURE SP_INCREMENTAR_SALARIO(@especialidad NVARCHAR(50),@incremento INT)
    //AS
    //BEGIN
    //    UPDATE DOCTOR
    //    SET SALARIO = SALARIO + @incremento
    //    WHERE ESPECIALIDAD = @especialidad
    //END
    //select* from DOCTOR

    //    create procedure SP_GET_DOCTORES(@especialidad nvarchar(50))
    //as
    //select* FROM DOCTOR WHERE ESPECIALIDAD=@especialidad
    //go
    #endregion
    public class RepositoryDoctores
    {
        private HospitalContext context;
        public RepositoryDoctores(HospitalContext context)
        {
            this.context = context;
        }
        public async Task<List<string>> GetEspecialidadesAsync()
        {
            string sql = "SP_GET_ESPECIALIDAD";
            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {   
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.CommandText = sql;
                await com.Connection.OpenAsync();
                DbDataReader reader = await com.ExecuteReaderAsync();
                List<string> especialidades = new List<string>();
                while (await reader.ReadAsync())
                {

                    string espe = reader["ESPECIALIDAD"].ToString();
                    especialidades.Add(espe);
                }
                await reader.CloseAsync();
                await com.Connection.CloseAsync();
                return especialidades;

            }
        }
        public async Task IncrementarSalarioEspecialidad(string especialidad, int incremento)
        {
            string sql = "SP_INCREMENTAR_SALARIO @especialidad, @incremento";
            SqlParameter pamEspe = new SqlParameter("@especialidad", especialidad);
            SqlParameter pamIncre = new SqlParameter("@incremento", incremento);
            
            await this.context.Database.ExecuteSqlRawAsync(sql, pamIncre, pamEspe);
        }
        public async Task<List<Doctor>> GetDoctoresEspecialidadAsync(string especialidad)
        {
            string sql = "SP_GET_DOCTORES @especialidad";
            SqlParameter pamEspe = new SqlParameter("@especialidad", especialidad);
            var consulta = await this.context.Doctores.FromSqlRaw(sql, pamEspe).ToListAsync();
            List<Doctor> doctores = consulta;
            return consulta;
            
        }
     public async Task UpdateDoctoresEspecialidadEFAsync(string especialidad,int incremento)
        {
            //debemos recuperar los datos a modificar/eliminar desde el context
            var consulta = from datos in this.context.Doctores where datos.Especialidad == especialidad select datos;
            List<Doctor> doctores = await consulta.ToListAsync();
            //recorremos los doctores
            foreach(Doctor doc in doctores)
            {
                doc.Salario += incremento;
            }
            await this.context.SaveChangesAsync();
        }


    }
}
    

