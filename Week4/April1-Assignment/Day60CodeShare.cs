
Now first copy the code which is there earlir into Day60 folder where we haev created identity package tables by running migrations 

Now follow these steps now further 

step 6:
---------
add another class Response in Models folder okay ...
    
public class Response
{
public string? Status { get; set; }
public string? Message { get; set; }
}

I will use above class properties to define status and message 

Next add AuthenticationController of web api type and it should be empty one 
-------------------------------------------------------------------------------
using CodeFirstEFDEmo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CodeFirstEFDEmo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;

            _configuration = configuration;
        }


        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser, string role)
        {
            //Check User Exist 
            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);
            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response { Status = "Error", Message = "User already exists!" });
            }

            //Add the User in the database
            IdentityUser user = new()
            {
                Email = registerUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUser.Username

            };
            if (await _roleManager.RoleExistsAsync(role))
            {
                var result = await _userManager.CreateAsync(user, registerUser.Password);
                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "User Failed to Create" });
                }
                //Add role to the user....

                await _userManager.AddToRoleAsync(user, role);





                return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = "Success", Message = $"User created SuccessFully" });

            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "This Role Doesnot Exist." });
            }


        }

    }
}

once build the solution and then
Run the web api an insert one value of the user

/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [Id]
      ,[UserName]
      ,[NormalizedUserName]
      ,[Email]
      ,[NormalizedEmail]
      ,[EmailConfirmed]
      ,[PasswordHash]
      ,[SecurityStamp]
      ,[ConcurrencyStamp]
      ,[PhoneNumber]
      ,[PhoneNumberConfirmed]
      ,[TwoFactorEnabled]
      ,[LockoutEnd]
      ,[LockoutEnabled]
      ,[AccessFailedCount]
  FROM [EventsDB].[dbo].[AspNetUsers]

step 7 :
---------
Now i need to work on login method ..
    
Now first add LoginModel class into the folder of models 

using System.ComponentModel.DataAnnotations;

namespace CodeFirstEFDEmo.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

    }
}

and then go to app settings and write the JWT ..code here


{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "ConnStr": "server=LAPTOP-4G8BHPK9\\SQLEXPRESS;database=TaokenDB;Trusted_Connection=true"
  },  
  "JWT": {
    "ValidAudience": "https://localhost:3000",
    "ValidIssuer": "https://localhost:7113",
    "Secret": "JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyrsss"
  }
}

so here 3000 is external frontend of react which is trying to access our web api ..and 7264 is
the URL used in profiles of launch settings https one only use it of properties but here in our case both urls are same only 


so changing it like this 

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "constring": "Data Source=LAPTOP-4G8BHPK9\\SQLEXPRESS;initial catalog=EventsDB;Integrated Security=true;Encrypt=true;TrustServerCertificate=true;"
  },
  "JWT": {
    "ValidAudience": "https://localhost:7267",
    "ValidIssuer": "https://localhost:7267",
    "Secret": "JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyrsss"
  }
}



step 8:
----------
Now further code of Authentication Controller 

using CodeFirstEFDEmo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CodeFirstEFDEmo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;

            _configuration = configuration;
        }


        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser, string role)
        {
            //Check User Exist 
            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);
            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response { Status = "Error", Message = "User already exists!" });
            }

            //Add the User in the database
            IdentityUser user = new()
            {
                Email = registerUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUser.Username

            };
            if (await _roleManager.RoleExistsAsync(role))
            {
                var result = await _userManager.CreateAsync(user, registerUser.Password);
                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "User Failed to Create" });
                }
                //Add role to the user....

                await _userManager.AddToRoleAsync(user, role);





                return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = "Success", Message = $"User created SuccessFully" });

            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "This Role Doesnot Exist." });
            }


        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddYears(2),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.Username);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }


                var jwtToken = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expiration = jwtToken.ValidTo
                });
                //returning the token...

            }
            return Unauthorized();


        }



    }
}

step 9 :
-----------
Now Program.cs file further jwt code is written like this and for authorize button in swagger i am
writing some code in the middleware because in swagger authorize button is not there like
postman so explicitly u have to add code and finally down u have to add one method which is
app.UseAuthentication();

using CodeFirstEFDEmo.Models;
using CodeFirstEFDEmo.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace CodeFirstEFDEmo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IPost, PostRepository>();
            builder.Services.AddScoped<IEmployee, EmployeeService>();


            builder.Services.AddDbContext<EventContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("constring")));

            // For Identity
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<EventContext>()
            .AddDefaultTokenProviders();


            // adding basic authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience =builder.Configuration["JWT:ValidAudience"],
                    ValidIssuer =builder.Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
                };
            }); ;


            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddEndpointsApiExplorer(); // Required for Swagger
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
            });



            builder.Services.AddSwaggerGen();           // Adds Swagger support


            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();  // ✅ Add this line
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

Now i had created one controller like this here your shawbuckble version shoudl be 9.0.3 and i am usng 10 version which is not matching and compatibility issue is coming so 
i had downgraded the version here ....



step 10 :
-----------

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeFirstEFDEmo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        [HttpGet("employees")]
        public IEnumerable<string> Get()
        {
            return new List<string> { "santosh", "Ali", "sita" };
        }

    }
}

Here all in built methods will be there and now run the login method then we will see that a
token will be created .
next i will add [Authorize] attribute Here [Authorize] is one kind of filter which gets executed before u call means its logic will be executed 
Above class then 401 error i will get it means that first you
have to login and then token will be created and u have to send the token whenever u you are
calling the method which has that [Authorize] attribute

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeFirstEFDEmo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {

        [HttpGet("employees")]
        public IEnumerable<string> Get()
        {
            return new List<string> { "santosh", "Ali", "sita" };
        }

    }
}
so you can see in program.cs file to create a authorize button i had added this much amount of
code
        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type=ReferenceType.SecurityScheme,
                Id="Bearer"
            }
        },
        new string[]{}
    }
});
        });

one token will be created after login and on top one button is there Authorize take this token and click that button once
and submit the token

step 11 :
----------
    now again i have changed it to Admin Controller like this with role okay
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeFirstEFDEmo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles="Admin")
    public class AdminController : ControllerBase
    {
    

        [HttpGet("employees")]
        public IEnumerable<string> Get()
        {
            return new List<string> { "santosh", "Ali", "sita" };
        }

    }
}

Now for the Employee Controller 

using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiInAsp.netcoreMvcDemo.Models;

namespace WebApiInAsp.netcoreMvcDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmpController : ControllerBase
    {
        private readonly IEmployee _employeeService;
        public EmpController(IEmployee employeeService)
        {
            _employeeService = employeeService;
        }
        [HttpGet]
       
        public async Task<ActionResult<List<Employee>>>  GetAll(int page=1,int pageSize=5)
        {
            
            return Ok(await _employeeService.GetAllEmployeesAsync(page,pageSize));
        }

        [HttpGet("{id}")]
       public async Task<ActionResult<Employee>> GetById(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound("Employee not found");
            return Ok(employee);
        }

        [HttpGet("basic")]
        public async Task<ActionResult<List<EmployeeBasicDto>>> GetBasicEmployeeList(
     int page = 1, int pageSize = 5, string? search = null)
        {
            var result = await _employeeService.GetAllEmployeeBasicInfoAsync(page, pageSize, search);
            return Ok(result);
        }
        [HttpPost]
        [Authorize(Roles ="Admin,HR")]
        public async Task<ActionResult<Employee>> Create([FromForm]Employee employee,
            IFormFile? image)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var added = await _employeeService.AddEmployeeAsync(employee,image);
            return Ok(added);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<Employee>> Update(int id,[FromForm]EmployeeUpdateDto  employeeDto
            ,IFormFile? image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // map dto to entity
            var employee = new Employee
            {
                Id = id, // take from route only
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                Age = employeeDto.Age,
                ImagePath = employeeDto.ImagePath
            };

            var updated = await _employeeService.UpdateEmployeeAsync(employee, image);
            if (updated == null)
                return NotFound("Employee not found to update");

            return Ok(updated);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Employee>> Delete(int id)
        {
            var deleted = await _employeeService.DeleteEmployeeAsync(id);
            if (deleted == null)
                return NotFound("Employee not foudn to delete");
            return Ok(deleted);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportToExcel(string? search = null)
        {
            var employees = await _employeeService.GetAllEmployeeBasicInfoAsync(1, int.MaxValue, search);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Employees");

            worksheet.Cell(1, 1).Value = "First Name";
            worksheet.Cell(1, 2).Value = "Last Name";
            worksheet.Cell(1, 3).Value = "Email";
            worksheet.Cell(1, 4).Value = "Image URL";

            int row = 2;
            foreach (var emp in employees)
            {
                worksheet.Cell(row, 1).Value = emp.FirstName;
                worksheet.Cell(row, 2).Value = emp.LastName;
                worksheet.Cell(row, 3).Value = emp.Email;
                worksheet.Cell(row, 4).Value = emp.ImageUrl;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employees.xlsx");
        }



    }
}

now again if try to use means create a user without admin role okay and then try to access this
method with that user just now error 403 forbidden will come okay means u will login with other
than admin user and will try to use this method then 403 error will come okay so using admin
token only i have to give then i can access above method

When IdentityUser is also a table in your database (e.g., when you want to store users along with other domain entities like employees, posts, etc.),
here’s what you should do step-by-step:

✅ 1. Create a Custom User Class (if needed)
     
Instead of using IdentityUser directly, you can extend it to include custom properties like FullName, Image, etc.

using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
    public string? ImageUrl { get; set; }  // Optional: for profile pictures
}
This will still use Identity, but now you control the schema of the AspNetUsers table.

✅ 2. Register Your Custom User with Identity
In Program.cs, register ApplicationUser instead of IdentityUser:

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<EventContext>()
    .AddDefaultTokenProviders();
✅ 3. Update DbContext to Use Custom User
Make sure your EventContext inherits from IdentityDbContext<ApplicationUser> (not just DbContext):

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class EventContext : IdentityDbContext<ApplicationUser>
{
    public EventContext(DbContextOptions<EventContext> options)
        : base(options)
    {
    }

    // Your other DbSets
    public DbSet<Post> Posts { get; set; }
    public DbSet<Employee> Employees { get; set; }
}
✅ 4. Apply Migrations
After updating the model and DbContext:

dotnet ef migrations add AddIdentityUser
dotnet ef database update
This will generate and apply tables like:

AspNetUsers (based on ApplicationUser)

AspNetRoles

AspNetUserRoles

AspNetUserClaims, etc.

✅ 5. Use Dependency Injection for UserManager<ApplicationUser>
You can inject the UserManager and SignInManager in your controllers:

private readonly UserManager<ApplicationUser> _userManager;
private readonly SignInManager<ApplicationUser> _signInManager;

public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
{
    _userManager = userManager;
    _signInManager = signInManager;
}
✅ 6. Save and Query Users
You can now save and query users like this:

var user = new ApplicationUser
{
    UserName = "testuser",
    Email = "test@example.com",
    FullName = "Test User"
};

var result = await _userManager.CreateAsync(user, "Test@123");
✅ Optional: Create Relations with Other Tables
If your Employee or Post tables relate to users, use ApplicationUser as a foreign key:

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
}
✅ Summary
Task	Implementation
Custom user properties	--->Extend IdentityUser to ApplicationUser
Register Identity  --->	Use ApplicationUser in AddIdentity
Use Identity in EF-->	Inherit IdentityDbContext<ApplicationUser>
Relate other tables	Use ApplicationUser as FK in other entities

so where ever IdentityUser is there there ApplicationUser will come thats all

Let me know if you want a full working example (model, dbcontext, registration, and login API) for Identity integration with JWT and custom fields.

I told yes 


Here is a complete working example of integrating ASP.NET Core Identity with JWT Authentication, using:

✅ Custom ApplicationUser class

✅ Identity tables in SQL Server

✅ Registration and Login API

✅ JWT Token generation

✅ STEP 1: Create the Custom ApplicationUser
// Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
    public string? ProfileImageUrl { get; set; }
}
✅ STEP 2: Update the DbContext
// Data/EventContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class EventContext : IdentityDbContext<ApplicationUser>
{
    public EventContext(DbContextOptions<EventContext> options)
        : base(options)
    {
    }

    public DbSet<Post> Posts { get; set; }
    public DbSet<Employee> Employees { get; set; }
}
✅ STEP 3: appsettings.json (JWT Section)
"JWT": {
  "ValidAudience": "http://localhost:5000",
  "ValidIssuer": "http://localhost:5000",
  "Secret": "ThisIsMySuperSecretKey1234567890"
}
✅ STEP 4: Program.cs
using CodeFirstEFDEmo.Models;
using CodeFirstEFDEmo.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add EF and Identity
builder.Services.AddDbContext<EventContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("constring")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<EventContext>()
    .AddDefaultTokenProviders();

// JWT Auth
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();  // Important
app.UseAuthorization();

app.MapControllers();
app.Run();
✅ STEP 5: AuthController

// Controllers/AuthController.cs
using CodeFirstEFDEmo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Username,
            Email = model.Email,
            FullName = model.FullName
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        return Unauthorized();
    }
}
✅ STEP 6: Models for Login/Register
// Models/RegisterModel.cs
public class RegisterModel
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Password { get; set; }
}
// Models/LoginModel.cs
public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}
✅ STEP 7: Migration and Database Update

dotnet ef migrations add InitialIdentity
dotnet ef database update

✅ STEP 8: Test Endpoints
POST /api/Auth/register

POST /api/Auth/login

Both can be tested in Swagger UI or Postman.


Azure storage Notes 
----------------------

create a new project of console application and through this console application i want to put some data in data container 

so Now add dependencies in project Azure.Storage.Blobs of version 12.24.0 version add it 

and code is here like this 

     using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading.Tasks;
namespace ConsoleApp1
{
    internal class Program
    {
        static string str_connection = "" ;// this value i had kept empty u have to take it from azure access keys and add it and in github u cant put sececet 
        // connection like this becasue when u try to commit it tells secret information is there so for security reasong i cannot do commit so removed

        static async Task Main()
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(str_connection);
            BlobContainerClient container_client =
            blobServiceClient.GetBlobContainerClient("data");
            BlobClient blob_client = container_client.GetBlobClient("CNotes.txt");
            using FileStream uploadFileStream = File.OpenRead(@"D:\CNotes.txt");
            await blob_client.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();
            Console.WriteLine("File uploaded");
            Console.WriteLine("Operation complete");
        }
    }
}


create a new asp.net core mvc application with the name AzureBlobProject



--->Now add dependencies in project Azure.Storage.Blobs of version 12.24.0 version add it 

--->Go to portal of storage account which u  have created click containers left side dont select containers or any container and just go to access key and there copy the 
connection string and put it that in app settings like this 

     {
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",

  "BlobConnection":""// here i had not kept key as while commiting it was giving error that some body can go inside ur storage so not kept but u copy and keep in program okay 

}

and then in program.cs 

builder.Services.AddSingleton(u => new BlobServiceClient(builder.Configuration.GetValue<string>("BlobConnection")));

--->create a folder with the name Services and in that add interface  IContainerService and a class ContainerService  


 namespace AzureBlobProject.Services
{
    public interface IContainerService
    {

        Task<List<string>> GetAllContainerAndBlobs();
        Task<List<string>> GetAllContainer();
        Task CreateContainer(string containerName);
        Task DeleteContainer(string containerName);
    }
}


     using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureBlobProject.Services
{
    public class ContainerService : IContainerService
    {
        private readonly BlobServiceClient _blobClient;
        public ContainerService(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }
        public async Task CreateContainer(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
        }

        public async Task DeleteContainer(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            await blobContainerClient.DeleteIfExistsAsync();
        }

        public async Task<List<string>> GetAllContainer()
        {
            List<string> containerName = new();

            await foreach (BlobContainerItem blobContainerItem in _blobClient.GetBlobContainersAsync())
            {
                containerName.Add(blobContainerItem.Name);
            }

            return containerName;
        }

        public async Task<List<string>> GetAllContainerAndBlobs()
        {
            List<string> containerAndBlobName = new();
            containerAndBlobName.Add("-----Account Name : " + _blobClient.AccountName + "-----");
            containerAndBlobName.Add("---------------------------------------------------------------");

            await foreach (BlobContainerItem blobContainerItem in _blobClient.GetBlobContainersAsync())
            {
                containerAndBlobName.Add("-----" + blobContainerItem.Name);
                BlobContainerClient _blobContainer = _blobClient.GetBlobContainerClient(blobContainerItem.Name);

                await foreach (BlobItem blobItem in _blobContainer.GetBlobsAsync())
                {
                    //get metadata

                    var blobClient = _blobContainer.GetBlobClient(blobItem.Name);
                    BlobProperties blobProperties = await blobClient.GetPropertiesAsync();
                    string tempBlobToAdd = blobItem.Name;
                    if (blobProperties.Metadata.ContainsKey("title"))
                    {
                        tempBlobToAdd += "(" + blobProperties.Metadata["title"] + ")";
                    }

                    containerAndBlobName.Add(">>" + tempBlobToAdd);
                }
                containerAndBlobName.Add("---------------------------------------------------------------");
            }


            return containerAndBlobName;
        }
    }
}

------->Again in program.cs file add this thing

     // Add services to the container.
     builder.Services.AddControllersWithViews();

     builder.Services.AddSingleton(u => new BlobServiceClient(builder.Configuration.GetValue<string>("BlobConnection")));
     builder.Services.AddSingleton<IContainerService, ContainerService>();// add this above code is already there 



----->In Models folder add ContainerModel like this 

     using System.ComponentModel.DataAnnotations;

namespace AzureBlobProject.Models
{
    public class ContainerModel
    {
        [Required]
        public string Name { get; set; }
    }
}

------>Now create a ContainerController Empty one and add the below code there 


using AzureBlobProject.Models;
using AzureBlobProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobProject.Controllers
{
    public class ContainerController : Controller
    {
        private readonly IContainerService _containerService;
        public ContainerController(IContainerService containerService)
        {
            _containerService = containerService;
        }
        public async Task<IActionResult> Index()
        {
            var allContainer = await _containerService.GetAllContainer();
            return View(allContainer);
        }

        public async Task<IActionResult> Create()
        {
            return View(new ContainerModel());
        }
        [HttpPost]
        public async Task<IActionResult> Create(ContainerModel container)
        {
            await _containerService.CreateContainer(container.Name);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(string containerName)
        {
            await _containerService.DeleteContainer(containerName);
            return RedirectToAction(nameof(Index));
        }

    }
}
Now add views for ContainerController like this 
create totally an empty view one is Index.cshtml and another is Create one 

so codes goes like this so while adding view razor view only but uncheck the layout 

Index.cshtml
------------
@model List<string>
<br />
<br />
<div class="border backgroundWhite p-4">
    <div class="row">
        <div class="col-6">
            <h2 class="text-info"> Container List</h2>
        </div>
        <div class="col-6 text-right">
            <a asp-action="Create" class="btn btn-info"><i class="fas fa-plus"></i> &nbsp;  Create New </a>
        </div>
    </div>
    <br />
    <div>
        @if (Model != null && Model.Count() > 0)
        {
            <table class="table table-striped border">
                <tr class="table-secondary">
                    <th>
                        Container Name
                    </th>
                    <th></th>
                </tr>
                @foreach (var item in Model)
                {
                    <tr>
                        <td>
                            @item
                        </td>
                        <td>
                            <a class="btn btn-primary text-white" asp-action="Manage" asp-controller="Blob" asp-route-containerName="@item">
                                Manage
                            </a>

                            <a class="btn btn-danger text-white" asp-action="Delete" asp-route-containerName="@item">
                                Delete
                            </a>
                        </td>
                    </tr>
                }
            </table>
        }
        else
        {
            <p>No category exists...</p>
        }
    </div>
</div>

Create.cshtml
---------------
@model ContainerModel

<div class="container border p-3">
    <h2 class="text-info pb-2">Create Category</h2>
    <form method="post" asp-action="Create">
        <div class="backgroundWhite">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <div class="form-group row py-2">
                <div class="col-2">
                    <label asp-for="Name" class="col-form-label"></label>
                </div>
                <div class="col-10">
                    <input asp-for="Name" class="form-control" />
                </div>
                <span asp-validation-for="Name" class="text-danger"></span>
            </div>
            <div class="form-group row">
                <div class="col-6">
                    <input type="submit" class="btn btn-info form-control" value="Create" />
                </div>
                <div class="col-6">
                    <a asp-action="Index" class="btn btn-success form-control">Back to List</a>
                </div>
            </div>
        </div>
    </form>
</div>
@section Scripts {
    @{
        await Html.RenderPartialAsync("_ValidationScriptsPartial");
    }
}
     
     
--->Now add BlobModel class in Models folder like this 

 namespace AzureBlobProject.Models
{
    public class BlobModel
    {

        public string Title { get; set; }
        public string Comment { get; set; }
        public string Uri { get; set; }
    }
}



Now add IBlobService interface in Services folder and also BlobService class also in Services folder like this so code is present below 

using AzureBlobProject.Models;

namespace AzureBlobProject.Services
{
    public interface IBlobService
    {
        Task<List<string>> GetAllBlobs(string containerName);
        Task<List<BlobModel>> GetAllBlobsWithUri(string containerName);
        Task<string> GetBlob(string name, string containerName);
        Task<bool> CreateBlob(string name, IFormFile file, string containerName, BlobModel blobModel);
        Task<bool> DeleteBlob(string name, string containerName);

    }
}



using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureBlobProject.Models;

namespace AzureBlobProject.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobClient;
        public BlobService(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }
        public async Task<bool> CreateBlob(string name, IFormFile file, string containerName, BlobModel blobModel)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);

            var blobClient = blobContainerClient.GetBlobClient(name);

            var httpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders()
            {
                ContentType = file.ContentType
            };

            IDictionary<string, string> metaData = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(blobModel.Title))
            {
                metaData.Add("title", blobModel.Title);
            }
            if (!string.IsNullOrEmpty(blobModel.Comment))
            {
                metaData.Add("comment", blobModel.Comment);
            }

            var result = await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders, metaData);

            //IDictionary<string,string> removeMetaData = new Dictionary<string, string>();

            //metaData.Remove("title");
            //await blobClient.SetMetadataAsync(metaData);

            if (result != null)
            {
                return true;
            }
            return false;

        }

        public async Task<bool> DeleteBlob(string name, string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);

            var blobClient = blobContainerClient.GetBlobClient(name);

            return await blobClient.DeleteIfExistsAsync();
        }

        public async Task<List<string>> GetAllBlobs(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            var blobs = blobContainerClient.GetBlobsAsync();

            List<string> blobNames = new List<string>();
            await foreach (var blob in blobs)
            {
                blobNames.Add(blob.Name);
            }

            return blobNames;
        }

        public async Task<List<BlobModel>> GetAllBlobsWithUri(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            var blobs = blobContainerClient.GetBlobsAsync();

            List<BlobModel> blobList = new List<BlobModel>();
            string sasContainerSignature = "";

            //if (blobContainerClient.CanGenerateSasUri)
            //{
            //    BlobSasBuilder blobSasBuilder = new()
            //    {
            //        BlobContainerName = blobContainerClient.Name,
            //        Resource = "c",
            //        ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            //    };

            //    blobSasBuilder.SetPermissions(BlobSasPermissions.Read);

            //    sasContainerSignature = blobContainerClient.GenerateSasUri(blobSasBuilder).AbsoluteUri.Split('?')[1].ToString();

            //}


            await foreach (var blob in blobs)
            {
                var blobClient = blobContainerClient.GetBlobClient(blob.Name);

                BlobModel blobModel = new()
                {
                    Uri = blobClient.Uri.AbsoluteUri //+ "?"+sasContainerSignature
                };

                //if (blobClient.CanGenerateSasUri)
                //{
                //    BlobSasBuilder blobSasBuilder = new()
                //    {
                //        BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                //        BlobName = blobClient.Name,
                //        Resource="b",
                //        ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                //    };

                //    blobSasBuilder.SetPermissions(BlobSasPermissions.Read);

                //    blobModel.Uri = blobClient.GenerateSasUri(blobSasBuilder).AbsoluteUri;

                //}


                BlobProperties properties = await blobClient.GetPropertiesAsync();
                if (properties.Metadata.ContainsKey("title"))
                {
                    blobModel.Title = properties.Metadata["title"];
                }
                if (properties.Metadata.ContainsKey("comment"))
                {
                    blobModel.Comment = properties.Metadata["comment"];
                }
                blobList.Add(blobModel);
            }

            return blobList;
        }

        public async Task<string> GetBlob(string name, string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);

            var blobClient = blobContainerClient.GetBlobClient(name);

            if (blobClient != null)
            {
                return blobClient.Uri.AbsoluteUri;
            }
            return "";
        }
    }
}

---->now again register the dependency now in program.cs file 

 builder.Services.AddSingleton<IContainerService, ContainerService>();
 builder.Services.AddSingleton<IBlobService, BlobService>();// this line add it not above one it is for your understanding where to write okay 

     
Now add BlobController now and the code is like this 

     using AzureBlobProject.Models;
using AzureBlobProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobProject.Controllers
{
    public class BlobController : Controller
    {
        private readonly IBlobService _blobService;
        public BlobController(IBlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpGet]
        public async Task<IActionResult> Manage(string containerName)
        {
            var blobsObj = await _blobService.GetAllBlobs(containerName);
            return View(blobsObj);
        }

        [HttpGet]
        public async Task<IActionResult> AddFile(string containerName)
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> AddFile(string containerName, IFormFile file, BlobModel blobModel)
        {
            if (file == null || file.Length < 1) return View();
            //file name - xps_img2.png 
            //new name - xps_img2_GUIDHERE.png
            var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
            var result = await _blobService.CreateBlob(fileName, file, containerName, blobModel);

            if (result)
                return RedirectToAction("Manage", new { containerName });


            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ViewFile(string name, string containerName)
        {
            return Redirect(await _blobService.GetBlob(name, containerName));
        }

        public async Task<IActionResult> DeleteFile(string name, string containerName)
        {
            await _blobService.DeleteBlob(name, containerName);
            return RedirectToAction("Manage", new { containerName });
        }


    }
}


---->add views now for Manage.cshtml and AddFile like this 

     
AddFile.cshtml
-------------
@model BlobModel

<form method="post" asp-action="AddFile" asp-controller="Blob" enctype="multipart/form-data">
    <div class="container">
        <h3 class="py-2">Container Name -  @Context.Request.Query["containerName"].ToString()</h3>
        <input hidden name="containerName" value="@Context.Request.Query["containerName"].ToString()" />
        <div class="row border p-2">
            <div class="col-12 pb-2"><h4 class="text-primary">Add Item to Blob :</h4></div>
            <div class="col-3 py-2">Title MetaData</div>
            <div class="col-9 py-2">
                <input asp-for="Title" class="form-control py-2" />
            </div>
            <div class="col-3 py-2">Comment MetaData</div>
            <div class="col-9 py-2">
                <input asp-for="Comment" class="form-control py-2" />
            </div>


            <div class="form-group col-9">
                <input type="file" class="form-control pt-1" name="file" />
            </div>
            <div class="form-group col-3">
                <button type="submit" class="btn btn-success form-control">Add</button>
            </div>
        </div>
    </div>
</form>

Manage.cshtml
--------------
@model List<string>


<div class="container p-4 border">
    <div class="row">
        <div class="col-6">
            <h3> Container Name - @Context.Request.Query["containerName"].ToString()</h3>
        </div>
        <div class="col-6 text-right">
            <a asp-action="AddFile" asp-controller="Blob"
               asp-route-containerName="@Context.Request.Query["containerName"].ToString()" class="btn btn-success">Add Blob</a>
        </div>
    </div>
    <div class="container pt-4">
        @foreach (var item in Model)
        {
            <div class="row py-1">
                <div class="col-6">
                    <a asp-action="ViewFile" asp-controller="Blob" target="_blank" asp-route-name="@item"
                       asp-route-containerName="@Context.Request.Query["containerName"].ToString()">
                        @item
                    </a>

                </div>
                <div class="col-6">

                    <a asp-action="DeleteFile" asp-controller="Blob" asp-route-name="@item" class="btn btn-danger"
                       asp-route-containerName="@Context.Request.Query["containerName"].ToString()">
                        Delete
                    </a>
                </div>
            </div>
        }
    </div>
</div>

For all the above coding and more in depth idea how code is implemented you can check this series here everything in depth about azure is there 

Bruhgpatel videos 
-------------------
https://www.youtube.com/watch?v=YVp3SBy90tw&list=PLeFMiSA9bHD6IS9bIuXzNcG-dAJmL1maa&index=1
