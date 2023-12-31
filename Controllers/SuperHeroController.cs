﻿using CrudWebApiDapper.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace CrudWebApiDapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly string _config;

        public SuperHeroController(IConfiguration config)
        {
            _config = config.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAllSuperHeroes()
        {
            using var connection = new SqlConnection(_config); //Pode chamar o GetConnectionString aqui
            IEnumerable<SuperHero> heroes = await SelectAllHeroes(connection);
            return Ok(heroes);
        }

        [HttpGet("{heroId}")]
        public async Task<ActionResult<SuperHero>> GetHero(int heroId)
        {
            using (var connection = new SqlConnection(_config))
            {
                var hero = await connection.QueryFirstOrDefaultAsync<SuperHero>("SELECT * FROM superheroes WHERE id = @Id", new { Id = heroId });
                if (hero is null)
                    return NotFound();
                return Ok(hero);
            }
        }

        [HttpPost]
        public async Task<ActionResult<List<SuperHero>>> CreateHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config);
            //Pode ser passado campo a campo aqui embaixo
            await connection.ExecuteAsync("INSERT INTO superheroes (name, firstname, lastname, place) VALUES (@Name, @FirstName, @LastName, @Place)", hero); 
            return Ok(await SelectAllHeroes(connection));
        }

        [HttpPut]
        public async Task<ActionResult<List<SuperHero>>> UpdateHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config);
            await connection.ExecuteAsync("UPDATE superheroes SET name = @Name, firstname = @FirstName, lastname = @LastName, place = @Place WHERE id = @Id", hero);
            return Ok(await SelectAllHeroes(connection));
        }

        [HttpDelete("{heroId}")]
        public async Task<ActionResult<List<SuperHero>>> DeleteHero(int heroId)
        {
            using var connection = new SqlConnection(_config);
            await connection.ExecuteAsync("DELETE FROM superheroes WHERE id = @Id", new { Id = heroId });
            return Ok(await SelectAllHeroes(connection));
        }

        private static async Task<IEnumerable<SuperHero>> SelectAllHeroes(SqlConnection connection)
        {
            return await connection.QueryAsync<SuperHero>("Select * from superheroes");
        }
    }
}
