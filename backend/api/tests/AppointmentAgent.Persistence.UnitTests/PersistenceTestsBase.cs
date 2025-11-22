using Microsoft.EntityFrameworkCore;

namespace AppointmentAgent.Persistence.UnitTests;

public class PersistenceTestsBase
{
    protected ApplicationDbContext SetupDbContext { get; private set; } = null!;
    protected ApplicationDbContext ArrangeDbContext { get; private set; } = null!;
    protected ApplicationDbContext AssertDbContext { get; private set; } = null!;
    
    protected CancellationToken CancellationToken = CancellationToken.None;
    
    private DbContextOptions<ApplicationDbContext> _options = null!;
    
    [SetUp]
    public void SetUp()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"TestDbContext_{Guid.NewGuid()}")
            .Options;

        SetupDbContext = new ApplicationDbContext(_options);
        ArrangeDbContext = new ApplicationDbContext(_options);
        AssertDbContext = new ApplicationDbContext(_options);
    }

    [TearDown]
    public void TearDown()
    {
        SetupDbContext.Dispose();
        ArrangeDbContext.Dispose();
        AssertDbContext.Dispose();
    }
}