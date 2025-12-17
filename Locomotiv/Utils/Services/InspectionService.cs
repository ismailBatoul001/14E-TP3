using Locomotiv.Model;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Locomotiv.Utils.Services
{
    public class InspectionService : IInspectionService
    {
        private readonly IInspectionDAL _inspectionDal;
        private readonly IUserDAL _userDal;

        public InspectionService(IInspectionDAL inspectionDal, IUserDAL userDal)
        {
            _inspectionDal = inspectionDal;
            _userDal = userDal;
        }

        public IEnumerable<Inspection> GetByTrainId(int trainId)
            => _inspectionDal.GetByTrainId(trainId);

        public Inspection CreerInspection(int trainId, int mecanicienId, TypeInspection type, ResultatInspection resultat, string observations)
        {
            var user = _userDal.GetById(mecanicienId);
            if (user == null)
                throw new InvalidOperationException("Mécanicien introuvable.");

            if (user.Role != Role.Mecanicien)
                throw new UnauthorizedAccessException("Seuls les mécaniciens peuvent créer une inspection.");

            var inspection = new Inspection
            {
                TrainId = trainId,
                MecanicienId = mecanicienId,
                DateInspection = DateTime.Now,
                TypeInspection = type,
                Resultat = resultat,
                Observations = observations ?? string.Empty
            };

            _inspectionDal.Add(inspection);
            return inspection;
        }

        public void SupprimerInspection(int inspectionId)
            => _inspectionDal.Delete(inspectionId);
    }
}
