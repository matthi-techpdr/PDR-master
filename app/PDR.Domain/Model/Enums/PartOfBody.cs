using PDR.Domain.Model.Attributes;

namespace PDR.Domain.Model.Enums
{
	public enum PartOfBody
	{
        [CarPartImage(6)]
        [TruckPartImage(3)]
		Hood = 0,

        [CarPartImage(8)]
        [TruckPartImage(9)]
		Roof = 1,

        [CarPartImage(10)]
        [TruckPartImage(15)]
		DeckLid = 2,
        
        [CarPartImage(12)]
        [TruckPartImage(4)]
        RFender = 3,

        [CarPartImage(1)]
        [TruckPartImage(2)]
        LFender = 4,

        [CarPartImage(13)]
        [TruckPartImage(11)]
        RFDoor = 5,

        [CarPartImage(15)]
        [TruckPartImage(12)]
        RRDoor = 6,

        [TruckPartImage(13)]
        RCabCorner = 7,

        [CarPartImage(2)]
        [TruckPartImage(5)]
        LFDoor = 8,

        [CarPartImage(3)]
        [TruckPartImage(6)]
        LRDoor = 9,

        [TruckPartImage(7)]
        LCabCorner = 10,

        [CarPartImage(14)]
        [TruckPartImage(17)]
        RQuarter = 11,

        [CarPartImage(4)]
        [TruckPartImage(14)]
        LQuarter = 12,

        MetalSunroof = 13,

        [CarPartImage(7)]
        [TruckPartImage(8)]
        LtRoofRail = 14,

        [CarPartImage(9)]
        [TruckPartImage(10)]
		RtRoofRail = 15,

        Other = 16,

        [CarPartImage(5)]
        [TruckPartImage(1)]
		FrontBumper = 17,

        [CarPartImage(11)]
        [TruckPartImage(16)]
		RearBumper = 18,
	}
}
