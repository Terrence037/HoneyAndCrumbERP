namespace ERP.domain.entities;

public class Inventory
{
    public int InventoryId { get; set; }
    public int ProductId { get; set; }
    public decimal QuantityOnHand { get; set; }
    public decimal ReorderLevel { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    public Product Product { get; set; } = null!;
}