using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace PruebaEFMariaDB.Models
{
    public partial class MiDbContext : DbContext
    {
        public MiDbContext()
        {
        }

        public MiDbContext(DbContextOptions<MiDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AuthHashAlgo> AuthHashAlgos { get; set; }
        public virtual DbSet<AuthPath> AuthPaths { get; set; }
        public virtual DbSet<AuthRole> AuthRoles { get; set; }
        public virtual DbSet<AuthRolePath> AuthRolePaths { get; set; }
        public virtual DbSet<AuthUser> AuthUsers { get; set; }
        public virtual DbSet<AuthUserRole> AuthUserRoles { get; set; }
        public virtual DbSet<VwAuthUser> VwAuthUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=localhost;port=3306;user=root;password=32marias;database=dbprueba", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.5.10-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            modelBuilder.Entity<AuthHashAlgo>(entity =>
            {
                entity.ToTable("auth_hash_algos");

                entity.HasIndex(e => e.Description, "description")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("description");
            });

            modelBuilder.Entity<AuthPath>(entity =>
            {
                entity.ToTable("auth_paths");

                entity.HasIndex(e => e.Description, "description")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(2048)
                    .HasColumnName("description");
            });

            modelBuilder.Entity<AuthRole>(entity =>
            {
                entity.ToTable("auth_roles");

                entity.HasComment("a table representing distinct roles for authenticacion and authorization");

                entity.HasIndex(e => e.Description, "description")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("description");
            });

            modelBuilder.Entity<AuthRolePath>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("auth_role_paths");

                entity.HasIndex(e => e.PathId, "FK__auth_paths");

                entity.HasIndex(e => e.RoleId, "roleId");

                entity.HasIndex(e => new { e.RoleId, e.PathId }, "roleId_pathId")
                    .IsUnique();

                entity.Property(e => e.PathId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("pathId");

                entity.Property(e => e.RoleId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("roleId");

                entity.HasOne(d => d.Path)
                    .WithMany()
                    .HasForeignKey(d => d.PathId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__auth_paths");

                entity.HasOne(d => d.Role)
                    .WithMany()
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__auth_roles");
            });

            modelBuilder.Entity<AuthUser>(entity =>
            {
                entity.ToTable("auth_users");

                entity.HasIndex(e => e.HashAlgoId, "FK_auth_users_auth_hash_algos");

                entity.HasIndex(e => e.Name, "name")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.HashAlgoId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("hashAlgoId");

                entity.Property(e => e.HashPwd)
                    .IsRequired()
                    .HasMaxLength(1024)
                    .HasColumnName("hashPwd");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.HasOne(d => d.HashAlgo)
                    .WithMany(p => p.AuthUsers)
                    .HasForeignKey(d => d.HashAlgoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_auth_users_auth_hash_algos");
            });

            modelBuilder.Entity<AuthUserRole>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("auth_user_roles");

                entity.HasIndex(e => e.RoleId, "FK_auth_user_roles_auth_roles");

                entity.HasIndex(e => new { e.UserId, e.RoleId }, "userId_roleId")
                    .IsUnique();

                entity.Property(e => e.RoleId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("roleId");

                entity.Property(e => e.UserId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("userId");

                entity.HasOne(d => d.Role)
                    .WithMany()
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_auth_user_roles_auth_roles");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_auth_user_roles_auth_users");
            });

            modelBuilder.Entity<VwAuthUser>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_auth_users");

                entity.Property(e => e.Hash)
                    .IsRequired()
                    .HasMaxLength(1024)
                    .HasColumnName("hash");

                entity.Property(e => e.HashAlgo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("hashAlgo");

                entity.Property(e => e.HashAlgoId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("hashAlgoId");

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(2048)
                    .HasColumnName("path");

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("role");

                entity.Property(e => e.RoleId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("roleId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
