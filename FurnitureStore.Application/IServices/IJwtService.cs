using FurnitureStore.Application.DTOs;
using FurnitureStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Application.IServices
{
    public interface IJwtService
    {
        (string Token, DateTime Expiration) GenerateJwtToken(User user, IList<string> roles);
    }
}
