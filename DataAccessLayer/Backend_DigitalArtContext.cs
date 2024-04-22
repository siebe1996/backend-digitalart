using Globals.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class Backend_DigitalArtContext : IdentityDbContext<
        User, 
        Role, 
        Guid, 
        IdentityUserClaim<Guid>, 
        UserRole, 
        IdentityUserLogin<Guid>, 
        IdentityRoleClaim<Guid>, 
        IdentityUserToken<Guid>
        >
    {
        public Backend_DigitalArtContext() { }

        public Backend_DigitalArtContext(DbContextOptions<Backend_DigitalArtContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles {  get; set; }
        public DbSet<UserRole> UserRoles {  get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Artpiece> Artpieces { get; set; }
        public DbSet<ArtpieceCategory> ArtpieceCategories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Exhibitor> Exhibitors { get; set; }
        public DbSet<Exposition> Expositions{ get; set; }
        public DbSet<ExpositionArtpiece> ExpositionArtpieces { get; set; }
        public DbSet<Place> Places {  get; set; }
        public DbSet<Projector> Projectors { get; set; }
        public DbSet<RentalAgreement> RentalAgreements { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.Email)
                .IsUnique();

                // Each User can have many UserClaims
                entity.HasMany(e => e.Claims)
                    .WithOne()
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                // Each User can have many UserLogins
                entity.HasMany(e => e.Logins)
                    .WithOne()
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                // Each User can have many UserTokens
                entity.HasMany(e => e.Tokens)
                    .WithOne()
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                // Each User can have many entries in the UserRole join table
                entity.HasMany(e => e.UserRoles)
                    .WithOne()
                    .HasForeignKey(ur => ur.UserId)
                .IsRequired();

                entity.Property(e => e.ImageData)
                .HasColumnType("longblob");
            });

            builder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
            });

            builder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne(x => x.User)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Artist>(entity =>
            {
                entity.HasMany(x => x.Artpieces)
                .WithOne()
                .HasForeignKey(x => x.ArtistId)
                .IsRequired();
            });

            builder.Entity<Artpiece>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(x => x.Artist)
                .WithMany(x => x.Artpieces)
                .HasForeignKey(x => x.ArtistId)
                .IsRequired();

                entity.HasMany(x => x.ExpositionArtpieces)
                .WithOne(x => x.Artpiece)
                .HasForeignKey(x => x.ArtpieceId)
                .IsRequired();

                entity.HasMany(x => x.ArtpieceCategories)
                .WithOne(x => x.Artpiece)
                .HasForeignKey(x => x.ArtpieceId)
                .IsRequired();
            });

            builder.Entity<ArtpieceCategory>(entity =>
            {
                entity.HasKey(x => new { x.ArtpieceId, x.CategoryId });

                entity.HasOne(x => x.Category)
                .WithMany(x => x.ArtpieceCategories)
                .HasForeignKey(x => x.CategoryId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Artpiece)
                .WithMany(x => x.ArtpieceCategories)
                .HasForeignKey(x => x.ArtpieceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Category>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasMany(x => x.ArtpieceCategories)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId)
                .IsRequired();
            });

            builder.Entity<Exhibitor>(entity =>
            {
                entity.HasOne(x => x.Place)
                .WithMany(x => x.Exhibitors)
                .HasForeignKey(x => x.PlaceId)
                .IsRequired();
            });

            builder.Entity<Exposition>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasMany(x => x.Projectors)
                .WithOne(x => x.Exposition)
                .HasForeignKey(x => x.ExpositionId)
                .IsRequired();

                entity.HasMany(x => x.ExpositionArtpieces)
                .WithOne(x => x.Exposition)
                .HasForeignKey(x => x.ExpositionId)
                .IsRequired();
            });

            builder.Entity<ExpositionArtpiece>(entity =>
            {
                entity.HasKey(x => new { x.ExpositionId, x.ArtpieceId });

                entity.HasOne(x => x.Exposition)
                .WithMany(x => x.ExpositionArtpieces)
                .HasForeignKey(x => x.ExpositionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Artpiece)
                .WithMany(x => x.ExpositionArtpieces)
                .HasForeignKey(x => x.ArtpieceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Place>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasMany(x => x.Exhibitors)
                .WithOne(x => x.Place)
                .HasForeignKey(x => x.PlaceId)
                .IsRequired();

                entity.HasMany(x => x.RentalAgreements)
                .WithOne(x => x.Place)
                .HasForeignKey(x => x.PlaceId)
                .IsRequired();
            });

            builder.Entity<Projector>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasMany(x => x.RentalAgreements)
                .WithOne(x => x.Projector)
                .HasForeignKey(x => x.ProjectorId)
                .IsRequired();

                entity.HasOne(x => x.Exposition)
                .WithMany(x => x.Projectors)
                .HasForeignKey(x => x.ExpositionId)
                .IsRequired();
            });

            builder.Entity<RentalAgreement>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.Place)
                .WithMany(x => x.RentalAgreements)
                .HasForeignKey(x => x.PlaceId)
                .IsRequired();

                entity.HasOne(x => x.Projector)
                .WithMany(x => x.RentalAgreements)
                .HasForeignKey(x => x.ProjectorId)
                .IsRequired();
            });

            builder.Entity<Like>(entity =>
            {
                entity.HasKey(x => new { x.ArtpieceId, x.UserId });

                entity.HasOne(x => x.Artpiece)
                .WithMany(x => x.Likes)
                .HasForeignKey(x => x.ArtpieceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.User)
                .WithMany(x => x.Likes)
                .HasForeignKey(x => x.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
