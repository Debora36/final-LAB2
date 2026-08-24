namespace final_LAB2.Models.ViewModels
{
    // Generico: sirve para paginar el listado de cualquier entidad (Usuario, Equipo, Empleado, etc.)
    public class PaginatedListViewModel<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
