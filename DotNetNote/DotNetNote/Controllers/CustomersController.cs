﻿using Microsoft.AspNetCore.Cors;

namespace DotNetNote.Controllers;

[Route("api/[controller]")]
[EnableCors("AllowAnyOrigin")]
public class CustomersController : Controller
{
    [HttpGet]
    public IEnumerable<Customer> Get() => [
            new Customer { Id = 1, Name = "김태영", City = "서울" },
            new Customer { Id = 2, Name = "박용준", City = "인천" },
            new Customer { Id = 3, Name = "한상훈", City = "경기" }
        ];
}
