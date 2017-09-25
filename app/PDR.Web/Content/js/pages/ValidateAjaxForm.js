function ValidateAjaxForm(form) {
    $.validator.unobtrusive.parse(document);
    form.validate();
}