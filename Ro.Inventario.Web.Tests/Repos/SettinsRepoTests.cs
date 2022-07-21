using Moq;
using Ro.Inventario.Web.Repos;
using Ro.Inventario.Web.Entities;
using Ro.SQLite.Data;
using Xunit;

namespace Ro.Inventario.Web.Tests;
// https://github.com/xunit/assert.xunit
public class SettinsRepoTests
{
    [Fact]
    public async Task GetValue()
    {
        var mock = new Mock<IDbAsync>();

        var sut = new SettingsRepo(mock.Object);

        var actual = await sut.GetValue("IVA", (value) => {
            return decimal.Parse(value);
        });

        Assert.Equal(decimal.Zero, actual);
    }
}