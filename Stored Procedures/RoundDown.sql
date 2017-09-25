Create Function dbo.RoundDown(@floatValue float, @Digits Int)
Returns Decimal(32,16)
AS
Begin
	DECLARE @Val Decimal(32,16);
	SET @Val = CAST(@floatValue AS Decimal(32,16));

    Return Case When Abs(@Val - Round(@Val, @Digits, 1)) * Power(10, @Digits+1) = 5 
                Then Floor(@Val * Power(10,@Digits))/Power(10, @Digits)
                Else Round(@Val, @Digits)
                End
End