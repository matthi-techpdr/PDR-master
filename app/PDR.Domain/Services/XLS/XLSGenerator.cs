using System.Collections;
using System.Collections.Generic;
using System.IO;

using PDR.Domain.Model;
using PDR.Domain.Model.Matrixes;
using PDR.Domain.Model.Users;

namespace PDR.Domain.Services.XLS
{
    public class XLSGenerator : IXLSGenerator
    {
        public byte[] GenerateXLS(Matrix matrix)
        {
            var matrixToXLSConverter = new MatrixToXLSConverter();
            return matrixToXLSConverter.Create(matrix);
        }

        public Matrix ImportMatrixFromXLS(string xlsFilePath, Matrix matrix, Admin newAdmin = null)
        {
            var xlsToMatrixConverter = new XLSToMatrixConverter(xlsFilePath, matrix);
            return xlsToMatrixConverter.Import(newAdmin);
        }

        public Matrix ImportMatrixFromXLS(Stream stream, Matrix matrix, Admin newAdmin = null)
        {
            var xlsToMatrixConverter = new XLSToMatrixConverter(stream, matrix);
            return xlsToMatrixConverter.Import(newAdmin);
        }

        public void InvoiceToXLSConverter(IEnumerable<Invoice> invoices)
        {
            var invoiceToXLSConverter = new InvoiceToXLSConverter(invoices);
        }
    }
}