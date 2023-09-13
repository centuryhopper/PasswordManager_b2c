
namespace password_manager_b2c.Client.Auth;

public interface IAuthService
{
    void EstablishClaimsFromAAD();
    void GetClaimsFromAADJwt();
    void GetJwtFromAAD();
}
