using SCA_MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCA_MVC.Services
{
    public interface IEstadisticasNegocio
    {
        Task<EstadisticasViewModel> ObtenerTodosAsync();
    }
}
