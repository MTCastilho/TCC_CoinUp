using Coin_up.Entities;
using Coin_up.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Quest
{
    // MUDANÇA: Adicionada Chave Primária
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Descricao { get; set; }

    [Required]
    public EnumQuestStatus Status { get; set; } = EnumQuestStatus.Ativa;
    public DateTime DataDeCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataDeExpiracao { get; set; }
    public DateTime? DataDeConclusao { get; set; }

    [Required]
    public EnumQuestObjectiveType TipoDeObjetivo { get; set; }

    public EnumCategoria? CategoriaAlvo { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal ValorAlvo { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal ValorAtual { get; set; }

    public int ProgressoAtual { get; set; } = 0;

    public int PontosDeExperiencia { get; set; }

    [ForeignKey("UserId")]
    public virtual Usuario Usuario { get; set; }
}