namespace VentasApi2026.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<RolePermission> RolePermissions { get; set; }
            = new List<RolePermission>();
    }
}
