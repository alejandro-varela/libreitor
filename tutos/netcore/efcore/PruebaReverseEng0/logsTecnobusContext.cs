using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace PruebaReverseEng0
{
    public partial class logsTecnobusContext : DbContext
    {
        private readonly IConfiguration _config;
        
        public logsTecnobusContext(IConfiguration config)
        {
            _config = config;
        }

        public logsTecnobusContext(DbContextOptions<logsTecnobusContext> options, IConfiguration config)
            : base(options)
        {
            _config = config;
        }

        public virtual DbSet<LogGpsSinCalculoAtrasoYexcesoVelocidad> LogGpsSinCalculoAtrasoYexcesoVelocidads { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
 //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                 //optionsBuilder.UseSqlServer("Data Source=db-tecnobus;Initial Catalog=logsTecnobus;Integrated Security=True");              
                 string connString = _config["MainConnString"];
                 optionsBuilder.UseSqlServer(connString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Modern_Spanish_CI_AS");

            modelBuilder.Entity<LogGpsSinCalculoAtrasoYexcesoVelocidad>(entity =>
            {
                entity.HasKey(e => e.Uid);

                entity.ToTable("log_gpsSinCalculoAtrasoYExcesoVelocidad");

                entity.HasIndex(e => new { e.NroEquipo, e.FechaHora }, "UK_EquipoRegistroFecha")
                    .IsUnique();

                entity.HasIndex(e => new { e.CodLinea, e.FechaHora }, "linea,fecha");

                entity.Property(e => e.Uid).HasColumnName("uid");

                entity.Property(e => e.CodBandera).HasColumnName("cod_bandera");

                entity.Property(e => e.CodLinea).HasColumnName("cod_linea");

                entity.Property(e => e.CodTipoDia).HasColumnName("cod_tipoDia");

                entity.Property(e => e.EstadoCoche).HasColumnName("estadoCoche");

                entity.Property(e => e.FechaHora)
                    .HasColumnType("datetime")
                    .HasColumnName("fechaHora");

                entity.Property(e => e.FechaInsert)
                    .HasColumnType("datetime")
                    .HasColumnName("fechaInsert")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IdGrupo).HasColumnName("id_grupo");

                entity.Property(e => e.IdRegistro).HasColumnName("id_registro");

                entity.Property(e => e.Latitud).HasColumnName("latitud");

                entity.Property(e => e.Longitud).HasColumnName("longitud");

                entity.Property(e => e.MetrosAcumulados).HasColumnName("metrosAcumulados");

                entity.Property(e => e.NroEquipo).HasColumnName("nro_equipo");

                entity.Property(e => e.NroFicha).HasColumnName("nro_ficha");

                entity.Property(e => e.NroInterno)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("nro_interno")
                    .IsFixedLength(true);

                entity.Property(e => e.NroLegajo)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("nro_legajo");

                entity.Property(e => e.Servicio).HasColumnName("servicio");

                entity.Property(e => e.Velocidad)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("velocidad");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
