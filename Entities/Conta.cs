using Coin_up.Entities;
using Coin_up.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Conta
{
    // MUDANÇA: Adicionada Chave Primária
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal SaldoInicial { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal SaldoAtual { get; set; }

    [Required]
    public EnumTipoConta TipoConta { get; set; } // MUDANÇA: Nome do Enum corrigido

    // MUDANÇA: Adicionadas propriedades de navegação
    [ForeignKey("UserId")]
    public virtual Usuario Usuario { get; set; }
    public virtual ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}