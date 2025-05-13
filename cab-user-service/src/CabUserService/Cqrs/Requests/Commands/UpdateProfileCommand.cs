using MediatR;

namespace CabUserService.Cqrs.Requests.Commands;

public class UpdateProfileCommand : IRequest<bool>
{
    #region Properties

    public Guid UserId { get; set; }

    /// <summary>
    ///null: unknown
    ///0: male
    ///1: female
    ///2: lgbt
    /// </summary>
    public int? SexualOrientation { get; set; }

    /// <summary>
    ///null: unknown
    ///0: marry
    ///1: not marry
    /// </summary>
    public int? Marry { get; set; }

    public List<Guid>? CategoryFavorites { get; set; }

    public List<string> Schools { get; set; }

    public List<string> Companys { get; set; }

    public string HomeLand { get; set; }

    public bool IsShowSexualOrientation { get; set; }

    public bool IsShowMarry { get; set; }

    public bool IsShowSchool { get; set; }

    public bool IsShowCompany { get; set; }

    public bool IsShowHomeLand { get; set; }

    public string IsShowDob { get; set; }

    public bool IsShowEmail { get; set; }

    public bool IsShowPhone { get; set; }

    public bool IsShowCity { get; set; }

    public bool IsShowDescription { get; set; }

    #endregion
}
