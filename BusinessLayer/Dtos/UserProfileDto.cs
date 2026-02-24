using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Dtos
{
    public record UserProfileDto(
     string Id,
     string Username,
     string FirstName,
     string LastName,
     List<string> Roles
    );
}
