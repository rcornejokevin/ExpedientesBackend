using Microsoft.EntityFrameworkCore;
using DBHandler.Models;
using Microsoft.Extensions.Configuration;

namespace DBHandler.Context
{
    public class DBHandlerContext : DbContext
    {
        private readonly string defaultSchema;
        public DBHandlerContext(DbContextOptions<DBHandlerContext> options, IConfiguration configuration) : base(options)
        {
            var configuredSchema = configuration["DatabaseSchemas:App"];
            defaultSchema = string.IsNullOrWhiteSpace(configuredSchema)
                ? "TANGRAM"
                : configuredSchema;
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Flujo> Flujos { get; set; }
        public DbSet<Etapa> Etapas { get; set; }
        public DbSet<EtapaDetalle> EtapaDetalles { get; set; }
        public DbSet<Campo> Campos { get; set; }
        public DbSet<Expediente> Expedientes { get; set; }
        public DbSet<ExpedienteDetalle> ExpedienteDetalles { get; set; }
        public DbSet<ExpedienteNotas> ExpedienteNotas { get; set; }
        public DbSet<Remitente> Remitentes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(defaultSchema);
            modelBuilder.Entity<Usuario>().ToTable("USUARIOS");
            modelBuilder.Entity<Flujo>().ToTable("FLUJOS");
            modelBuilder.Entity<Etapa>().ToTable("ETAPAS");
            modelBuilder.Entity<EtapaDetalle>().ToTable("ETAPA_DETALLES");
            modelBuilder.Entity<Campo>().ToTable("CAMPOS");
            modelBuilder.Entity<Expediente>().ToTable("EXPEDIENTES");
            modelBuilder.Entity<ExpedienteDetalle>().ToTable("EXPEDIENTE_DETALLES");
            modelBuilder.Entity<Remitente>().ToTable("REMITENTE");
            modelBuilder.Entity<ExpedienteNotas>().ToTable("EXPEDIENTE_NOTAS");

            base.OnModelCreating(modelBuilder);
        }
    }

    public class LoginDbContext : DbContext
    {
        private readonly string defaultSchema;

        public LoginDbContext(DbContextOptions<LoginDbContext> options, IConfiguration configuration) : base(options)
        {
            var configuredSchema = configuration["DatabaseSchemas:Login"];
            defaultSchema = string.IsNullOrWhiteSpace(configuredSchema)
                ? "SIA"
                : configuredSchema;
        }

        public DbSet<UsuariosNida> UsuariosNidas { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(defaultSchema);
            modelBuilder.Entity<UsuariosNida>().ToTable("USUARIOSNIDA");
            base.OnModelCreating(modelBuilder);
        }
    }
}
