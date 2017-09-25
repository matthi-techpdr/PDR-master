using FluentValidation.Attributes;
using PDR.Web.Areas.Default.Validators;

namespace PDR.Web.Areas.Default.Models
{
    [Validator(typeof(LoginModelValidator))]
    public class LoginModel
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string Company { get; set; }
    }
    //public class LoginViewModel
    //{
    //    //[Required]
    //    //[Display(Name = "Email")]
    //    //[EmailAddress]
    //    public string Email { get; set; }

    //    //[Required]
    //    //[DataType(DataType.Password)]
    //    //[Display(Name = "Password")]
    //    public string Password { get; set; }

    //    //[Display(Name = "Remember me?")]
    //    public bool RememberMe { get; set; }
    //}

}