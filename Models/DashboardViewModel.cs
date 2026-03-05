namespace AppEnvios.Models
{
    public class DashboardViewModel
    {
        // Totales generales
        public int TotalEnvios { get; set; }
        public int TotalClientes { get; set; }
        public int TotalUsuarios { get; set; }

        // Estados de envíos
        public int EnviosPendientes { get; set; }
        public int EnviosEnTransito { get; set; }
        public int EnviosEntregados { get; set; }

        // Métricas financieras
        public decimal IngresoTotal { get; set; }
        public decimal PromedioCostoEnvio { get; set; }

        // Envíos recientes (para mostrar en dashboard)
        public List<Envio>? EnviosRecientes { get; set; }

        // Estadísticas mensuales (para gráficos)
        public Dictionary<string, int>? EnviosPorMes { get; set; }
        public Dictionary<string, decimal>? IngresosPorMes { get; set; }
    }
}