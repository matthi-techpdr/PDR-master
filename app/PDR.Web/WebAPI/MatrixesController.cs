using System.Collections.Generic;
using System.Linq;
using PDR.Domain.Model.Matrixes;
using PDR.Web.WebAPI.IphoneModels;

namespace PDR.Web.WebAPI
{
    public class MatrixesController : BaseWebApiController<Matrix, ApiMatrixModel>
    {
        public IList<ApiMatrixModel> Get(bool isDefault)
        {
            List<Matrix> defaultMatrixes = this.Repository.Where(x => (x is DefaultMatrix) == isDefault).ToList();
            List<ApiMatrixModel> model = new List<ApiMatrixModel>();
            foreach (var defaultMatrix in defaultMatrixes)
            {
                model.Add(new ApiMatrixModel(defaultMatrix));
            }
            return model;
        }
    }
}