using Locomotiv.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Locomotiv.Utils.Services.Interfaces
{
    public interface IInspectionService
    {
        IEnumerable<Inspection> GetByTrainId(int trainId);

        Inspection CreerInspection(
            int trainId,
            int mecanicienId,
            TypeInspection type,
            ResultatInspection resultat,
            string observations
        );

        void SupprimerInspection(int inspectionId);
    }
}
