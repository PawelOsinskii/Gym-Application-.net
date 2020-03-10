using Projekcik.NETS.Models.Data;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Projekcik.NETS.Views.Pages
{
    public class PageVM
    {
        public PageVM()
        {

        }

        public PageVM(PageDTO row)
        {
            Id = row.Id;
            Title = row.Title;
            Slug = row.Slug;
            Body = row.Body;
            Sorting = row.Sorting;
            HasSidebar = row.HasSidebar;
        }

        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength =3)]
        [Display(Name = "Tytuł strony")]
        public string Title { get; set; }
        [Display(Name = "adres strony")]
        public string Slug { get; set; }
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3)]
        [Display(Name = "zawartość strony")]
        [AllowHtml]
        public string Body { get; set; }
        public int Sorting { get; set; }
        [Display(Name = "pasek boczny")]
        public bool HasSidebar { get; set; }
    }
}