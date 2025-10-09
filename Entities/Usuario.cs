using Coin_up.Enums;
using System.ComponentModel.DataAnnotations;

namespace Coin_up.Entities
{
    public class Usuario
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string FirebaseUid { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(150)]
        public string Email { get; set; }
        public string Telefone { get; set; }
        public EnumSexo Sexo { get; set; }
        public int PontosDeExperiencia { get; set; } = 0;
        public EnumRank Rank { get; set; } = EnumRank.Nenhum;
        public virtual ICollection<Conta> Contas { get; set; } = new List<Conta>();
        public virtual ICollection<Quest> Quests { get; set; } = new List<Quest>();
        public virtual ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
    }
}
