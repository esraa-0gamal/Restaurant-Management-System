namespace RestaurantSystem.Models
{
    public class MenuVeiwModel
    {
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Dish> Dishes { get; set; } = new List<Dish>();

        public int ? SelectedCategoryId { get; set; }
        public string SearchTerm { get; set; } 

        public List<Extra> Extras { get; set; } = new List<Extra>();




    }
}
