function EffortsModel() {
    var self = this;
    self.effortsArray = [];
    self.initEffortsArray = function () {
        self.effortsArray.push(
            new EffortLine(0, 0.0,
                [new EffortItem(21, 'Hood', 0.0, 0.0, 'CarInspectionsModel.CarInspections[0].EffortItems', 0, 2),
                    new EffortItem(22, 'Insulator', 0.0, null, 'CarInspectionsModel.CarInspections[0].EffortItems', 1, 2),
                    new EffortItem(23, 'Emblem', 0.0, null, 'CarInspectionsModel.CarInspections[0].EffortItems', 2, 2),
                    new EffortItem(24, 'Cowl', 0.0, 0.0, 'CarInspectionsModel.CarInspections[0].EffortItems', 3, 2),
                    new EffortItem(25, 'Labels', null, 0.0, 'CarInspectionsModel.CarInspections[0].EffortItems', 4, 2)
                ]),
            new EffortLine(1, 0.0,
                [new EffortItem(26, 'Luggage Rack', 0.0, null, 'CarInspectionsModel.CarInspections[1].EffortItems', 0, 2),
                    new EffortItem(27, 'Headliner', 0.0, null, 'CarInspectionsModel.CarInspections[1].EffortItems', 1, 2),
                    new EffortItem(28, 'Sunroof Frame', 0.0, null, 'CarInspectionsModel.CarInspections[1].EffortItems', 2, 2),
                    new EffortItem(29, 'Antenna', 0.0, null, 'CarInspectionsModel.CarInspections[1].EffortItems', 3, 2),
                    new EffortItem(30, 'High Mount Lamp', 0.0, 0.0, 'CarInspectionsModel.CarInspections[1].EffortItems', 4, 2),
                    new EffortItem(31, 'Back Glass', 0.0, 0.0, 'CarInspectionsModel.CarInspections[1].EffortItems', 5, 2),
                    new EffortItem(32, 'L Roof Molding', 0.0, 0.0, 'CarInspectionsModel.CarInspections[1].EffortItems', 6, 2),
                    new EffortItem(33, 'R Roof Molding', 0.0, 0.0, 'CarInspectionsModel.CarInspections[1].EffortItems', 7, 2)
                ]),
            new EffortLine(2, 0.0,
                [new EffortItem(34, 'Trunk Lid', 0.0, 0.0, 'CarInspectionsModel.CarInspections[2].EffortItems', 0, 2),
                    new EffortItem(35, 'Trim', 0.0, null, 'CarInspectionsModel.CarInspections[2].EffortItems', 1, 2),
                    new EffortItem(36, 'Spoiler', 0.0, null, 'CarInspectionsModel.CarInspections[2].EffortItems', 2, 2),
                    new EffortItem(37, 'Handle', 0.0, null, 'CarInspectionsModel.CarInspections[2].EffortItems', 3, 2),
                    new EffortItem(38, 'Bumper Cover', 0.0, 0.0, 'CarInspectionsModel.CarInspections[2].EffortItems', 4, 2),
                    new EffortItem(39, 'Emblems', null, 0.0, 'CarInspectionsModel.CarInspections[2].EffortItems', 5, 2)
                ]),
            new EffortLine(3, 0.0,
                [new EffortItem(61, 'Front Lamp', 0.0, 0.0, 'CarInspectionsModel.CarInspections[3].EffortItems', 0, 2),
                    new EffortItem(62, 'Stripe', null, 0.0, 'CarInspectionsModel.CarInspections[3].EffortItems', 1, 2)
                ]),
            new EffortLine(4, 0.0,
                [new EffortItem(59, 'Front Lamp', 0.0, 0.0, 'CarInspectionsModel.CarInspections[4].EffortItems', 0, 2),
                    new EffortItem(60, 'Stripe', null, 0.0, 'CarInspectionsModel.CarInspections[4].EffortItems', 1, 2)
                ]),
            new EffortLine(5, 0.0,
                [new EffortItem(63, 'Belt Molding', 0.0, 0.0, 'CarInspectionsModel.CarInspections[5].EffortItems', 0, 2),
                    new EffortItem(64, 'Upper Molding', 0.0, 0.0, 'CarInspectionsModel.CarInspections[5].EffortItems', 1, 2),
                    new EffortItem(65, 'Applique Molding', 0.0, 0.0, 'CarInspectionsModel.CarInspections[5].EffortItems', 2, 2),
                    new EffortItem(66, 'Mirrors', 0.0, null, 'CarInspectionsModel.CarInspections[5].EffortItems', 3, 2),
                    new EffortItem(67, 'Handle', 0.0, null, 'CarInspectionsModel.CarInspections[5].EffortItems', 4, 2),
                    new EffortItem(68, 'Door Trim', 0.0, null, 'CarInspectionsModel.CarInspections[5].EffortItems', 5, 2),
                    new EffortItem(69, 'Bodyside Molding', 0.0, null, 'CarInspectionsModel.CarInspections[5].EffortItems', 6, 2),
                    new EffortItem(70, 'Mirror Glass', 0.0, 0.0, 'CarInspectionsModel.CarInspections[5].EffortItems', 7, 2),
                    new EffortItem(71, 'Stripe', 0.0, 0.0, 'CarInspectionsModel.CarInspections[5].EffortItems', 8, 2)
                ]),
            new EffortLine(6, 0.0,
                [new EffortItem(72, 'Belt Molding', 0.0, 0.0, 'CarInspectionsModel.CarInspections[6].EffortItems', 0, 2),
                    new EffortItem(73, 'Upper Molding', 0.0, 0.0, 'CarInspectionsModel.CarInspections[6].EffortItems', 1, 2),
                    new EffortItem(74, 'Applique Molding', 0.0, 0.0, 'CarInspectionsModel.CarInspections[6].EffortItems', 2, 2),
                    new EffortItem(75, 'Handle', 0.0, null, 'CarInspectionsModel.CarInspections[6].EffortItems', 3, 2),
                    new EffortItem(76, 'Door Trim', 0.0, null, 'CarInspectionsModel.CarInspections[6].EffortItems', 4, 2),
                    new EffortItem(77, 'Bodyside Molding', 0.0, null, 'CarInspectionsModel.CarInspections[6].EffortItems', 5, 2),
                    new EffortItem(78, 'Stripe', 0.0, 0.0, 'CarInspectionsModel.CarInspections[6].EffortItems', 6, 2)
                ]),
            new EffortLine(7, 0.0, null),
            new EffortLine(8, 0.0,
                [new EffortItem(50, 'Belt Molding', 0.0, 0.0, 'CarInspectionsModel.CarInspections[8].EffortItems', 0, 2),
                    new EffortItem(51, 'Upper Molding', 0.0, 0.0, 'CarInspectionsModel.CarInspections[8].EffortItems', 1, 2),
                    new EffortItem(52, 'Applique Molding', 0.0, 0.0, 'CarInspectionsModel.CarInspections[8].EffortItems', 2, 2),
                    new EffortItem(53, 'Mirrors', 0.0, null, 'CarInspectionsModel.CarInspections[8].EffortItems', 3, 2),
                    new EffortItem(54, 'Handle', 0.0, null, 'CarInspectionsModel.CarInspections[8].EffortItems', 4, 2),
                    new EffortItem(55, 'Door Trim', 0.0, null, 'CarInspectionsModel.CarInspections[8].EffortItems', 5, 2),
                    new EffortItem(56, 'Bodyside Molding', 0.0, null, 'CarInspectionsModel.CarInspections[8].EffortItems', 6, 2),
                    new EffortItem(57, 'Mirror Glass', 0.0, 0.0, 'CarInspectionsModel.CarInspections[8].EffortItems', 7, 2),
                    new EffortItem(58, 'Stripe', 0.0, 0.0, 'CarInspectionsModel.CarInspections[8].EffortItems', 8, 2)
                ]),
            new EffortLine(9, 0.0,
                [new EffortItem(43, 'Belt Molding', 0.0, 0.0, 'CarInspectionsModel.CarInspections[9].EffortItems', 0, 2),
                    new EffortItem(44, 'Upper Molding', 0.0, 0.0, 'CarInspectionsModel.CarInspections[9].EffortItems', 1, 2),
                    new EffortItem(45, 'Applique Molding', 0.0, 0.0, 'CarInspectionsModel.CarInspections[9].EffortItems', 2, 2),
                    new EffortItem(46, 'Handle', 0.0, null, 'CarInspectionsModel.CarInspections[9].EffortItems', 3, 2),
                    new EffortItem(47, 'Door Trim', 0.0, null, 'CarInspectionsModel.CarInspections[9].EffortItems', 4, 2),
                    new EffortItem(48, 'Bodyside Molding', 0.0, null, 'CarInspectionsModel.CarInspections[9].EffortItems', 5, 2),
                    new EffortItem(49, 'Stripe', 0.0, 0.0, 'CarInspectionsModel.CarInspections[9].EffortItems', 6, 2)
                ]),
            new EffortLine(10, 0.0, null),
            new EffortLine(11, 0.0,
                [new EffortItem(79, 'Rear Lamp', 0.0, 0.0, 'CarInspectionsModel.CarInspections[11].EffortItems', 0, 2),
                    new EffortItem(80, 'Glass', 0.0, 0.0, 'CarInspectionsModel.CarInspections[11].EffortItems', 1, 2),
                    new EffortItem(81, 'Stripe', null, 0.0, 'CarInspectionsModel.CarInspections[11].EffortItems', 2, 2)
                ]),
            new EffortLine(12, 0.0,
                [new EffortItem(40, 'Rear Lamp', 0.0, 0.0, 'CarInspectionsModel.CarInspections[12].EffortItems', 0, 2),
                    new EffortItem(41, 'Glass', 0.0, 0.0, 'CarInspectionsModel.CarInspections[12].EffortItems', 1, 2),
                    new EffortItem(42, 'Stripe', null, 0.0, 'CarInspectionsModel.CarInspections[12].EffortItems', 2, 2)
                ]),
            new EffortLine(13, 0.0, null),
            new EffortLine(14, 0.0, null),
            new EffortLine(15, 0.0, null),
            new EffortLine(16, 0.0, [new EffortItem(263, 'Cowl', 0.0, 0.0, 'CarInspectionsModel.CarInspections[16].EffortItems', 0, 2)
                ]),
            new EffortLine(17, 0.0,
                [new EffortItem(82, 'Front Bumper', 0.0, 0.0, 'CarInspectionsModel.CarInspections[17].EffortItems', 0, 2)
                ]),
            new EffortLine(18, 0.0,
                [new EffortItem(83, 'Rear Bumper', 0.0, 0.0, 'CarInspectionsModel.CarInspections[18].EffortItems', 0, 2)
                ]));
    };
}