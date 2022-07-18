using Ro.SQLite.Data;
using Ro.Inventario.Web.Entities;
using System.Data;
using System.Text;
using System.Linq;
using System.Security.Cryptography;

namespace Ro.Inventario.Web.Repos;

public interface IUsersRepo
{
    Task<User> GetOne(string email);
	Task<User> GetOne(Guid id);
	Task<IEnumerable<User>> GetAll();
	Task<int> Create(Register model);
	Task<int> ToggleActive(Guid userId);
    Task<int> Delete(Guid id);
    Task<bool> HasAccess(Login model);

	void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
	bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
}

public class UsersRepo : IUsersRepo
{
	private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";
    private IDbAsync Db { get; set; }
	private readonly ILogger<UsersRepo> _logger;
    public UsersRepo(IDbAsync db, ILogger<UsersRepo> logger)
    {
		this.Db = db;
		this._logger = logger;
    }

    public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
		using (var hmac = new HMACSHA512())
		{
			passwordSalt = hmac.Key;
			passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
		}
    }

    public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
		using (var hmac = new HMACSHA512(passwordSalt))
		{
			var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			return computedHash.SequenceEqual(passwordHash);
		}
    }

    public Task<User> GetOne(string email)
    {
		string sql = "SELECT Id,Email,DateCreated,IsActive FROM Users WHERE Email=@Email;";
		var cmd = sql.ToCmd
		(
			"@Email".ToParam(DbType.String, email)
		);

		return Db.GetOneRow(cmd, GetData);
    }

	 public Task<User> GetOne(Guid id)
    {
		string sql = "SELECT Id,Email,DateCreated,IsActive FROM Users WHERE Id=@Id;";
		var cmd = sql.ToCmd
		(
			"@Id".ToParam(DbType.String, id.ToString())
		);

		return Db.GetOneRow(cmd, GetData);
    }

	public Task<IEnumerable<User>> GetAll()
	{
		string sql = "SELECT Id,Email,DateCreated,IsActive FROM Users;";
		var cmd = sql.ToCmd();

		return Db.GetRows(cmd, GetData);
	}

    public async Task<bool> HasAccess(Login model)
    {
		var user = await GetOne(model.Email);
		if (user == null)
		{
			_logger.LogInformation(0, "--> User not found {email}", model.Email);
			return false;
		}
		if (user.IsActive == false)
		{
			_logger.LogInformation(0, "--> User not active {email}", model.Email);
			return false;
		}

		string sql = "SELECT PasswordHash, PasswordSalt FROM Users WHERE Id=@Id";
		var cmd = sql.ToCmd("@Id".ToParam(DbType.String, user.Id.ToString()));
		var tuple = await Db.GetOneRow(cmd, (dr) => {
			(byte[] hash, byte[] salt) t =
			(
				(byte[])dr["PasswordHash"],
				(byte[])dr["PasswordSalt"]
			);
			return t;
		});

		return VerifyPasswordHash(model.Password, tuple.hash, tuple.salt);
    }

    private User GetData(IDataReader dr)
    {
		return new User()
		{
			Id = Guid.Parse(dr.GetString("Id")),
			Email = dr.GetString("Email"),
			IsActive = dr.GetInt("IsActive") == 1
		};
    }

    public Task<int> Create(Register model)
    {
		string sql = "INSERT INTO Users (Id,Email,IsActive,PasswordHash,PasswordSalt,DateCreated) VALUES (@Id,@Email,@IsActive,@PasswordHash,@PasswordSalt,@DateCreated);";
		DateTime stamp = DateTime.Now;
		
		CreatePasswordHash(model.Password, out byte[] hash, out byte[] salt);

		var cmd = sql.ToCmd(
			"@Id".ToParam(DbType.String, model.Id.ToString()),
			"@Email".ToParam(DbType.String, model.Email),
			"@IsActive".ToParam(DbType.Int32, 0),
			"@PasswordHash".ToParam(DbType.Binary, hash),
			"@PasswordSalt".ToParam(DbType.Binary, salt),
			"@DateCreated".ToParam(DbType.String, stamp.ToString(DATE_FORMAT))
		);

		return Db.ExecuteNonQuery(cmd);
    }	

    public Task<int> Delete(Guid id)
    {
		string sql = "DELETE FROM Users WHERE Id=@Id;";

		var cmd = sql.ToCmd(
			"@Id".ToParam(DbType.String, id.ToString())
		);

		return Db.ExecuteNonQuery(cmd);
    }

	public Task<int> ToggleActive(Guid userId)
    {
		string sql = "UPDATE Users SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE Id=@Id;";

		var cmd = sql.ToCmd(
			"@Id".ToParam(DbType.String, userId.ToString())
		);

		return Db.ExecuteNonQuery(cmd);
    }
}