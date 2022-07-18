using Ro.SQLite.Data;
using Ro.Inventario.Web.Entities;
using System.Data;
using System.Text;
using System.Linq;

namespace Ro.Inventario.Web.Repos;

public class ChangePassword
{
	public Guid Id {get; set;}
	public Guid UserId {get; set;}
	public DateTime ExpiryDate {get; set;}
	public DateTime? UsedDate {get; set;}
}

public enum ChangePasswordResult
{
	Success,	
	NotFound,
	Expired
}

public interface IChangePasswordRepo
{
    Task<ChangePassword> GetOne(Guid id);
	Task<IEnumerable<ChangePassword>> GetAll(Guid userId);
	Task<ChangePasswordResult> ChangePassword(Guid userId, string password);
	Task<int> Create(Guid id, Guid userId, DateTime expiryDate);	
    Task<int> Delete(Guid id);
}

public class ChangePasswordRepo : IChangePasswordRepo
{
	private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";
    private IDbAsync Db { get; set; }
    private IUsersRepo _usersRepo;
	private readonly ILogger<ChangePasswordRepo> _logger;
    public ChangePasswordRepo(
        IDbAsync db,
        IUsersRepo usersRepo,
        ILogger<ChangePasswordRepo> logger)
    {
		this.Db = db;
		this._logger = logger;
        this._usersRepo = usersRepo;
    }
   
	public async Task<IEnumerable<ChangePassword>> GetAll(Guid userId)
	{
		string sql = "SELECT Id,UserId,ExpiryDate,UsedDate FROM Reset_Password WHERE UsedDate is null AND UserId=@UserId;";
		var cmd = sql.ToCmd
		(
			"@UserId".ToParam(DbType.String, userId)
		);

		var list = await Db.GetRows(cmd, GetData);
		return list ?? Enumerable.Empty<ChangePassword>();
	}    

    private ChangePassword GetData(IDataReader dr)
    {
		var usedDate = dr.GetString("UsedDate");
        return new ChangePassword()
        {
            Id = Guid.Parse(dr.GetString("Id")),
            UserId = Guid.Parse(dr.GetString("UserId")),
            ExpiryDate = DateTime.Parse(dr.GetString("ExpiryDate")),
            UsedDate = string.IsNullOrWhiteSpace(usedDate) ? null : DateTime.Parse(usedDate)
        };
    }

	public Task<int> Create(Guid id, Guid userId, DateTime expiryDate)
    {
		string sql = "INSERT INTO Reset_Password (Id,UserId,ExpiryDate,UsedDate) VALUES (@Id,@UserId,@ExpiryDate,null);";		

		var cmd = sql.ToCmd(
			"@Id".ToParam(DbType.String, id.ToString()),
			"@UserId".ToParam(DbType.String, userId.ToString()),			
			"@ExpiryDate".ToParam(DbType.String, expiryDate.ToString(DATE_FORMAT))			
		);

		return Db.ExecuteNonQuery(cmd);
    }   
	
	public Task<ChangePassword> GetOne(Guid id)
	{
		string sql = "SELECT Id,UserId,ExpiryDate,UsedDate FROM Reset_Password WHERE Id=@Id;";
		var cmd = sql.ToCmd
		(
			"@Id".ToParam(DbType.String, id)
		);

		return Db.GetOneRow(cmd, GetData);
	}

	public async Task<ChangePasswordResult> ChangePassword(Guid id, string password)
	{
		var row = await GetOne(id);

		if (row == null)
		{
			_logger.LogInformation(0, "--> Change pwd request not found");
			return ChangePasswordResult.NotFound;
		}

		if (row.UsedDate.HasValue)
		{
			_logger.LogInformation(0, "--> Change pwd request expired");
			return ChangePasswordResult.Expired;
		}

		_usersRepo.CreatePasswordHash(password, out byte[] hash, out byte[] salt);
		string chngPwdSql = "UPDATE Users SET PasswordHash=@PasswordHash, PasswordSalt=@PasswordSalt WHERE Id=@Id;";
		var cmd = chngPwdSql.ToCmd
		(
			"@Id".ToParam(DbType.String, row.UserId.ToString()),
			"@PasswordHash".ToParam(DbType.Binary, hash),
			"@PasswordSalt".ToParam(DbType.Binary, salt)
		);		

		await Db.ExecuteNonQuery(cmd);
		
		_logger.LogInformation(0, "--> Changed pwd for user {id}", row.UserId);

		string updateSql = "UPDATE Reset_Password SET UsedDate=@UsedDate WHERE Id=@Id;";
		cmd = updateSql.ToCmd
		(
			"@Id".ToParam(DbType.String, id),
			"@UsedDate".ToParam(DbType.String, DateTime.Now.ToString(DATE_FORMAT))
		);

		await Db.ExecuteNonQuery(cmd);

		_logger.LogInformation(0, "--> Used changed pwd request {id}", id);
		return ChangePasswordResult.Success;
	}

    public Task<int> Delete(Guid id)
    {
		string sql = "DELETE FROM Reset_Password WHERE Id=@Id;";

		var cmd = sql.ToCmd(
			"@Id".ToParam(DbType.String, id.ToString())
		);

		return Db.ExecuteNonQuery(cmd);
    }	

}
