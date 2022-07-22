using Ro.Inventario.Web.Repos;
using Ro.Inventario.Web.Entities;
using Ro.SQLite.Data;
using System.Data.Common;
using System.Data;
using Xunit.Abstractions;

namespace Ro.Inventario.Web.Tests;

public class SettinsRepoTests
{
    private readonly ITestOutputHelper output;
    public SettinsRepoTests(ITestOutputHelper output)
    {
        this.output = output;
    }
    
    [Fact]
    public async Task GetValue()
    {
        var s = new Setting()
        {
            Key = "IVA",
            Value = "0"
        };

        var mDb = new Mock<IDbAsync>();
        mDb.Setup(x => x.GetOneRow(
            It.IsAny<DbCommand>(),
            It.IsAny<Func<IDataReader, Setting>>()
        )).ReturnsAsync(s);

        var sut = new SettingsRepo(mDb.Object);        

        var actual = await sut.GetValue(s.Key, (value) => {
            return decimal.Parse(value);
        });

        Assert.Equal(decimal.Zero, actual);
    }    

    [Fact]
    public async Task Integration_Settings()
    {
        var dbSetup = new DatabaseProvider();
        await dbSetup.InitDb();

        var s = new Setting()
        {
            Key = "IVA",
            Value = "0.16"
        };

        var sut = new SettingsRepo(dbSetup.GetDb());

        var actual = await sut.GetValue(s.Key, (value) => {
            Assert.Equal(s.Value, value);
            return decimal.Parse(value);
        });

        Assert.Equal(decimal.Parse(s.Value), actual);
        
    }    
}