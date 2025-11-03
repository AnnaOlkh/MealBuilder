using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

public class AuthController : Controller
{
    [HttpGet]
    public IActionResult Login(string returnUrl = "/")
    {
        var props = new AuthenticationProperties
        {
            RedirectUri = Url.Action("ExternalLoginCallback", "Auth", new { returnUrl })
        };

        return Challenge(props);
    }

    [HttpGet]
    public IActionResult ExternalLoginCallback(string returnUrl = "/")
    {
        if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            return RedirectToAction("Index", "Home");

        return Redirect(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}
