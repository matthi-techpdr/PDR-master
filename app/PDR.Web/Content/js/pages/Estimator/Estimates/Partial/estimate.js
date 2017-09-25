var viewModel;

var elem;
var url = '';
var inspection;
var retailCustomer;

var estimateViewModel;

var $loader;
var $retailFirstName;
var $retailLastName;

$().ready(function () {
    window.globals.flagAlert = true;
    $(document).bind('onAfterSetup', function () { $.ajaxSetup({ async: true }); });
    $("#accordion").multiAccordion({ active: '0' });
    $loader = $('#loader');
    $retailFirstName = window.globals.order == "True" ? $('#EstimateModel_Customer_Retail_FirstName') : $('#Customer_Retail_FirstName');
    $retailLastName = window.globals.order == "True" ? $('#EstimateModel_Customer_Retail_LastName') : $('#Customer_Retail_LastName');

    viewModel = new ViewModel();

    estimateViewModel = new EstimateViewModel();

    estimateViewModel.retailCustomer.firstName($retailFirstName.val());
    estimateViewModel.retailCustomer.lastName($retailLastName.val());

    viewModel.carInspection(estimateViewModel.inspection);
    viewModel.customerRetailInfo(estimateViewModel.retailCustomer);

    viewModel.carInfo(estimateViewModel.car);

    var type = $(this).attr('value');
    window.globals.type = type;

    ko.applyBindings(viewModel);

    estimateViewModel.setupCustomerTypeChange();

    
    if (window.globals.customerType == 'Retail') {
        viewModel.customerInfo(estimateViewModel.retailCustomer.fullName());
        estimateViewModel.retailCustomer.affiliateCustomerDataBindChange();
    }
    else {
        estimateViewModel.wholesale.wholesaleCustomerDataBindChange();
        estimateViewModel.wholesale.wholesaleId.selectBox('value', window.globals.currentWholesaleCustomer);
        estimateViewModel.wholesale.wholesalaMatrixId.selectBox('value', window.globals.currentMatrix);
        viewModel.customerInfo(' - ' + $('#Customer_Wholesale_CustomerId :selected').text());
        $('#Customer_Retail_State').selectBox('disable');
        $('#retail-customer input[type="text"]').attr('disabled', 'disabled');
        $('#Customer_Retail_AffiliateId').selectBox('disable');
        if (window.globals.hasInsurance == "False") {
            estimateViewModel.insurance.insuranceHide();
        }
    }

    if (window.globals.order == 'True') {
        estimateViewModel.wholesale.wholesaleDisable();
        viewModel.order(true);
    }
    else {
        viewModel.order(false);
        if (window.globals.state == "view") {

            estimateViewModel.wholesale.wholesaleDisable();
        }
        else {
            estimateViewModel.wholesale.wholesaleEnable();
        }
    }

    setTimeout(function () {
        estimateViewModel.inspection.typeEstimate(window.globals.estimateType);
        estimateViewModel.inspection.initCarInspections(window.globals.inspection, estimateViewModel.effortsModel);
    }, 500);

    setupEmail();
    setupPrint();
    $.validator.unobtrusive.parse(document);
    $("form").validate();
    DisableFormSubmissionByEnter();
});

//=================== CustomerWholesale =====================

function CustomerWholesaleInfo(parent) {
    var self = this;
    self.parent = parent;
    self.wholesaleId = $('#Customer_Wholesale_CustomerId');
    self.wholesalaMatrixId = $('#Customer_Wholesale_MatrixId');

    self.wholesaleDisable = function() {
        self.wholesaleId.attr('disabled', 'disabled');
        self.wholesalaMatrixId.attr('disabled', 'disabled');
        self.parent.selectBox.selectBox('disable');
    };

    self.wholesaleEnable = function () {
        self.wholesaleId.removeAttr('disabled');
        self.wholesalaMatrixId.removeAttr('disabled');
        self.parent.selectBox.selectBox('enable');
    };

    self.wholesaleCustomerDataBindChange = function() {
        self.wholesaleId.bind('change', function() { self.parent.updateMatrixData(); });
        self.wholesalaMatrixId.bind('change', function() { self.parent.updateData(); });
    };

    self.wholesaleCustomerDataUnBindChange = function() {
        self.wholesaleId.unbind('change');
        self.wholesalaMatrixId.unbind('change');
    };
}