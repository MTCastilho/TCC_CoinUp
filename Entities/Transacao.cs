using Coin_up.Entities;
using Coin_up.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Transacao
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UsuarioId { get; set; }

    [Required]
    public Guid ContaId { get; set; }

    [Required]
    public EnumTipoTransacao TipoTransacao { get; set; }

    [Required]
    public EnumCategoria Categoria { get; set; }

    public string? Descricao { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Valor { get; set; }

    [Required]
    public DateTime Data { get; set; } = DateTime.UtcNow;

    [ForeignKey("UsuarioId")]
    public virtual Usuario Usuario { get; set; }

    [ForeignKey("ContaId")]
    public virtual Conta Conta { get; set; }
}