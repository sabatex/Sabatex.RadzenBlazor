using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor.Services;

public interface ISabatexUserManager
{
    /// <summary>
    /// Read all roles in application
    /// </summary>
    /// <returns></returns>
    IEnumerable<string> GetAvaliableRoles();
}
