using Moq;
using Ro.Inventario.Web.Repos;
using Ro.Inventario.Web.Entities;
using Ro.SQLite.Data;
using Xunit;
using System.Data.Common;
using System.Data;

namespace Ro.Inventario.Web.Tests;

public class SettinsRepoTests
{
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
}