using Microsoft.AspNetCore.Mvc;
using A2.Models;
using A2.Data;
using A2.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;

namespace A2.Controllers
{
    [Route("api")]
    [ApiController]
    public class A2Controller : Controller
    {
        private readonly IA2Repo _repository;

        public A2Controller(IA2Repo repository)
        {
            _repository=repository;
        }

        // Endpoint 3: PURCHASE ITEM (0.5 marks)

        [Authorize(AuthenticationSchemes = "A2UserAuthentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("PurchaseItem/{id}")]
        public ActionResult<Order> PurchaseItem(long id)
        {
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string username = c.Value;

            Order newOrder = new() { ProductId = id, UserName = username };
            return Ok(newOrder);
        }

        // Endpoint 1: USER REGISTRATION(1.5 marks)
        [HttpPost("Register")]
        public ActionResult<string> Register(User user)
        {
            Response.Headers.Add("Content-Type", "text/plain");

            if (_repository.UserExists(user))
            {
                return Ok("Username not available.");
            }
            else
            {
                User newUser = new() { UserName = user.UserName, Password = user.Password, Address = user.Address };
                _repository.RegisterUser(newUser);
                return Ok("User successfully registered.");
            }
        }

        // Endpoint 2: GET THE VERSION OF THE WEB API (1.5 marks)

        [Authorize(AuthenticationSchemes = "A2UserAuthentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("GetVersionA")]
        public ActionResult<string> GetVersionA()
        {
            return Ok("1.0.0 (auth)");
        }

        // Endpoint 4: START A GAME (3.25 marks)

        [Authorize(AuthenticationSchemes = "A2UserAuthentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("PairMe")]
        public ActionResult<GameRecordOutDto> PairMe()
        {
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string username = c.Value;

            GameRecord newGame = _repository.PairingUser(username);
            GameRecordOutDto dtoNewGame = new() { 
                GameId = newGame.GameId, 
                State = newGame.State, 
                Player1 = newGame.Player1, 
                Player2 = newGame.Player2, 
                LastMovePlayer1 = newGame.LastMovePlayer1, 
                LastMovePlayer2 = newGame.LastMovePlayer2 };

            return Ok(dtoNewGame);
        }

        // Endpoint 6: MAKE A MOVE (2.75 marks)
        [Authorize(AuthenticationSchemes = "A2UserAuthentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpPost("MyMove")]
        public ActionResult<string> MyMove(GameMove gamemove)
        {
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string username = c.Value;

            GameRecord userGameRecord = _repository.GetGameById(gamemove.GameId);

            if (userGameRecord == null)
            {
                return Ok("no such gameId");
            }
            else if (userGameRecord.State.Equals("wait") && userGameRecord.Player1 == username)
            {
                return Ok("You do not have an opponent yet.");
            }
            else if (userGameRecord.Player1.Equals(username))
            {
                if (userGameRecord.LastMovePlayer1 != null)
                {
                    return Ok("It is not your turn.");
                }
                else
                {
                    userGameRecord.LastMovePlayer1 = gamemove.Move;
                    userGameRecord.LastMovePlayer2 = null;
                    _repository.UpdateGameRecord(userGameRecord);
                    return Ok("move registered");
                }
            }
            else if (userGameRecord.Player2.Equals(username))
            {
                if (userGameRecord.LastMovePlayer2 != null)
                {
                    return Ok("It is not your turn.");
                }
                else
                {
                    userGameRecord.LastMovePlayer2 = gamemove.Move;
                    userGameRecord.LastMovePlayer1 = null;
                    _repository.UpdateGameRecord(userGameRecord);
                    return Ok("move registered");
                }
            }
            else
            {
                return Ok("not your gameId");
            }
        }

        // Endpoint 5: GET OPPONENT’s MOVE (2.75 marks)

        [Authorize(AuthenticationSchemes = "A2UserAuthentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("TheirMove/{gameId}")]
        public ActionResult<string> TheirMove(string gameId)
        {
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string username = c.Value;

            GameRecord gameRecord = _repository.GetGameById(gameId);

            if (gameRecord == null)
            {
                return Ok("no such gameId");
            }
            else if (gameRecord.State.Equals("wait") && gameRecord.Player1 == username)
            {
                return Ok("You do not have an opponent yet.");
            }
            else if (gameRecord.Player1.Equals(username))
            {
                if (gameRecord.LastMovePlayer2 == null)
                {
                    return Ok("Your opponent has not moved yet.");
                }
                else
                {
                    return Ok(gameRecord.LastMovePlayer2);
                }
            }
            else if (gameRecord.Player2.Equals(username))
            {
                if (gameRecord.LastMovePlayer1 == null)
                {
                    return Ok("Your opponent has not moved yet.");
                }
                else
                {
                    return Ok(gameRecord.LastMovePlayer1);
                }
            }
            else
            {
                return Ok("not your game id");
            }
        }


        // Endpoint 7 QUIT A GAME (2.75 marks)

        [Authorize(AuthenticationSchemes = "A2UserAuthentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("QuitGame/{gameId}")]
        public ActionResult<string> QuitGame(string gameId)
        {
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string username = c.Value;

            GameRecord gameRecord = _repository.GetGameById(gameId);

            // If no game record in exists then it should return no such gameID.
            if (gameRecord == null)
            {
                return Ok("no such gameId");
            }
            else if (!_repository.GetUserActiveGame(username))
            {
                return Ok("You have not started a game");
            }
            else if (!gameRecord.Player1.Equals(username) && !gameRecord.Player2.Equals(username))
            {
                return Ok("not your game id");
            }
            else
            {
                _repository.RemoveGameRecord(gameRecord);
                return Ok("game over");
            }
        }
    }
}


