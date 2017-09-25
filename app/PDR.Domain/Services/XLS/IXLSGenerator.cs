using System.Collections.Generic;
using System.IO;

using PDR.Domain.Model;
using PDR.Domain.Model.Matrixes;
using PDR.Domain.Model.Users;

namespace PDR.Domain.Services.XLS
{
    public interface IXLSGenerator
    {
        byte[] GenerateXLS(Matrix matrix);

        Matrix ImportMatrixFromXLS(string xlsFilePath, Matrix matrix, Admin newAdmin = null);

        Matrix ImportMatrixFromXLS(Stream stream, Matrix matrix, Admin newAdmin = null);

        void InvoiceToXLSConverter(IEnumerable<Invoice> invoices);
    }
}