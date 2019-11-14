using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingAppMvc.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DatingAppMvc.Controllers
{
    [Authorize]
    [Route("[Controller]")]
    [ApiController]

    // ControllerBase is used here because we are using angular to supply the view.  If
    // we were using a normal MVC setup where we are supplying the View then we would need
    // to inherit from teh Controll class
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _context;

        public ValuesController(DataContext context)
        {
            _context = context;
        }
        // GET: /<controller>/

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            var values = await _context.Values.ToListAsync();
            return Ok(values);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetValue(int id)
        {
            var value = await _context.Values.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(value);
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {

        }

        [HttpDelete("{id}")]
        public void Delet(int id)
        {

        }

    }
}
