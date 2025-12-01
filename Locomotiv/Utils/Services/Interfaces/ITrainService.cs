using Locomotiv.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Locomotiv.Utils.Services.Interfaces
{
    public interface ITrainService
    {
        void AjouterTrain(string numero, TypeTrain type, EtatTrain etat, int capacite, int stationId);
        void ModifierTrain(Train train);
        void SupprimerTrain(Train train);
        Train? GetById(int id);
        IList<Train> GetByStation(int stationId);
        int CompterTrainsStation(int stationId);
        IEnumerable<Train> GetAll();
    }
}
