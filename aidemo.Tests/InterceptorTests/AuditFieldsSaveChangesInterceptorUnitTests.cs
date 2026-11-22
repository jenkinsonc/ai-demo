using System;
using System.Threading.Tasks;
using aidemo.Integrations.Interceptors;
using aidemo.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace aidemo.Tests.InterceptorTests
{
    [TestClass]
    public sealed class AuditFieldsSaveChangesInterceptorUnitTests
    {
        private static readonly DateTimeOffset Earlier = new(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public TestContext TestContext { get; set; }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task Saving_An_Added_Entity_Sets_CreatedDate_And_LastUpdatedDate_To_Now(bool useSaveChangesAsync)
        {
            await using var context = CreateContext();
            var entity = new AuditedEntity { Name = "new" };
            context.Add(entity);

            var before = DateTimeOffset.Now;
            await SaveAsync(context, useSaveChangesAsync);
            var after = DateTimeOffset.Now;

            Assert.IsInRange(before, after, entity.CreatedDate);
            Assert.AreEqual(entity.CreatedDate, entity.LastUpdatedDate);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task Saving_A_Modified_Entity_Sets_Only_LastUpdatedDate(bool useSaveChangesAsync)
        {
            await using var context = CreateContext();
            var entity = await SeedAsync(context);
            entity.Name = "changed";

            var before = DateTimeOffset.Now;
            await SaveAsync(context, useSaveChangesAsync);
            var after = DateTimeOffset.Now;

            Assert.AreEqual(Earlier, entity.CreatedDate);
            Assert.IsInRange(before, after, entity.LastUpdatedDate);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task Saving_An_Unchanged_Entity_Leaves_Both_Dates_Alone(bool useSaveChangesAsync)
        {
            await using var context = CreateContext();
            var entity = await SeedAsync(context);

            await SaveAsync(context, useSaveChangesAsync);

            Assert.AreEqual(Earlier, entity.CreatedDate);
            Assert.AreEqual(Earlier, entity.LastUpdatedDate);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task Saving_An_Entity_Without_Both_Audit_Fields_Leaves_It_Alone(bool useSaveChangesAsync)
        {
            await using var context = CreateContext();
            var entity = new PartlyAuditedEntity();
            context.Add(entity);

            await SaveAsync(context, useSaveChangesAsync);

            Assert.AreEqual(default, entity.CreatedDate);
        }

        // Saves an entity, then backdates both audit fields and accepts the change, so it is tracked as Unchanged with known dates.
        private async Task<AuditedEntity> SeedAsync(AuditTestDbContext context)
        {
            var entity = new AuditedEntity { Name = "seed" };
            context.Add(entity);
            await context.SaveChangesAsync(TestContext.CancellationToken);
            entity.CreatedDate = Earlier;
            entity.LastUpdatedDate = Earlier;
            context.ChangeTracker.DetectChanges();
            context.ChangeTracker.AcceptAllChanges();

            return entity;
        }

        private async Task SaveAsync(AuditTestDbContext context, bool useSaveChangesAsync)
        {
            if (useSaveChangesAsync)
                await context.SaveChangesAsync(TestContext.CancellationToken);
            else
                context.SaveChanges();
        }

        private static AuditTestDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AuditTestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .AddInterceptors(new AuditFieldsSaveChangesInterceptor())
                .Options;

            return new AuditTestDbContext(options);
        }

        private sealed class AuditedEntity : BaseEntity
        {
            public string Name { get; set; }
        }

        // Has CreatedDate but no LastUpdatedDate, so the interceptor must skip it.
        private sealed class PartlyAuditedEntity
        {
            public int Id { get; set; }
            public DateTimeOffset CreatedDate { get; set; }
        }

        private sealed class AuditTestDbContext(DbContextOptions<AuditTestDbContext> options) : DbContext(options)
        {
            public DbSet<AuditedEntity> AuditedEntities { get; set; }
            public DbSet<PartlyAuditedEntity> PartlyAuditedEntities { get; set; }
        }
    }
}
