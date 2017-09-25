function PartsTemp() {
    var self = this;
    self.parts = [];
    self.remove = function () {
        self.parts.length = 0;
    };

    self.add = function (item) {
        self.parts.push(item);
    };

    self.addRange = function(items) {
        for (var i = 0, len = items.length; i < len;) {
            self.setTemp(items[i++]);
        }
    };

    self.setTemp = function (part) {
        var partTemp = new PartTemp();
        partTemp.aluminium = part.Aluminium();
        part.Aluminium(false);
        partTemp.doublemetal = part.DoubleMetal();
        part.DoubleMetal(false);
        partTemp.oversizedroof = part.OversizedRoof();
        part.OversizedRoof(false);
        partTemp.averagesize = part.selectedAverageSize();
        part.selectedAverageSize(-1);
        partTemp.totaldents = part.selectedTotalDents();
        part.selectedTotalDents(-1);
        part.change();
        partTemp.amountoversizeddents = part.amountOversizedDents();
        partTemp.oversizeddentscost = part.oversizedDentsCost();
        part.amountOversizedDents(0);
        partTemp.corrosionprotection = part.CorrosionProtection();
        partTemp.corrosionprotectioncost = part.CorrosionProtectionCost();
        part.CorrosionProtection(false);

        self.add(partTemp);
    };
}

function PartTemp() {
    var self = this;
    self.aluminium = false;
    self.doublemetal = false;
    self.oversizedroof = false;
    self.corrosionprotection = false;
    self.corrosionprotectioncost = 0;
    self.averagesize = -1;
    self.totaldents = -1;
    self.amountoversizeddents = 0;
    self.oversizeddentscost = 0;
}