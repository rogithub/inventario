using Ro.SQLite.Data;
using Ro.Inventario.Web.Entities;
using System.Data;
using System.Text;
using System.Linq;

namespace Ro.Inventario.Web.Repos;

public interface IRolesRepo
{
	Task<IEnumerable<string>> GetRoles(Guid userId);
	Task<bool> RoleExists(string role);
    Task<IDictionary<string, Guid>> GetRoles();
    Task<int> AddToRole(Guid id, Guid userId, Guid roleId);
	Task<int> AddToRole(Guid id, Guid userId, string role);
}

public class RolesRepo : IRolesRepo
{
	private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";
    private IDbAsync Db { get; set; }
	private readonly ILogger<RolesRepo> _logger;
    public RolesRepo(IDbAsync db, ILogger<RolesRepo> logger)
    {
		this.Db = db;
		this._logger = logger;
    }

    public Task<IEnumerable<string>> GetRoles(Guid userId)
	{
		string sql = "SELECT r.Role FROM User_Roles ur JOIN Roles r ON ur.RoleId = r.Id WHERE ur.UserId=@Id;";
		var cmd = sql.ToCmd
		(
			"@Id".ToParam(DbType.String, userId)
		);

		return Db.GetRows(cmd, dr => {
			return dr.GetString("Role");
		});
	}

    public async Task<IDictionary<string, Guid>> GetRoles()
	{
		string sql = "SELECT Id, Role FROM Roles;";
		var cmd = sql.ToCmd();
        var d = new Dictionary<string, Guid>();

		await Db.GetRows(cmd, dr => {
			d.Add
            (
				dr.GetString("Role"),
                Guid.Parse(dr.GetString("Id"))                
            );
			return 0;
		});

		return d;
	}

    public Task<int> AddToRole(Guid id, Guid userId, Guid roleId)
	{
		string sql = "INSERT INTO User_Roles (Id,RoleId,UserId) VALUES (@Id,@RoleId,@UserId);";
		var cmd = sql.ToCmd(
			"@Id".ToParam(DbType.String, id.ToString()),
			"@RoleId".ToParam(DbType.String, roleId.ToString()),
			"@UserId".ToParam(DbType.String, userId.ToString())
		);

		return Db.ExecuteNonQuery(cmd);
	}

	public Task<int> AddToRole(Guid id, Guid userId, string role)
	{
		string sql = "INSERT INTO User_Roles (Id,RoleId,UserId) SELECT @Id,r.Id,@UserId FROM Roles r WHERE Role=@role;";
		var cmd = sql.ToCmd(
			"@Id".ToParam(DbType.String, id.ToString()),			
			"@UserId".ToParam(DbType.String, userId.ToString()),
			"@role".ToParam(DbType.String, role)
		);

		return Db.ExecuteNonQuery(cmd);
	}

	public async Task<bool> RoleExists(string role)
	{
		string sql = "SELECT count(*) FROM Roles WHERE Role=@role;";
		var cmd = sql.ToCmd(			
			"@role".ToParam(DbType.String, role)
		);

		var count = await Db.ExecuteScalar(cmd);
		return ((int)count) > 0;
	}
}
