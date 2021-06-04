using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using WebApi.Entities;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    string _roles = null;
    public AuthorizeAttribute(string roles) : base()
    {
        _roles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = (User)context.HttpContext.Items["User"];

        if (ValidateUser(user))
        {
            // aca podríamos loguear o avisar algo...
            // se autoriza el uso de "EL RECURSO X" para "LE USUARIE X"
        }
        else
        {
            // no logueado o no autorizado para ese recurso...
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }       
    }

    private bool ValidateUser(User user)
    {
        if (user == null)
        {
            return false;
        }

        var rolesAux = _roles;

        // verifico si el usuario tiene esos roles asignados...
        return true;
    }
}
