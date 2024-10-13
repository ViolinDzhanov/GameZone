using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameZone.Data
{
    public class GamerGame
    {
        public int GameId { get; set; }

        [ForeignKey(nameof(GameId))]
        public Game Game { get; set; } = null!;
        public string GamerId { get; set; } = null!;

        [ForeignKey(nameof(GamerId))]
        public IdentityUser Gamer { get; set; } = null!; 
    }
}
