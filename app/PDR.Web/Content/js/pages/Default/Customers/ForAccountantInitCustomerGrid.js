function Customer() {
    this.grid = $("#customerForAccountantGrid");
    this.getdataurl = "/customers/getdataforaccountant";
//    this.getcurrenturl = "employees/getemployee";
//    this.editurl = "employees/editemployee";
//    this.createurl = "employees/createemployee";
//    this.suspendurl = "employees/suspendemployee";
//    this.reactivateurl = "employees/reactivateemployee";
    this.editform = '#customerForm';
    this.editcontainer = $("#customerInfo");
};

$(function () {
    GridInitializer.InitGrid(
        [
            'Id',
            'Customer adding date',
            'Customer name',
            'Customer e-mail',
            'Customer phone number',
            'Customer account status'
        ], [
            { name: 'Id', index: 'Id', hidden: true },
            { name: 'Date', index: 'Date' },
            { name: 'Name', index: 'Name' },
            { name: 'Email', index: 'Email' },
            { name: 'Phone', index: 'Phone' },
            { name: 'Status', index: 'Status' }
        ],
        new Customer(),
        'Date',
        'List of wholesale customers'
    );
})