using System;
using System.Collections.Generic;
using LiberiaDriveMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace LiberiaDriveMVC.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cita> Cita { get; set; }

    public virtual DbSet<Cliente> Cliente { get; set; }

    public virtual DbSet<FeedbackCliente> FeedbackCliente { get; set; }

    public virtual DbSet<HorarioInstructor> HorarioInstructor { get; set; }

    public virtual DbSet<InscripcionCurso> InscripcionCurso { get; set; }

    public virtual DbSet<Instructor> Instructor { get; set; }

    public virtual DbSet<Licencia> Licencia { get; set; }

    public virtual DbSet<MantenimientoVehiculo> MantenimientoVehiculo { get; set; }

    public virtual DbSet<Pago> Pago { get; set; }

    public virtual DbSet<ResultadoExamen> ResultadoExamen { get; set; }

    public virtual DbSet<Rol> Rol { get; set; }

    public virtual DbSet<SesionPractica> SesionPractica { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    public virtual DbSet<Vehiculo> Vehiculo { get; set; }
    public virtual DbSet<TokenRecuperacion> TokenRecuperacion { get; set; }
    public virtual DbSet<InstructorLicencia> InstructorLicencias { get; set; }
    public virtual DbSet<Curso> Cursos { get; set; }
public virtual DbSet<CursoTipo> CursoTipo { get; set; } // ✅ Agregado




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cita>(entity =>
        {
            entity.HasKey(e => e.IdCita).HasName("PK__Cita__394B0202B65B302E");

            entity.ToTable(tb => tb.HasTrigger("trg_Validar_FechaCita"));

            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.TipoExamen)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Cita)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK__Cita__IdCliente__46E78A0C");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PK__Cliente__D5946642D2D216C9");

            entity.Property(e => e.Apellidos)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Contacto)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });
        modelBuilder.Entity<CursoTipo>(entity =>
{
    entity.HasKey(e => e.IdCursoTipo).HasName("PK_CursoTipo");
    entity.Property(e => e.NombreCursoTipo)
        .HasMaxLength(50)
        .IsUnicode(false);
});

       modelBuilder.Entity<Curso>(entity =>
{
    entity.HasOne(d => d.CursoTipo)
        .WithMany(p => p.Cursos)
        .HasForeignKey(d => d.IdCursoTipo)
        .HasConstraintName("FK_Curso_CursoTipo");
});


        modelBuilder.Entity<FeedbackCliente>(entity =>
        {
            entity.HasKey(e => e.IdFeedback).HasName("PK__Feedback__408FF1037AD4BB7B");

            entity.Property(e => e.Comentario)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FechaFeedback).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.FeedbackCliente)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK__FeedbackC__IdCli__5DCAEF64");

            entity.HasOne(d => d.IdCursoNavigation).WithMany(p => p.FeedbackCliente)
                .HasForeignKey(d => d.IdCurso)
                .HasConstraintName("FK__FeedbackC__IdCur__5EBF139D");
        });

        modelBuilder.Entity<HorarioInstructor>(entity =>
        {
            entity.HasKey(e => e.IdHorario).HasName("PK__HorarioI__1539229B8911E16A");

            entity.Property(e => e.DiaSemana)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Disponible).HasDefaultValue(true);

            entity.HasOne(d => d.IdInstructorNavigation).WithMany(p => p.HorarioInstructor)
                .HasForeignKey(d => d.IdInstructor)
                .HasConstraintName("FK__HorarioIn__IdIns__30F848ED");
        });

        modelBuilder.Entity<InscripcionCurso>(entity =>
        {
            entity.HasKey(e => e.IdInscripcion).HasName("PK__Inscripc__A122F2BF4C887AD7");

            entity.ToTable(tb => tb.HasTrigger("trg_Validar_InscripcionDuplicada"));

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.InscripcionCurso)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK__Inscripci__IdCli__38996AB5");

            entity.HasOne(d => d.IdCursoNavigation).WithMany(p => p.InscripcionCurso)
                .HasForeignKey(d => d.IdCurso)
                .HasConstraintName("FK__Inscripci__IdCur__398D8EEE");
        });

        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.HasKey(e => e.IdInstructor).HasName("PK__Instruct__108500446B38B4DC");

            entity.Property(e => e.Apellidos)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasMany(d => d.IdLicencia).WithMany(p => p.IdInstructor)
                .UsingEntity<Dictionary<string, object>>(
                    "InstructorLicencia",
                    r => r.HasOne<Licencia>().WithMany()
                        .HasForeignKey("IdLicencia")
                        .HasConstraintName("FK__Instructo__IdLic__2D27B809"),
                    l => l.HasOne<Instructor>().WithMany()
                        .HasForeignKey("IdInstructor")
                        .HasConstraintName("FK__Instructo__IdIns__2C3393D0"),
                    j =>
                    {
                        j.HasKey("IdInstructor", "IdLicencia").HasName("PK__Instruct__D07DD15CFF53AD52");
                    });
        });


        modelBuilder.Entity<Licencia>(entity =>
        {
            entity.HasKey(e => e.IdLicencia).HasName("PK__Licencia__0F8D118DCFD19B11");

            entity.Property(e => e.TipoLicencia)
                .HasMaxLength(20)
                .IsUnicode(false);
        });
        modelBuilder.Entity<InstructorLicencia>(entity =>
{
    entity.HasKey(e => new { e.IdInstructor, e.IdLicencia });

    entity.HasOne(e => e.Instructor)
        .WithMany(i => i.InstructorLicencias)
        .HasForeignKey(e => e.IdInstructor)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.Licencia)
        .WithMany(l => l.InstructorLicencias)
        .HasForeignKey(e => e.IdLicencia)
        .OnDelete(DeleteBehavior.Cascade);
});


        modelBuilder.Entity<MantenimientoVehiculo>(entity =>
        {
            entity.HasKey(e => e.IdMantenimiento).HasName("PK__Mantenim__DD1C44171CDF7B29");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("trg_Validar_MantenimientoDuplicado");
                    tb.HasTrigger("trg_Vehiculo_Mantenimiento");
                });

            entity.Property(e => e.Costo).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TipoMantenimiento)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.MantenimientoVehiculo)
                .HasForeignKey(d => d.IdVehiculo)
                .HasConstraintName("FK__Mantenimi__IdVeh__59063A47");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("PK__Pago__FC851A3A961897AE");

            entity.ToTable(tb => tb.HasTrigger("trg_Validar_Pago_SinInscripcion"));

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Completado");
            entity.Property(e => e.Monto).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TipoPago)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdInscripcionNavigation).WithMany(p => p.Pago)
                .HasForeignKey(d => d.IdInscripcion)
                .HasConstraintName("FK__Pago__IdInscripc__3D5E1FD2");
        });

        modelBuilder.Entity<ResultadoExamen>(entity =>
        {
            entity.HasKey(e => e.IdResultado).HasName("PK__Resultad__DAF71D0BA47D835B");

            entity.Property(e => e.TipoExamen)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.ResultadoExamen)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK__Resultado__IdCli__5535A963");

            entity.HasOne(d => d.IdInstructorNavigation).WithMany(p => p.ResultadoExamen)
                .HasForeignKey(d => d.IdInstructor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Resultado__IdIns__5629CD9C");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Rol__2A49584C0917920D");

            entity.HasIndex(e => e.NombreRol, "UQ__Rol__4F0B537F8C8CA59D").IsUnique();

            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NombreRol)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SesionPractica>(entity =>
        {
            entity.HasKey(e => e.IdSesionPractica).HasName("PK__SesionPr__8FF1ECA89E45991B");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("trg_Cliente_Estado");
                    tb.HasTrigger("trg_Instructor_Disponibilidad");
                    tb.HasTrigger("trg_Validar_Instructor_Disponible");
                    tb.HasTrigger("trg_Validar_Vehiculo_SesionDuplicada");
                    tb.HasTrigger("trg_Vehiculo_Estado");
                });

            entity.Property(e => e.Calificacion).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Programada");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.SesionPractica)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK__SesionPra__IdCli__412EB0B6");

            entity.HasOne(d => d.IdInstructorNavigation).WithMany(p => p.SesionPractica)
                .HasForeignKey(d => d.IdInstructor)
                .HasConstraintName("FK__SesionPra__IdIns__4222D4EF");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.SesionPractica)
                .HasForeignKey(d => d.IdVehiculo)
                .HasConstraintName("FK__SesionPra__IdVeh__4316F928");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__5B65BF978A758C23");

            entity.HasIndex(e => e.Correo, "UQ__Usuario__60695A197F7C7E17").IsUnique();

            entity.HasIndex(e => e.NombreUsuario, "UQ__Usuario__6B0F5AE0E5369341").IsUnique();

            entity.Property(e => e.ContrasenaHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Usuario__IdClien__5165187F");

            entity.HasOne(d => d.IdInstructorNavigation).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.IdInstructor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Usuario__IdInstr__52593CB8");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__Usuario__IdRol__5070F446");
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.IdVehiculo).HasName("PK__Vehiculo__7086121580FF7183");

            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Modelo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Transmision)
                .HasMaxLength(20)
                .IsUnicode(false);
        });
modelBuilder.Entity<TokenRecuperacion>(entity =>
{
    entity.HasKey(e => e.IdToken).HasName("PK__TokenRecuperacion");

    entity.Property(e => e.Correo)
        .HasMaxLength(100)
        .IsUnicode(false)
        .IsRequired();

    entity.Property(e => e.Token)
        .HasMaxLength(255)
        .IsUnicode(false)
        .IsRequired();

    entity.Property(e => e.FechaExpira)
        .HasColumnType("datetime")
        .IsRequired();

    entity.Property(e => e.Usado)
        .HasDefaultValue(false);
});

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
