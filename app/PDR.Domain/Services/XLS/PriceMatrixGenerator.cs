using PDR.Domain.Model.Matrixes;

namespace PDR.Domain.Services.XLS
{
    using System;

    using PDR.Domain.Model.Enums;

    public static class PriceMatrixGenerator
    {
        public static Matrix Generate(Matrix matrix)
        {
            var rnd = new Random();
            foreach (var part in Enum.GetValues(typeof(PartOfBody)))
            {
                foreach (var dent in Enum.GetValues(typeof(TotalDents)))
                {
                    foreach (var size in Enum.GetValues(typeof(AverageSize)))
                    {                        
                        var price = new MatrixPrice(true)
                            {
                                AverageSize = (AverageSize)size, 
                                Cost = rnd.Next(1, 1000), 
                                Matrix = matrix, 
                                PartOfBody = (PartOfBody)part, 
                                TotalDents = (TotalDents)dent
                            };
                        matrix.MatrixPrices.Add(price);
                    }
                }
            }

            return matrix;
        }
    }
}
